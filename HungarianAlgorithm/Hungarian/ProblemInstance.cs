namespace Hungarian
{
    public class ProblemInstance : IWriteable
    {
        public readonly int N;

        public readonly int K;

        public readonly (double x, double y)[] WellsLocations;

        public readonly (double x, double y)[] HousesLocations;

        public ProblemInstance(int n, int k)
        {
            N = n;
            K = k;
            WellsLocations = new (double x, double y)[n];
            HousesLocations = new (double x, double y)[n * k];
        }

        public void SetWellLocation(int index, double x, double y)
        {
            WellsLocations[index] = (x, y);
        }

        public void SetHouseLocation(int index, double x, double y)
        {
            HousesLocations[index] = (x, y);
        }

        public void Write(TextWriter? textWriter = null)
        {
            textWriter ??= Console.Out; //For some reason cannot be put as parameter

            textWriter.WriteLine($"{N}");
            textWriter.WriteLine($"{K}");

            for (int i = 0; i < N; i++)
            {
                (double x, double y) = WellsLocations[i];
                textWriter.WriteLine($"{i + 1} {x} {y}");
            }

            for (int i = 0; i < K * N; i++)
            {
                (double x, double y) = HousesLocations[i];
                textWriter.WriteLine($"{i + 1} {x} {y}");
            }
        }

        public decimal[,] CreateDistancesDecimalMatrix()
        {
            var costMatrix = new decimal[N * K, N];

            for (int i = 0; i < N * K; i++)
            {
                for (int j = 0; j < N * K; j++)
                {
                    costMatrix[i, j] = CalculateDistanceBetweenHouseAndWell(i, j);
                }
            }

            return costMatrix;
        }

        public int[,] CreateDistancesIntMatrix()
        {
            var costMatrix = new int[N * K, N * K];

            for (int i = 0; i < N * K; i++)
            {
                for (int j = 0; j < N * K; j++)
                {
                    costMatrix[i, j] = (int)CalculateDistanceBetweenHouseAndWell(i, j);
                }
            }

            return costMatrix;
        }

        private decimal CalculateDistanceBetweenHouseAndWell(int houseIndex, int wellIndex)
        {
            (double x, double y) houseLocation = HousesLocations[houseIndex];
            (double x, double y) wellLocation = WellsLocations[wellIndex % N];
            return (decimal)Math.Sqrt((houseLocation.x - wellLocation.x) * (houseLocation.x - wellLocation.x) +
                             (houseLocation.y - wellLocation.y) * (houseLocation.y - wellLocation.y));
        }
    }
}
