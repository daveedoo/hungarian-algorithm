namespace Application
{
    public static class ProgramUtils
    {
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
                        case "-i":
                            ++token;
                            if (token >= args.Length)
                            {
                                throw new ArgumentException("Unexpected end of input arguemnts");
                            }

                            programOptions.SetInputFilename(args[token]);
                            break;
                        case "-o":
                            ++token;
                            if (token >= args.Length)
                            {
                                throw new ArgumentException("Unexpected end of input arguemnts");
                            }

                            programOptions.SetOutputFilename(args[token]);
                            break;
                        case "-v":
                            programOptions.SetDisplayLogs();
                            break;
                        case "-s":
                            programOptions.SetWriteSolutionToConsole();
                            break;
                        default:
                            throw new ArgumentException("Unrecognized input argument");
                    }

                    ++token;
                }

                programOptions.SetNotSpecifiedValues();
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
