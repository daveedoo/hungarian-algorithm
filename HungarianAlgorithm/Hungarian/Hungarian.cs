using QuikGraph;
using QuikGraph.Collections;

namespace Hungarian
{
    public class Hungarian
    {
        private readonly ProblemInstance _problemInstance;

        private readonly double[,] _distances; // [house_index, well_index]

        private readonly VertexLabelledGraph _graph;

        public Hungarian(ProblemInstance problem)
        {
            _problemInstance = problem;
            _distances = CreateDistancesMatrixBasedOnProblemInstance(problem);
            var graph = GraphUtils.CreateGraphBasedOnProblemInstance(problem);

            // TODO: Move creation of LabelledGraph elsewhere
            int n = _problemInstance.N * _problemInstance.K;
            var taggedEdges = graph.Edges.Select(e =>
            {
                double cost = e.Source < n ? GetDistanceBetweenHouseAndWell(e.Source, e.Target) : GetDistanceBetweenHouseAndWell(e.Target, e.Source);
                return new TaggedUndirectedEdge<int, double>(e.Source, e.Target, cost);
            });
            var taggedEdgesGraph = new UndirectedGraph<int, TaggedUndirectedEdge<int, double>>();
            taggedEdgesGraph.AddVertexRange(Enumerable.Range(0, 2 * n));
            taggedEdgesGraph.AddEdgeRange(taggedEdges);

            _graph = new VertexLabelledGraph(taggedEdgesGraph);
        }

        public Solution Solve()
        {
            int N = _problemInstance.N * _problemInstance.K;
            var Matching = new EdgeList<int, Edge<int>>();
            
            GetStartingPotential(_graph, out double[] _, out IMutableBidirectionalGraph<int, Edge<int>> eqGraph);
            var H = Enumerable.Range(0, N).ToHashSet();     // all house vertices
            var W = Enumerable.Range(N, N).ToHashSet();     // all well vertices
            bool[] verticesMatched = new bool[2 * N];

            while (Matching.Count < N)
            {
                int free_s = verticesMatched.TakeWhile(h => h == true).Count();     // root of the alternating tree
                var S = new HashSet<int> { free_s };                                // set of alternating tree vertices from H
                var T = new HashSet<int>();                                         // set of alternating tree vertices from W

                List<Edge<int>> path = null;
                while (path is null)
                {
                    if (AreAllNeighboursInSet(eqGraph, S, T, out int? nextT))   // TODO: call this method once and only update bool when S or T is modified
                    {
                        // improving potential
                        var WexceptT = W.Except(T);
                        double delta = S.Min(s =>
                            WexceptT.Min(w =>
                            {
                                _graph.TryGetEdge(s, w, out var edge);
                                return edge.Tag - _graph.GetVertexLabel(s) - _graph.GetVertexLabel(w);
                            })
                        );

                        // TODO: use wellsSlackness
                        foreach (int s in S)
                        {
                            _graph.AddValueToVertexLabel(s, delta);
                        }
                        foreach (int t in T)
                        {
                            _graph.AddValueToVertexLabel(t, -delta);
                        }

                        // Update eqGraph
                        foreach (int s in S)
                        {
                            foreach (var edge in _graph.AdjacentEdges(s))
                            {
                                if (!eqGraph.ContainsEdge(edge.Source, edge.Target) &&
                                    edge.Tag == _graph.GetVertexLabel(edge.Source) + _graph.GetVertexLabel(edge.Target))
                                {
                                    eqGraph.AddEdge(new Edge<int>(edge.Source, edge.Target));
                                    nextT = edge.GetOtherVertex(s);
                                }
                            }
                        }
                    }

                    // nextT is vertex from (N_p(S) \ T)
                    var nextTMatchingEdge = Matching.Find(e => e.Source == nextT || e.Target == nextT);
                    if (nextTMatchingEdge is null)
                    {
                        path = new List<Edge<int>>();

                        Edge<int> pathEdge = eqGraph.InEdges(nextT!.Value).First();
                        while (pathEdge.Source != free_s)
                        {
                            path.Add(pathEdge);
                            pathEdge = eqGraph.InEdges(pathEdge.Source).First();
                        }
                        path.Add(pathEdge);
                    }
                    else
                    {
                        S.Add(nextTMatchingEdge.GetOtherVertex(nextT!.Value));
                        T.Add(nextT!.Value);
                    }
                }


                // augment M by path TODO: tidy it up
                for (int i = 0; i < path.Count / 2; i += 2)
                {
                    var edgeToAdd = path[2 * i];
                    Matching.Add(edgeToAdd);

                    var eqEdgeToAdd = eqGraph.OutEdges(edgeToAdd.Source).Where(e => e.Target == edgeToAdd.Target).Single();
                    eqGraph.RemoveEdge(eqEdgeToAdd);
                    eqGraph.AddEdge(new Edge<int>(eqEdgeToAdd.Target, eqEdgeToAdd.Source));


                    var edgeToRemove = path[2 * i + 1];
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
            for (int well = N * K; well < K * (N + 1); well++)
            {
                var suppliedHouses = new List<(int index, double cost)>();
                for (int wellCopy = well; wellCopy < 2 * N * K; wellCopy += K)
                {
                    var house = matching.Find(e => e.Source == wellCopy || e.Target == wellCopy).GetOtherVertex(wellCopy);
                    _graph.TryGetEdge(house, wellCopy, out TaggedUndirectedEdge<int, double> edge);
                    var cost = edge.Tag;

                    suppliedHouses.Add((house, cost));
                }

                assignments.Add(new WellAssignments(well - N * K, suppliedHouses));
            }

            return new Solution(assignments);
        }

        private bool AreAllNeighboursInSet(IImplicitGraph<int, Edge<int>> graph, ISet<int> sourcesSet, ISet<int> neighboursSet, out int? freeNeighbour)
        {
            freeNeighbour = null;

            foreach (int s in sourcesSet)
            {
                foreach (var edge in graph.OutEdges(s))
                {
                    int neighbour = edge.GetOtherVertex(s);
                    if (!neighboursSet.Contains(neighbour))
                    {
                        freeNeighbour = neighbour;
                        break;
                    }
                }

                if (freeNeighbour is not null)
                    break;
            }
            return freeNeighbour is null;
        }

        private double[] GetStartingPotential(VertexLabelledGraph graph, out double[] wellsSlackness, out IMutableBidirectionalGraph<int, Edge<int>> equalityGraph)
        {
            var eqGraph = new BidirectionalGraph<int, Edge<int>>();
            eqGraph.AddVertexRange(graph.Vertices);

            int N = _problemInstance.N * _problemInstance.K;
            var potential = new double[2 * N];

            for (int w = N; w < 2 * N; w++)
            {
                var startLabel = graph.AdjacentEdges(w).Min(e => e.Tag);
                graph.SetVertexLabel(w, startLabel);

                var equalityEdges = graph.AdjacentEdges(w).Where(e => e.Tag == startLabel)
                    .Select(e => new Edge<int>(e.GetOtherVertex(w), w));    // directed vertex: house -> well
                eqGraph.AddEdgeRange(equalityEdges);
            }

            wellsSlackness = new double[N]; // equal to 0 for every well because that's how starting potential is constructed
            equalityGraph = eqGraph;
            return potential;
        }

        private double[,] CreateDistancesMatrixBasedOnProblemInstance(ProblemInstance problem)
        {
            var costMatrix = new double[problem.N * problem.K, problem.N];

            for (int i = 0; i < problem.N * problem.K; i++)
            {
                for (int j = 0; j < problem.N; j++)
                {
                    costMatrix[i, j] = CalculateDistanceBetweenHouseAndWell(i, j);
                }
            }

            return costMatrix;
        }

        private double CalculateDistanceBetweenHouseAndWell(int houseIndex, int wellIndex)
        {
            (double x, double y) houseLocation = _problemInstance.HousesLocations[houseIndex];
            (double x, double y) wellLocation = _problemInstance.WellsLocations[wellIndex];
            return Math.Sqrt((houseLocation.x - wellLocation.x) * (houseLocation.x - wellLocation.x) +
                             (houseLocation.y - wellLocation.y) * (houseLocation.y - wellLocation.y));
        }

        private double GetDistanceBetweenHouseAndWell(int houseIndex, int wellIndex)
        {
            return _distances[houseIndex, wellIndex % _problemInstance.N];
        }
    }
}
