using Application;
using Hungarian;
using Hungarian.Algorithms;
using System.Diagnostics;

namespace TestRunner
{
    static class Program
    {
        public static void Main(string[] args)
        {
            int N = 5;
            int K = 5;
            string inputFileName = $"../../../../Application/Input/{N}_{K}.txt";
            string outputFileName = inputFileName.Replace("Input", "Output");

            ProblemInstance problemInstance = ProblemInstanceGenerator.GenerateProblemInstance(N, K);
            FileWriter.WriteToOutputFile(inputFileName, problemInstance);

            IAlgorithm hungarian = new HungarianAlgorithm(problemInstance);
            IAlgorithm library = new LibraryAlgorithm(problemInstance);

            var distances = problemInstance.CreateDistancesIntMatrix();
            //var distances = problemInstance.CreateDistancesDecimalMatrix();

            var timer = new Stopwatch();
            timer.Start();
            Solution hungarian_solution = hungarian.Solve(distances);
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            Console.WriteLine("Hungarian: " + timeTaken.ToString(@"m\:ss\.fff"));

            timer = new Stopwatch();
            timer.Start();
            Solution library_solution = library.Solve(distances);
            timer.Stop();
            timeTaken = timer.Elapsed;
            Console.WriteLine("Library: " + timeTaken.ToString(@"m\:ss\.fff"));

            FileWriter.WriteToOutputFile(outputFileName.Replace(".txt", "_h.txt"), hungarian_solution);
            FileWriter.WriteToOutputFile(outputFileName.Replace(".txt", "_l.txt"), library_solution);
        }
    }
}