using Hungarian;

namespace Application
{
    public static class FileReader
    {
        private static int _lineNo = 0;

        public static ProblemInstance ReadInputFile(string path)
        {
            ProblemInstance problemInstance = null;

            try
            {
                using (var reader = new StreamReader(path))
                {
                    _lineNo = 0;

                    string line = reader.ReadLineWrapped();
                    int n = int.Parse(line);
                    if (n < 1)
                    {
                        throw new InvalidInputFileFormatException(_lineNo, $"Invalid n value. Expected int value >= 1. Provided value: {n}");
                    }

                    line = reader.ReadLineWrapped();
                    int k = int.Parse(line);
                    if (k < 1)
                    {
                        throw new InvalidInputFileFormatException(_lineNo, $"Invalid k value. Expected int value >= 1. Provided value: {k}");
                    }

                    problemInstance = new ProblemInstance(n, k);

                    bool[] vertexLocationSpecified = new bool[n];
                    for (int i = 0; i < n; i++)
                    {
                        (int index, double x, double y) = reader.ReadLineWithVertexLocation();

                        if (index >= n || index < 0)
                        {
                            throw new InvalidInputFileFormatException(_lineNo, $"Invalid index for well vertex. Specified index: {index + 1}. Valid range: [1, {n}].");
                        }

                        if (vertexLocationSpecified[index])
                        {
                            throw new InvalidInputFileFormatException(_lineNo, $"Location of the well vertex with index {index + 1} already specified.");
                        }
                        vertexLocationSpecified[index] = true;

                        problemInstance.SetWellLocation(index, x, y);
                    }

                    vertexLocationSpecified = new bool[n * k];
                    for (int i = 0; i < n * k; i++)
                    {
                        (int index, double x, double y) = reader.ReadLineWithVertexLocation();

                        if (index >= n * k || index < 0)
                        {
                            throw new InvalidInputFileFormatException(_lineNo, $"Invalid index for house vertex. Specified index: {index + 1}. Valid range: [1, {n * k}].");
                        }

                        if (vertexLocationSpecified[index])
                        {
                            throw new InvalidInputFileFormatException(_lineNo, $"Location of the house vertex with index {index + 1} already specified.");
                        }
                        vertexLocationSpecified[index] = true;

                        problemInstance.SetHouseLocation(index, x, y);
                    }
                }
            }
            catch (InvalidInputFileFormatException e)
            {
                Console.WriteLine($"Invalid input file format at line: {e.LineNo}.");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            return problemInstance;
        }

        private static string ReadLineWrapped(this StreamReader reader)
        {
            _lineNo++;

            string line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                throw new InvalidInputFileFormatException(_lineNo, "Empty line");
            }

            return line;
        }

        private static (int index, double x, double y) ParseLineWithVertexLocation(string line)
        {
            var splittedLine = line.Split(' ');
            if (splittedLine.Length != 3)
            {
                throw new InvalidInputFileFormatException(_lineNo, $"Could not parse line with vertex location. Line: {line}");
            }

            if (!int.TryParse(splittedLine[0], out int index))
            {
                throw new InvalidInputFileFormatException(_lineNo, $"Could not parse vertex index. Line: {line}");
            }

            if (!double.TryParse(splittedLine[1], out double x))
            {
                throw new InvalidInputFileFormatException(_lineNo, $"Could not parse vertex x coordinate. Line: {line}");
            }

            if (!double.TryParse(splittedLine[2], out double y))
            {
                throw new InvalidInputFileFormatException(_lineNo, $"Could not parse vertex y coordinate. Line: {line}");
            }

            return (index - 1, x, y);
        }

        private static (int index, double x, double y) ReadLineWithVertexLocation(this StreamReader reader)
        {
            string line = reader.ReadLineWrapped();
            return ParseLineWithVertexLocation(line);
        }
    }
}
