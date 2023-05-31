using QuikGraph;
using QuikGraph.Collections;

namespace Hungarian.Algorithms
{
    public class Hungarian : IAlgorithm
    {
        private readonly ProblemInstance _problemInstance;

        private readonly decimal[,] _distances; // [house_index, well_index]

        private readonly VertexLabelledGraph _graph;

        public Hungarian(ProblemInstance problem)
        {
            _problemInstance = problem;
            _distances = problem.CreateDistancesMatrix();
            var graph = GraphUtils.CreateGraphBasedOnProblemInstance(problem);

            // TODO: Move creation of LabelledGraph elsewhere
            int n = _problemInstance.N * _problemInstance.K;
            var taggedEdges = graph.Edges.Select(e =>
            {
                decimal cost = e.Source < n ? GetDistanceBetweenHouseAndWell(e.Source, e.Target) : GetDistanceBetweenHouseAndWell(e.Target, e.Source);
                return new TaggedUndirectedEdge<int, decimal>(e.Source, e.Target, cost);
            });
            var taggedEdgesGraph = new UndirectedGraph<int, TaggedUndirectedEdge<int, decimal>>();
            taggedEdgesGraph.AddVertexRange(Enumerable.Range(0, 2 * n));
            taggedEdgesGraph.AddEdgeRange(taggedEdges);

            _graph = new VertexLabelledGraph(taggedEdgesGraph);
        }

        public Solution Solve()
        {
            int N = _problemInstance.N * _problemInstance.K;
            var Matching = new EdgeList<int, Edge<int>>();

            SetStartingPotential(_graph, out IMutableBidirectionalGraph<int, Edge<int>> eqGraph);

            var H = Enumerable.Range(0, N).ToHashSet();     // all house vertices
            var W = Enumerable.Range(N, N).ToHashSet();     // all well vertices
            bool[] verticesMatched = new bool[2 * N];

            while (Matching.Count < N)
            {
                int free_s = verticesMatched.TakeWhile(h => h == true).Count();     // root of the alternating tree
                var S = new HashSet<int> { free_s };                                // set of alternating tree vertices from H
                var T = new HashSet<int>();                                         // set of alternating tree vertices from W
                var alternatingTree = new BidirectionalGraph<int, Edge<int>>();
                alternatingTree.AddVertexRange(Enumerable.Range(0, 2 * N));

                // SLACK: inital slackness
                var wellsSlackness = new decimal[2 * N];  // first N array values are not used. For convenience of use with wells vertices numbers
                for (int well = N; well < 2 * N; well++)
                {
                    _graph.TryGetEdge(well, free_s, out var edge);
                    wellsSlackness[well] = edge.Tag - _graph.GetVertexLabel(well) - _graph.GetVertexLabel(free_s);
                }


                List<Edge<int>> path = null;
                while (path is null)
                {
                    if (AreAllNeighboursInSet(eqGraph, S, T, out int? nextT, out int? fromS))   // TODO: call this method once and only update bool when S or T is modified
                    {
                        decimal delta = W.Except(T).Select(w => wellsSlackness[w]).Min();
                        foreach (int s in S)
                        {
                            _graph.AddValueToVertexLabel(s, delta);
                        }
                        foreach (int t in T)
                        {
                            _graph.AddValueToVertexLabel(t, -delta);
                        }
                        foreach (var well in W.Except(T))
                        {
                            wellsSlackness[well] -= delta;
                            if (wellsSlackness[well] < 0.0m)
                                throw new Exception("DEBUG only exception");
                        }

                        // Update eqGraph
                        var edgesToRemove = new List<Edge<int>>();
                        foreach (var edge in eqGraph.Edges)
                        {
                            _graph.TryGetEdge(edge.Source, edge.Target, out var e);
                            if (e.Tag != _graph.GetVertexLabel(edge.Source) + _graph.GetVertexLabel(edge.Target))
                            {
                                edgesToRemove.Add(edge);
                            }
                        }
                        foreach (var edge in edgesToRemove)
                        {
                            eqGraph.RemoveEdge(edge);
                        }
                        foreach (var well in W.Except(T))
                        {
                            if (wellsSlackness[well] == 0.0m)
                            {
                                foreach (var s in S)
                                {
                                    _graph.TryGetEdge(well, s, out var edge);
                                    var cost = edge.Tag;

                                    if (!eqGraph.ContainsEdge(well, s) && !eqGraph.ContainsEdge(s, well) &&
                                        cost == _graph.GetVertexLabel(well) + _graph.GetVertexLabel(s))
                                    {
                                        var newEdge = new Edge<int>(s, well);
                                        alternatingTree.AddEdge(newEdge);
                                        eqGraph.AddEdge(newEdge);
                                        nextT = edge.GetOtherVertex(s);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        alternatingTree.AddEdge(new Edge<int>(fromS!.Value, nextT!.Value));
                    }
                    T.Add(nextT!.Value);

                    // nextT is vertex from (N_p(S) \ T)
                    var nextTMatchingEdge = Matching.Find(e => e.Source == nextT || e.Target == nextT);
                    if (nextTMatchingEdge is null)
                    {
                        path = new List<Edge<int>>();

                        Edge<int> pathEdge = alternatingTree.InEdges(nextT!.Value).First();
                        while (pathEdge.Source != free_s)
                        {
                            path.Add(pathEdge);
                            pathEdge = alternatingTree.InEdges(pathEdge.Source).First();
                        }
                        path.Add(pathEdge);
                    }
                    else
                    {
                        int newS = nextTMatchingEdge.GetOtherVertex(nextT!.Value);
                        S.Add(newS);
                        alternatingTree.AddEdge(new Edge<int>(nextT!.Value, newS));

                        // SLACK:: update necessary values
                        foreach (var well in W.Except(T))
                        {
                            _graph.TryGetEdge(well, newS, out var edge);
                            decimal newSSlackness = edge.Tag - _graph.GetVertexLabel(edge.Source) - _graph.GetVertexLabel(edge.Target);
                            if (newSSlackness < wellsSlackness[well])
                                wellsSlackness[well] = newSSlackness;
                        }
                    }
                }

                // augment M by path TODO: tidy it up
                for (int i = 0; i < path.Count - 1; i += 2)
                {
                    var edgeToAdd = path[i];
                    Matching.Add(edgeToAdd);

                    var eqEdgeToAdd = eqGraph.OutEdges(edgeToAdd.Source).Where(e => e.Target == edgeToAdd.Target).Single();
                    eqGraph.RemoveEdge(eqEdgeToAdd);
                    eqGraph.AddEdge(new Edge<int>(eqEdgeToAdd.Target, eqEdgeToAdd.Source));


                    var edgeToRemove = path[i + 1];
                    Matching.RemoveAll(e => e.Source == edgeToRemove.Target && e.Target == edgeToRemove.Source);

                    var eqEdgeToRemove = eqGraph.OutEdges(edgeToRemove.Source).Where(e => e.Target == edgeToRemove.Target).Single();
                    eqGraph.RemoveEdge(eqEdgeToRemove);
                    eqGraph.AddEdge(new Edge<int>(eqEdgeToRemove.Target, eqEdgeToRemove.Source));
                }
                var lastEdge = path.Last();
                Matching.Add(lastEdge);

                var eqEdge = eqGraph.OutEdges(lastEdge.Source).Where(e => e.Target == lastEdge.Target).Single();
                eqGraph.RemoveEdge(eqEdge);
                eqGraph.AddEdge(new Edge<int>(eqEdge.Target, eqEdge.Source));


                verticesMatched[path[0].Target] = true;
                verticesMatched[path.Last().Source] = true;
            }

            return CreateSolution(Matching);
        }

        private Solution CreateSolution(EdgeList<int, Edge<int>> matching)
        {
            int N = _problemInstance.N;
            int K = _problemInstance.K;

            var assignments = new List<WellAssignments>();
            for (int well = N * K; well < (K + 1) * N; well++)
            {
                var suppliedHouses = new List<(int index, decimal cost)>();
                for (int wellCopy = well; wellCopy < 2 * N * K; wellCopy += N)
                {
                    var house = matching.Find(e => e.Source == wellCopy || e.Target == wellCopy).GetOtherVertex(wellCopy);
                    _graph.TryGetEdge(house, wellCopy, out TaggedUndirectedEdge<int, decimal> edge);
                    var cost = edge.Tag;

                    suppliedHouses.Add((house, cost));
                }

                assignments.Add(new WellAssignments(well - N * K, suppliedHouses));
            }

            return new Solution(assignments);
        }

        /// <param name="freeNeighbour">
        ///     Vertex adjacent in graph <paramref name="graph"/> to <paramref name="fromS"/> from <paramref name="sourcesSet"/>.
        ///     Set only if <c>false</c> returned.
        /// </param>
        /// <param name="fromS">Set only if <paramref name="freeNeighbour"/> set.</param>
        /// <returns></returns>
        private bool AreAllNeighboursInSet(IImplicitGraph<int, Edge<int>> graph, ISet<int> sourcesSet, ISet<int> neighboursSet, out int? freeNeighbour, out int? fromS)
        {
            fromS = null;
            freeNeighbour = null;

            foreach (int s in sourcesSet)
            {
                foreach (var edge in graph.OutEdges(s))
                {
                    int neighbour = edge.GetOtherVertex(s);
                    if (!neighboursSet.Contains(neighbour))
                    {
                        fromS = s;
                        freeNeighbour = neighbour;
                        return false;
                    }
                }
            }
            return true;
        }

        private void SetStartingPotential(VertexLabelledGraph graph, out IMutableBidirectionalGraph<int, Edge<int>> equalityGraph)
        {
            var eqGraph = new BidirectionalGraph<int, Edge<int>>();
            eqGraph.AddVertexRange(graph.Vertices);

            int N = _problemInstance.N * _problemInstance.K;
            for (int w = N; w < 2 * N; w++)
            {
                var startLabel = graph.AdjacentEdges(w).Min(e => e.Tag);
                graph.SetVertexLabel(w, startLabel);

                var equalityEdges = graph.AdjacentEdges(w).Where(e => e.Tag == startLabel)
                    .Select(e => new Edge<int>(e.GetOtherVertex(w), w));    // directed vertex: house -> well
                eqGraph.AddEdgeRange(equalityEdges);
            }

            equalityGraph = eqGraph;
        }

        private decimal GetDistanceBetweenHouseAndWell(int houseIndex, int wellIndex)
        {
            return _distances[houseIndex, wellIndex % _problemInstance.N];
        }
    }
}
