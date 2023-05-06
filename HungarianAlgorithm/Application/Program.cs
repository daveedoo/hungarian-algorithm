using Hungarian;

namespace Application
{
    static class Program
    {
        public static void Main(string[] args)
        {
            ProgramOptions options = ProgramUtils.ParseInputArgs(args);

            ProblemInstance problemInstance;
            if (options.GenerateFile)
            {
                problemInstance = ProblemInstanceGenerator.GenerateProblemInstance(options.N, options.K);
                FileWriter.WriteToOutputFile(options.Filename, problemInstance);
            }
            else
            {
                problemInstance = FileReader.ReadInputFile(options.Filename);
            }

            Hungarian.Hungarian algorithm = new Hungarian.Hungarian(problemInstance);
            Solution solution = algorithm.Solve();

            string outputFileName = options.Filename.Replace(".txt", "_output.txt");
            FileWriter.WriteToOutputFile(outputFileName, solution);
        }
    }
}
