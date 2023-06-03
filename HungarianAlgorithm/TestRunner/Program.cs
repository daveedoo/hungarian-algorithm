using Application;
using Hungarian;
using Hungarian.Algorithms;
using System.Diagnostics;
using System.IO;

namespace TestRunner
{
    static class Program
    {
        public static void Main(string[] args)
        {

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = customCulture;

            string testResultsFilePath = "../../../../Application/Output/TestResults.txt";

            (int N, IEnumerable<int> Ks, IEnumerable<int> seeds)[] tests = new (int N, IEnumerable<int> Ks, IEnumerable<int> seeds)[]
            {
                (1, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (2, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (3, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (4, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (5, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (6, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (7, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (8, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (9, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (10, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (20, Enumerable.Range(1, 10), Enumerable.Range(1, 10)),
                (50, Enumerable.Range(1, 5), Enumerable.Range(1, 10)),
                (100, Enumerable.Range(1, 4), Enumerable.Range(1, 5)),
                (200, Enumerable.Range(1, 3), Enumerable.Range(1, 5)),
                (500, Enumerable.Range(1, 2), Enumerable.Range(1, 5)),
                (1000, Enumerable.Range(1, 1), Enumerable.Range(1, 5)),
            };
            int totalTestsCount = tests.Sum(test => test.Ks.Count());

            int testIndex = 1;
            var timer = new Stopwatch();
            timer.Start();
            foreach (var test in tests)
            {
                foreach (var K in test.Ks)
                {
                    int run = 0;
                    Console.WriteLine($"Executing test ({testIndex}/{totalTestsCount}): N: {test.N}, K: {K}");
                    TimeSpan totalExecutionTimeForTest = TimeSpan.Zero;
                    bool solutionsMatch = true;
                    foreach (int seed in test.seeds)
                    {
                        run++;
                        Console.WriteLine($"Run {run}:");
                        (TimeSpan hungarianExecutionTime, TimeSpan libraryExecutionTime, bool doesAssignmentCostMatch) = ExecuteTestAndGetAlgorithmsExecutionTimes(test.N, K, run, seed);
                        totalExecutionTimeForTest += hungarianExecutionTime;
                        solutionsMatch &= doesAssignmentCostMatch;
                    }

                    using (StreamWriter sw = File.AppendText(testResultsFilePath))
                    {
                        sw.WriteLine($"{test.N}, {K}, {(totalExecutionTimeForTest / test.seeds.Count()).TotalSeconds}, {solutionsMatch}");
                    }

                    testIndex++;
                }
            }
            timer.Stop();
            TimeSpan totalTestsExeutionTime = timer.Elapsed;

            Console.WriteLine();
            Console.WriteLine($"Total tests execution time: {totalTestsExeutionTime.ToString(@"hh\:mm\:ss\.fff")}");
        }

        private static (TimeSpan hungarianExecutionTime, TimeSpan libraryExecutionTime, bool doesAssignmentCostMatch) ExecuteTestAndGetAlgorithmsExecutionTimes(int N, int K, int run, int seed)
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
            Solution hungarianSolution = hungarian.Solve(distances);
            timer.Stop();
            TimeSpan hungarianExecutionTime = timer.Elapsed;
            Console.WriteLine("Hungarian: " + hungarianExecutionTime.ToString(@"hh\:mm\:ss\.fff"));

            timer = new Stopwatch();
            timer.Start();
            Solution librarySolution = library.Solve(distances);
            timer.Stop();
            TimeSpan libraryExecutionTime = timer.Elapsed;
            Console.WriteLine("Library: " + libraryExecutionTime.ToString(@"hh\:mm\:ss\.fff"));

            FileWriter.WriteToOutputFile(outputFileName.Replace(".txt", "_h.txt"), hungarianSolution);
            FileWriter.WriteToOutputFile(outputFileName.Replace(".txt", "_l.txt"), librarySolution);

            return (hungarianExecutionTime, libraryExecutionTime, hungarianSolution.TotalAssignmentCost == librarySolution.TotalAssignmentCost);
        }
    }
}