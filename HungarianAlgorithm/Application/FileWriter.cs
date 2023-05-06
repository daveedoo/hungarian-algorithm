using Hungarian;

namespace Application
{
    public static class FileWriter
    {
        public static void WriteToOutputFile(string path, Solution solution)
        {
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
        }
        
        public static void GenerateInputFile(string path, int n, int k, int seed = 0)
        {
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
            Console.Error.WriteLine("File has been generated");
        }
    }
}
