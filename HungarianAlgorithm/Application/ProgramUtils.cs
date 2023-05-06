using Hungarian;
namespace Application
{
    public static class ProgramUtils
    {
        public static ProblemInstance GenerateProblemInstance(int n, int k, int seed = 0)
        {
            ProblemInstance problemInstance = new ProblemInstance(n, k);
            var rnd = new Random(seed);
            (int x_min, int x_max) = (-100, 100);
            (int y_min, int y_max) = (-100, 100);

            for (int i = 0; i < n; i++)
            {
                var x = rnd.Next(x_min, x_max);
                var y = rnd.Next(y_min, y_max);
                problemInstance.SetWellLocation(i, x, y);
            }

            for (int i = 0; i < k * n; i++)
            {
                var x = rnd.Next(x_min, x_max);
                var y = rnd.Next(y_min, y_max);
                problemInstance.SetHouseLocation(i, x, y);
            }

            return problemInstance;
        }

        public static ProgramOptions ParseInputArgs(string[] args)
        {
            ProgramOptions programOptions = new ProgramOptions();
            var token = 0;

            try
            {
                while (token != args.Length)
                {
                    switch (args[token])
                    {
                        case "-n":
                            ++token;
                            if (token >= args.Length)
                            {
                                throw new ArgumentException("Unexpected end of input arguemnts");
                            }

                            programOptions.SetN(int.Parse(args[token]));
                            break;
                        case "-k":
                            ++token;
                            if (token >= args.Length)
                            {
                                throw new ArgumentException("Unexpected end of input arguemnts");
                            }

                            programOptions.SetK(int.Parse(args[token]));
                            break;
                        case "-o":
                            ++token;
                            if (token >= args.Length)
                            {
                                throw new ArgumentException("Unexpected end of input arguemnts");
                            }

                            programOptions.SetFileName(args[token]);
                            break;
                        default:
                            throw new ArgumentException("Unrecognized input argument");
                    }

                    ++token;
                }
            
                programOptions.ValidateAndThrow();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invalid arguments specified on program entry");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            return programOptions;
        }
    }
}
