namespace Application
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
            HousesLocations = new (double x, double y)[n*k];
        }

        public void AddWell(int index, double x, double y)
        {
            WellsLocations[index] = (x, y);
        }

        public void AddHouse(int index, double x, double y) 
        {
            HousesLocations[index] = (x, y);
        }
    }
}
