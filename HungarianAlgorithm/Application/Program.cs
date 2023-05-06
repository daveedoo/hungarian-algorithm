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
                options.LogStateToConsole("Generating problem instance..");
                problemInstance = ProblemInstanceGenerator.GenerateProblemInstance(options.N, options.K);
                options.LogStateToConsole("Saving problem instance to file..");
                FileWriter.WriteToOutputFile(options.InputFilename, problemInstance);
                options.LogStateToConsole("Problem instance saved to file.");
            }
            else
            {
                options.LogStateToConsole("Reading problem instance from file..");
                problemInstance = FileReader.ReadInputFile(options.InputFilename);
                options.LogStateToConsole("Problem instance lodaded.");
            }

            options.LogStateToConsole("Initializing algorithm..");
            Hungarian.Hungarian algorithm = new Hungarian.Hungarian(problemInstance);
            options.LogStateToConsole("Staring computations..");
            Solution solution = algorithm.Solve();
            options.LogStateToConsole("Solution found.");

            if (options.WriteSolutionToConsole)
            {
                solution.Write();
            }

            options.LogStateToConsole("Saving solution to output file..");
            FileWriter.WriteToOutputFile(options.OutputFilename, solution);
            options.LogStateToConsole("Solution saved to file.");
        }

        public static void LogStateToConsole(this ProgramOptions programOptions, string message)
        {
            if (programOptions.DisplayLogs)
            {
                Console.WriteLine(message);
            }
        }
    }
}
