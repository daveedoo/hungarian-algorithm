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
    }
}
