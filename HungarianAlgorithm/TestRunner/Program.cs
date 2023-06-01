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
            int[] seeds = { 420, 9999, 12345 };
            int numberOfRuns = 3;

            (int N, IEnumerable<int> Ks)[] tests = new (int N, IEnumerable<int> Ks)[]
            {
                (500, new int[]{2}),
            };

            //(int N, IEnumerable<int> Ks)[] tests = new (int N, IEnumerable<int> Ks)[]
            //{
            //    (1, Enumerable.Range(1, 10)),
            //    (2, Enumerable.Range(1, 10)),
            //    (3, Enumerable.Range(1, 10)),
            //    (4, Enumerable.Range(1, 10)),
            //    (5, Enumerable.Range(1, 10)),
            //    (6, Enumerable.Range(1, 10)),
            //    (7, Enumerable.Range(1, 10)),
            //    (8, Enumerable.Range(1, 10)),
            //    (9, Enumerable.Range(1, 10)),
            //    (10, Enumerable.Range(1, 10)),
            //    (20, Enumerable.Range(1, 10)),
            //    (50, Enumerable.Range(1, 4)),
            //    (100, Enumerable.Range(1, 4)),
            //    (200, Enumerable.Range(1, 4)),
            //    (500, Enumerable.Range(1, 4)),
            //    (1000, Enumerable.Range(1, 2)),
            //};
            int totalTestsCount = tests.Sum(test => test.Ks.Count());

            int testIndex = 1;
            foreach (var test in tests)
            {
                foreach (var K in test.Ks)
                {
                    int run = 0;
                    Console.WriteLine($"Executing test ({testIndex}/{totalTestsCount}): N: {test.N}, K: {K}");
                    while (run++ < numberOfRuns)
                    {
                        Console.WriteLine($"Run {run}:");
                        ExecuteTestAndGetAlgorithmsExecutionTimes(test.N, K, run, seeds[run-1]);
                    }
                    testIndex++;
                }
            }

            //Computation time (N=100, K=4): 1 minute for three runs
        }

        private static (TimeSpan hungarianExecutionTime, TimeSpan libraryExecutionTime, bool isAssignmentCostDifferent) ExecuteTestAndGetAlgorithmsExecutionTimes(int N, int K, int run, int seed)
        {
            string inputFileName = $"../../../../Application/Input/{N}_{K}_{run}.txt";
            string outputFileName = inputFileName.Replace("Input", "Output");

            ProblemInstance problemInstance = ProblemInstanceGenerator.GenerateProblemInstance(N, K, seed);
            FileWriter.WriteToOutputFile(inputFileName, problemInstance);

            IAlgorithm hungarian = new HungarianAlgorithm(problemInstance);
            IAlgorithm library = new LibraryAlgorithm(problemInstance);

            var distances = problemInstance.CreateDistancesIntMatrix();
            //var distances = problemInstance.CreateDistancesDecimalMatrix();

            var timer = new Stopwatch();
            timer.Start();
            Solution hungarian_solution = hungarian.Solve(distances);
            timer.Stop();
            TimeSpan hungarianExecutionTime = timer.Elapsed;
            Console.WriteLine("Hungarian: " + hungarianExecutionTime.ToString(@"m\:ss\.fff"));

            timer = new Stopwatch();
            timer.Start();
            Solution library_solution = library.Solve(distances);
            timer.Stop();
            TimeSpan libraryExecutionTime = timer.Elapsed;
            Console.WriteLine("Library: " + libraryExecutionTime.ToString(@"m\:ss\.fff"));

            FileWriter.WriteToOutputFile(outputFileName.Replace(".txt", "_h.txt"), hungarian_solution);
            FileWriter.WriteToOutputFile(outputFileName.Replace(".txt", "_l.txt"), library_solution);

            return (hungarianExecutionTime, libraryExecutionTime, 3 != library_solution.TotalAssignmentCost);
        }
    }
}