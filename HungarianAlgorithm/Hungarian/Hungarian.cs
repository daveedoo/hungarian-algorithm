using QuikGraph;

namespace Hungarian
{
    public class Hungarian
    {
        private readonly ProblemInstance _problemInstance;

        private readonly double[,] _distances; // [house_index, well_index]

        private readonly IUndirectedGraph<int, Edge<int>> _graph;

        public Hungarian(ProblemInstance problem)
        {
            _problemInstance = problem;
            _graph = GraphUtils.CreateGraphBasedOnProblemInstance(problem);
            _distances = CreateDistancesMatrixBasedOnProblemInstance(problem);
        }

        //var solution = new QuikGraph.Algorithms.Assignment.HungarianAlgorithm(costs);
        public void Solve()
        {
            GraphUtils.WriteAllEdgesWithDistances(_graph, _distances);
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
