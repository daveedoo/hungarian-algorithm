using Hungarian;
namespace Application
{
    public static class ProgramUtils
    {
        public static ProblemInstance GenerateProblemInstance(int n, int k, int seed = 0)
        {
            ProblemInstance problemInstance = new ProblemInstance(n, k);
            var rnd = new Random(seed);
            (int x_min, int x_max) = (-100, 100);
            (int y_min, int y_max) = (-100, 100);

            for (int i = 0; i < n; i++)
            {
                var x = rnd.Next(x_min, x_max);
                var y = rnd.Next(y_min, y_max);
                problemInstance.SetWellLocation(i, x, y);
            }

            for (int i = 0; i < k * n; i++)
            {
                var x = rnd.Next(x_min, x_max);
                var y = rnd.Next(y_min, y_max);
                problemInstance.SetHouseLocation(i, x, y);
            }

            return problemInstance;
        }
    }
}
