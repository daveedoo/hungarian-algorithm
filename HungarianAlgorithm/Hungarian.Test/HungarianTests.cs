namespace Hungarian.Test
{
    public class HungarianTests
    {
        [Fact]
        public void OneHouseAndWell()
        {
            var problem = new ProblemInstance(1, 1);
            problem.SetWellLocation(0, 0.0, 0.0);
            problem.SetHouseLocation(0, 0.0, 1.0);

            var solution = new Hungarian(problem).Solve();

            solution.Assignments.ShouldHaveSingleItem();
            solution.Assignments.First().WellIndex.ShouldBe(0);
            solution.Assignments.First().SuppliedHouses.ShouldHaveSingleItem();
            solution.Assignments.First().SuppliedHouses[0].index.ShouldBe(0);
            solution.Assignments.First().SuppliedHouses[0].cost.ShouldBe(1.0m);
        }

        [Fact]
        public void Symmetric_N2_K2()
        {
            var problem = new ProblemInstance(2, 2);
            problem.SetWellLocation(0, 2.0, 0.0);
            problem.SetWellLocation(1, 6.0, 0.0);
            problem.SetHouseLocation(0, 2.0, 3.0);
            problem.SetHouseLocation(1, 6.0, -3.0);
            problem.SetHouseLocation(2, 2.0, -3.0);
            problem.SetHouseLocation(3, 6.0, 3.0);

            var solution = new Hungarian(problem).Solve();

            solution.GetTotalAssignmentCost().ShouldBe(12.0m);
        }
    }
}
