using QuikGraph;

namespace Hungarian
{
    public static class Hungarian
    {
        //var solution = new QuikGraph.Algorithms.Assignment.HungarianAlgorithm(costs);
        public static void Solve(int[,] costs)
        {
            var graph = CreateBipartiteCompleteGraph(costs.GetLength(0), costs.GetLength(1));

            WriteAlEdges(graph);
        }
    }
}
