namespace Application
{
    public class ProgramOptions
    {
        public bool GenerateFile { get; private set; } = false;
        public string Filename { get; private set; } = String.Empty;
        public int N { get; private set; } = 2;
        public int K { get; private set; } = 2;

        public ProgramOptions(string filename, int? n = null, int? k = null)
        {
            Filename = filename;
            N = n.GetValueOrDefault(N);
            K = k.GetValueOrDefault(K);
            GenerateFile = n.HasValue || k.HasValue;
        }

        public ProgramOptions() { }

        public void SetN(int value)
        {
            N = value;
            GenerateFile = true;
        }

        public void SetK(int value)
        {
            K = value;
            GenerateFile = true;
        }

        public void SetFileName(string filename)
        {
            Filename = filename;
            if (!Filename.EndsWith(".txt"))
            {
                Filename += ".txt";
            }
        }

        public void ValidateAndThrow()
        {
            if (String.IsNullOrEmpty(Filename))
            {
                throw new ArgumentException("Filename not specified");
            }

            if (N <= 0)
            {
                throw new ArgumentException("N cannot be less or equal to 0");
            }

            if (K <= 0)
            {
                throw new ArgumentException("N cannot be less or equal to 0");
            }
        }
    }
}
