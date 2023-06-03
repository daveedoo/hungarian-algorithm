namespace Hungarian
{
    public class Solution : IWriteable
    {
        public readonly IEnumerable<WellAssignments> Assignments;

        public readonly decimal TotalAssignmentCost;

        public Solution(IEnumerable<WellAssignments> assignments)
        {
            Assignments = assignments;
            TotalAssignmentCost = assignments.Sum(assignment => assignment.GetTotalAssignmentCost());
        }

        public void Write(TextWriter? textWriter = null)
        {
            textWriter ??= Console.Out; //For some reason cannot be put as parameter
            foreach (var assignment in Assignments) 
            {
                assignment.Write(textWriter);
            }

            textWriter.WriteLine($"Sumaryczny koszt: {TotalAssignmentCost}");
        }
    }
}
