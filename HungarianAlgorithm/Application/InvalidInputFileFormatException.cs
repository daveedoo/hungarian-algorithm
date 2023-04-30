namespace Application
{
    public class InvalidInputFileFormatException : Exception
    {
        public readonly int LineNo;

        public InvalidInputFileFormatException(int lineNo, string message) : base(message)
        {
            LineNo = lineNo;
        }
    }
}
