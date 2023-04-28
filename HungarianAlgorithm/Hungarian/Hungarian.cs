namespace Hungarian
{
    public static class Hungarian
    {
        //var solution = new QuikGraph.Algorithms.Assignment.HungarianAlgorithm(costs);
        public static void Solve(int[,] costs)
        {
            var graph = GraphUtils.CreateBipartiteCompleteGraph(costs.GetLength(0), costs.GetLength(1));

            GraphUtils.WriteAlEdges(graph);
        }
    }
}
