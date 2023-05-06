using Hungarian;

namespace Application
{
    static class Program
    {
        public static void Main(string[] args)
        {
            string pathToInputDirectory = "../../../Input";
            string pathToOutputDirectory = "../../../Output";
            int seed = 420;

            ProgramOptions options = ProgramUtils.ParseInputArgs(args);

            if (options.GenerateFile)
            {
                FileWriter.GenerateInputFile($"{pathToInputDirectory}/{options.Filename}", options.N, options.K, seed);
            }

            ProblemInstance problemInstance = FileReader.ReadInputFile($"{pathToInputDirectory}/{options.Filename}");

            Hungarian.Hungarian algorithm = new Hungarian.Hungarian(problemInstance);
            Solution solution = algorithm.Solve();

            FileWriter.WriteToOutputFile($"{pathToOutputDirectory}/{options.Filename}", solution);
        }
    }
}
