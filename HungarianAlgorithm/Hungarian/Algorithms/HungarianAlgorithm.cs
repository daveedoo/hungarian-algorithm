﻿using QuikGraph;
using QuikGraph.Collections;

namespace Hungarian.Algorithms
{
    public class HungarianAlgorithm : IAlgorithm
    {
        private readonly ProblemInstance _problemInstance;

        private decimal[,] _distances { get; set; } // [house_index, well_index]

        private VertexLabelledGraph _graph { get; set; }

        public HungarianAlgorithm(ProblemInstance problem)
        {
            _problemInstance = problem;
            _distances = new decimal[problem.K * problem.N, problem.N];
        }

        public Solution Solve(decimal[,] distances)
        {
            _distances = distances;

            CreateLabelledGraph();

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
                var areAllNeighborsInT = new bool[N];                               // valid for each s in S
                var WExceptT = new HashSet<int>(W);
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
                    int nextT = -1;
                    var edgesToNextTs = new List<Edge<int>>();
                    if (AreAllNeighborsInSet(eqGraph, S, T, areAllNeighborsInT, out Edge<int>? _edgeToNextT))
                    {
                        decimal delta = WExceptT.Select(w => wellsSlackness[w]).Min();
                        foreach (int s in S)
                        {
                            _graph.AddValueToVertexLabel(s, delta);
                        }
                        foreach (int t in T)
                        {
                            _graph.AddValueToVertexLabel(t, -delta);
                        }
                        foreach (var well in WExceptT)
                        {
                            wellsSlackness[well] -= delta;
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

                        var newTVertices = WExceptT.Where(well => wellsSlackness[well] == 0.0m);
                        foreach (var well in newTVertices)
                        {
                            // we add all tightened edges to the eqGraph,
                            // but we need only one of them to add to the alternating tree
                            Edge<int>? edgeToNewT = null;
                            foreach (var s in S)
                            {
                                _graph.TryGetEdge(well, s, out var edge);
                                var cost = edge.Tag;

                                if (!eqGraph.ContainsEdge(well, s) && !eqGraph.ContainsEdge(s, well) &&
                                    cost == _graph.GetVertexLabel(well) + _graph.GetVertexLabel(s))
                                {
                                    var newEqEdge = new Edge<int>(s, well);
                                    eqGraph.AddEdge(newEqEdge);

                                    //areAllNeighborsInT[s] = false;
                                    if (edgeToNewT is null)
                                        edgeToNewT = newEqEdge;
                                }
                            }
                            edgesToNextTs.Add(edgeToNewT!);
                        }
                    }
                    else
                    {
                        // only one edge added to the list if we are here
                        edgesToNextTs.Add(_edgeToNextT!);
                    }

                    // all of newTVertices need to be added to T
                    // we can then process them all at once
                    foreach (var edgeToNextT in edgesToNextTs)
                    {
                        alternatingTree.AddEdge(edgeToNextT!);
                        nextT = edgeToNextT!.Target;
                        T.Add(nextT);
                        WExceptT.Remove(nextT);

                        // nextT is vertex from (N_p(S) \ T)
                        var nextTMatchingEdge = Matching.Find(e => e.Source == nextT || e.Target == nextT);
                        if (nextTMatchingEdge is null)
                        {
                            path = new List<Edge<int>>();

                            Edge<int> pathEdge = edgeToNextT!;
                            while (pathEdge.Source != free_s)
                            {
                                path.Add(pathEdge);
                                pathEdge = alternatingTree.InEdges(pathEdge.Source).First();
                            }
                            path.Add(pathEdge);
                            break;  // we found the path, so we can break
                        }
                        else
                        {
                            int newS = nextTMatchingEdge.GetOtherVertex(nextT);
                            S.Add(newS);
                            alternatingTree.AddEdge(new Edge<int>(nextT, newS));
                        
                            // SLACK:: update necessary values
                            foreach (var well in WExceptT)
                            {
                                _graph.TryGetEdge(well, newS, out var edge);
                                decimal newSSlackness = edge.Tag - _graph.GetVertexLabel(edge.Source) - _graph.GetVertexLabel(edge.Target);
                                if (newSSlackness < wellsSlackness[well])
                                    wellsSlackness[well] = newSSlackness;
                            }
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

                suppliedHouses = suppliedHouses.OrderBy(sh => sh.index).ToList();
                assignments.Add(new WellAssignments(well - N * K, suppliedHouses));
            }

            return new Solution(assignments);
        }

        private bool AreAllNeighborsInSet(IImplicitGraph<int, Edge<int>> graph, ISet<int> sourcesSet, ISet<int> neighboursSet,
            bool[] allNeighborsDone, out Edge<int>? edgeToNextT)
        {
            edgeToNextT = null;

            foreach (int s in sourcesSet)
            {
                if (!allNeighborsDone[s])
                {
                    foreach (var edge in graph.OutEdges(s))
                    {
                        int neighbour = edge.GetOtherVertex(s);
                        if (!neighboursSet.Contains(neighbour))
                        {
                            edgeToNextT = edge;
                            return false;
                        }
                    }
                    allNeighborsDone[s] = true;
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

        private void CreateLabelledGraph()
        {
            var graph = GraphUtils.CreateGraphBasedOnProblemInstance(_problemInstance);

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

        public Solution Solve(int[,] distances)
        {
            int firstDimension = distances.GetLength(0);
            int secondDimension = distances.GetLength(1);

            var decimalDistances = new decimal[firstDimension, secondDimension];
            for (int i = 0; i < firstDimension; i++)
            {
                for (int j = 0; j < secondDimension; j++)
                {
                    decimalDistances[i, j] = distances[i, j];
                }
            }

            return Solve(decimalDistances);
        }
    }
}
