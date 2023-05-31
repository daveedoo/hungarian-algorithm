using Hungarian;
using Hungarian.Algorithms;
using System.Diagnostics;

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
            IAlgorithm hungarian = new Hungarian.Algorithms.Hungarian(problemInstance);
            IAlgorithm brute = new Hungarian.Algorithms.BruteForceAlgorithm(problemInstance);

            var timer = new Stopwatch();
            timer.Start();
            Solution hungarian_solution = hungarian.Solve();
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            Console.WriteLine("Hungarian: " + timeTaken.ToString(@"m\:ss\.fff"));

            timer = new Stopwatch();
            timer.Start();
            Solution brute_solution_algorithm = brute.Solve();
            timer.Stop();
            timeTaken = timer.Elapsed;
            Console.WriteLine("Brute force: " + timeTaken.ToString(@"m\:ss\.fff")); //n*k = 12 takes about 15 minutes

            options.LogStateToConsole("Staring computations..");
            options.LogStateToConsole("Solution found.");

            if (options.WriteSolutionToConsole)
            {
                hungarian_solution.Write();
            }

            options.LogStateToConsole("Saving solution to output file..");

            FileWriter.WriteToOutputFile(options.OutputFilename.Replace(".txt", "_h.txt"), hungarian_solution);
            FileWriter.WriteToOutputFile(options.OutputFilename.Replace(".txt", "_b.txt"), brute_solution_algorithm);
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
