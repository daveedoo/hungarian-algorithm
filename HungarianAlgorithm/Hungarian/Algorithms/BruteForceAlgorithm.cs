namespace Hungarian.Algorithms
{
    public class BruteForceAlgorithm : IAlgorithm
    {
        private readonly ProblemInstance _problemInstance;

        private decimal[,] _distances { get; set; } // [house_index, well_index]

        public BruteForceAlgorithm(ProblemInstance problem)
        {
            _problemInstance = problem;
        }

        public Solution Solve(decimal[,] distances)
        {
            _distances = distances;

            List<int> housesList = new List<int>(Enumerable.Range(0, _problemInstance.N * _problemInstance.K));
            List<int> bestAssignment = new List<int>();
            var bestAssignmentCost = decimal.MaxValue;

            foreach (var assignment in Permutate(housesList, housesList.Count))
            {
                var assignmentCost = GetAssignmentCost(assignment);
                if (assignmentCost < bestAssignmentCost)
                {
                    bestAssignment = new List<int>(assignment);
                    bestAssignmentCost = assignmentCost;
                }
            }

            return CreateSolution(bestAssignment);
        }

        private void RotateRight(List<int> sequence, int count)
        {
            int tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        private IEnumerable<List<int>> Permutate(List<int> sequence, int count)
        {
            if (count == 1) yield return sequence;
            else
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var perm in Permutate(sequence, count - 1))
                        yield return perm;
                    RotateRight(sequence, count);
                }
            }
        }

        private decimal GetDistanceBetweenHouseAndWell(int houseIndex, int wellIndex)
        {
            return _distances[houseIndex, wellIndex / _problemInstance.K];
        }

        private decimal GetAssignmentCost(List<int> assignment)
        {
            decimal cost = 0.0M;
            foreach (var (house, index) in assignment.Select((house, index) => (house, index)))
            {
                cost += GetDistanceBetweenHouseAndWell(house, index);
            }

            return cost;
        }

        private Solution CreateSolution(List<int> houses_permutation)
        {
            int N = _problemInstance.N;
            int K = _problemInstance.K;

            var assignments = new List<WellAssignments>();
            for (int well = 0; well < N; well++)
            {
                var suppliedHouses = new List<(int index, decimal cost)>();
                foreach (var house in houses_permutation.GetRange(well * K, K).OrderBy(i => i))
                {
                    var cost = GetDistanceBetweenHouseAndWell(house, well * K);

                    suppliedHouses.Add((house, cost));
                }

                assignments.Add(new WellAssignments(well, suppliedHouses));
            }

            return new Solution(assignments);
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