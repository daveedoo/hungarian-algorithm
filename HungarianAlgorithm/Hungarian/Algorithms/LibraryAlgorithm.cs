namespace Hungarian.Algorithms
{
    public class LibraryAlgorithm : IAlgorithm
    {
        private readonly ProblemInstance _problemInstance;

        private int[,] _distances { get; set; } // [house_index, well_index]

        public LibraryAlgorithm(ProblemInstance problem)
        {
            _problemInstance = problem;
            _distances = new int[problem.K * problem.N, problem.N];
        }

        public Solution Solve(int[,] distances)
        {
            distances = ReShapeArray(distances);
            _distances = distances;

            var solver = new QuikGraph.Algorithms.Assignment.HungarianAlgorithm(distances.Clone() as int[,]);
            var assignments = solver.Compute();

            return CreateSolution(assignments);
        }

        private Solution CreateSolution(int[] assignment)
        {
            int N = _problemInstance.N;
            int K = _problemInstance.K;

            var assignments = new List<WellAssignments>();
            for (int well = 0; well < N; well++)
            {
                var suppliedHouses = new List<(int index, decimal cost)>();
                for (int i = well; i < N * K; i += N)
                {
                    var house = assignment[i];
                    var cost = GetDistanceBetweenHouseAndWell(well, house);
                    suppliedHouses.Add((house, cost));
                }

                suppliedHouses = suppliedHouses.OrderBy(sh => sh.index).ToList();
                assignments.Add(new WellAssignments(well, suppliedHouses));
            }

            return new Solution(assignments);
        }

        private decimal GetDistanceBetweenHouseAndWell(int houseIndex, int wellIndex)
        {
            return _distances[houseIndex, wellIndex];
        }

        public Solution Solve(decimal[,] distances)
        {
            int firstDimension = distances.GetLength(0);
            int secondDimension = distances.GetLength(1);

            var intDistances = new int[firstDimension, secondDimension];
            for (int i = 0; i < firstDimension; i++)
            {
                for (int j = 0; j < secondDimension; j++)
                {
                    intDistances[i, j] = (int)distances[i, j];
                }
            }

            return Solve(ReShapeArray(intDistances));
        }

        public T[,] ReShapeArray<T>(T[,] array)
        {
            int firstDimension = array.GetLength(0);
            int secondDimension = array.GetLength(1);

            var reshapedArray = new T[secondDimension, firstDimension];
            for (int i = 0; i < firstDimension; i++)
            {
                for (int j = 0; j < secondDimension; j++)
                {
                    reshapedArray[j, i] = array[i, j];
                }
            }

            return reshapedArray;
        }
    }
}