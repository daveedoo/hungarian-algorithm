namespace Hungarian
{
    public class Solution
    {
        public readonly IEnumerable<WellAssignments> Assignments;

        public Solution(IEnumerable<WellAssignments> assignments)
        {
            Assignments = assignments;
        }

        public void Write(TextWriter? textWriter = null)
        {
            textWriter ??= Console.Out; //For some reason cannot be put as parameter
            foreach (var assignment in Assignments) 
            {
                assignment.Write(textWriter);
            }

            textWriter.WriteLine($"Sumaryczny koszt: {GetTotalAssignmentCost()}");
        }

        public double GetTotalAssignmentCost()
        {
            return Assignments.Sum(assignment => assignment.GetTotalAssignmentCost());
        }
    }
}
