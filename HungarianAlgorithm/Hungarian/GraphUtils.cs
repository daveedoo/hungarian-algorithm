using QuikGraph;

namespace Hungarian
{
    public static class GraphUtils
    {
        public static IUndirectedGraph<int, Edge<int>> CreateGraphBasedOnProblemInstance(ProblemInstance problemInstance)
        {
            int vertexCount = problemInstance.N * problemInstance.K;
            return CreateBipartiteCompleteGraph(vertexCount, vertexCount);
        }

        public static IUndirectedGraph<int, Edge<int>> CreateBipartiteCompleteGraph(int set1Size, int set2Size)
        {
            var vertSet1 = Enumerable.Range(0, set1Size);
            var vertSet2 = Enumerable.Range(set1Size, set2Size);

            var graph = new UndirectedGraph<int, Edge<int>>();
            graph.AddVertexRange(vertSet1);
            graph.AddVertexRange(vertSet2);
            for (int v1 = 0; v1 < set1Size; v1++)
            {
                var edges = vertSet2.Select(v2 => new Edge<int>(v1, v2));
                graph.AddEdgeRange(edges);
            }

            return graph;
        }

        public static void WriteAllEdges(IUndirectedGraph<int, Edge<int>> graph)
        {
            foreach (var v in graph.Vertices)
            {
                Console.Write($"{v + 1} ->");
                foreach (var edge in graph.AdjacentEdges(v))
                {
                    Console.Write($"{edge.GetOtherVertex(v) + 1}, ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Writes all edges with costs of base input graph (vertices representing wells are not multiplied)
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="distanceMatrix"></param>
        public static void WriteAllEdgesOfBaseGraphWithDistances(IUndirectedGraph<int, Edge<int>> graph, double[,] distanceMatrix)
        {
            var housesLength = distanceMatrix.GetLength(0);
            var wellsLength = distanceMatrix.GetLength(1);

            var vertices = graph.Vertices.Where(v => v < housesLength);
            foreach (var v in vertices)
            {
                Console.Write($"{v + 1} ->");
                var edges = graph.AdjacentEdges(v).Where(e => e.GetOtherVertex(v) < housesLength + wellsLength);
                foreach (var edge in edges)
                {
                    var u = edge.GetOtherVertex(v) % wellsLength;
                    Console.Write($"{u + 1} (cost: {distanceMatrix[v, u]}), ");
                }
                Console.WriteLine();
            }
        }
    }
}
