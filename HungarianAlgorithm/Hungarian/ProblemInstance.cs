namespace Hungarian
{
    public class ProblemInstance
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
    }
}
