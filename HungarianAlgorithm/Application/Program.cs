
namespace Application
{
    static class Program
    {
        public static void Main(string[] args)
        {
            ProblemInstance problemInstance = FileReader.ReadInputFile(args[0]); //Not yet tested
            // TODO:
            // 1. Add conversion of problem instance to graph
            // 2. Add calculation of weights
            // 3. Add extending set represneting wells
        }
    }
}

//int[,] costs = new[,] { { 1 } };
//Hungarian.Hungarian.Solve(costs);
