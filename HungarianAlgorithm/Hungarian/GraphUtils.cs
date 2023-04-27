using QuikGraph;

namespace Hungarian
{
    public static class GraphUtils
    {
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

        public static void WriteAlEdges(IUndirectedGraph<int, Edge<int>> graph)
        {
            foreach (var v in graph.Vertices)
            {
                Console.Write($"{v} ->");
                foreach (var edge in graph.AdjacentEdges(v))
                {
                    Console.Write($"{edge.GetOtherVertex(v)}, ");
                }
                Console.WriteLine();
            }
        }
    }
}
