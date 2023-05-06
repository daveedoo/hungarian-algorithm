namespace Application
{
    public class ProgramOptions
    {
        public string InputFilename { get; private set; } = String.Empty;

        public string OutputFilename { get; private set; } = String.Empty;

        public int N { get; private set; } = 2;

        public int K { get; private set; } = 2;

        public bool GenerateFile { get; private set; } = false;

        public bool DisplayLogs { get; private set; } = false;

        public bool WriteSolutionToConsole { get; private set; } = false;

        public ProgramOptions(string filename, int? n = null, int? k = null)
        {
            InputFilename = filename;
            OutputFilename = InputFilename.Replace(".txt", "_output.txt");
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

        public void SetInputFilename(string filename)
        {
            InputFilename = filename;
            if (!InputFilename.EndsWith(".txt"))
            {
                InputFilename += ".txt";
            }
        }

        public void SetOutputFilename(string filename)
        {
            OutputFilename = filename;
            if (!OutputFilename.EndsWith(".txt"))
            {
                OutputFilename += ".txt";
            }
        }

        public void SetDisplayLogs()
        {
            DisplayLogs = true;
        }

        public void SetWriteSolutionToConsole()
        {
            WriteSolutionToConsole = true;
        }

        public void ValidateAndThrow()
        {
            if (String.IsNullOrEmpty(InputFilename))
            {
                throw new ArgumentException("Input filename not specified");
            }

            if (String.IsNullOrEmpty(OutputFilename))
            {
                throw new ArgumentException("Output filename not specified");
            }

            if (N <= 0)
            {
                throw new ArgumentException("N cannot be less or equal to 0");
            }

            if (K <= 0)
            {
                throw new ArgumentException("K cannot be less or equal to 0");
            }
        }

        public void SetNotSpecifiedValues()
        {
            if (String.IsNullOrEmpty(OutputFilename))
            {
                OutputFilename = InputFilename.Replace(".txt", "_output.txt");
            }
        }
    }
}
