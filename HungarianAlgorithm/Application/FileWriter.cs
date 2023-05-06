using Hungarian;

namespace Application
{
    public static class FileWriter
    {
        public static void WriteToOutputFile(string path, IWriteable writeable)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    writeable.Write(writer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }
    }
}
