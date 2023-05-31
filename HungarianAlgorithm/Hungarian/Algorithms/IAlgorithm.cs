namespace Hungarian.Algorithms
{
    public interface IAlgorithm
    {
        Solution Solve(decimal[,] _distances);

        Solution Solve(int[,] _distances);
    }
}