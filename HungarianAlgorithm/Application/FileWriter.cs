using Hungarian;

namespace Application
{
    public static class FileWriter
    {
        public static void WriteToOutputFile(string path, Solution solution)
        {
            Console.WriteLine("Saving optimal solution to file...");

            try
            {
                using (var writer = new StreamWriter(path))
                {
                    solution.Write(writer);
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            Console.WriteLine("Optimal solution saved.");
        }
        
        public static void GenerateInputFile(string path, int n, int k, int seed = 0)
        {
            Console.WriteLine("Generating new file...");

            try
            {
                ProblemInstance problemInstance = ProgramUtils.GenerateProblemInstance(n, k, seed);
                using (var writer = new StreamWriter(path))
                {
                    problemInstance.Write(writer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            Console.WriteLine("File has been generated.");
            Console.WriteLine();
        }
    }
}
