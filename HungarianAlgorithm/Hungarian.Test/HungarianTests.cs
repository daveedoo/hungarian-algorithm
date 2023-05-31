using Application;
using Hungarian.Algorithms;
using System.Runtime.Serialization;
using Xunit.Sdk;

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

            var hungarianAlgorithm = new HungarianAlgorithm(problem);
            var solution = hungarianAlgorithm.Solve(problem.CreateDistancesDecimalMatrix());

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

            var hungarianAlgorithm = new HungarianAlgorithm(problem);
            var solution = hungarianAlgorithm.Solve(problem.CreateDistancesDecimalMatrix());

            solution.TotalAssignmentCost.ShouldBe(12.0m);
        }

        [Theory]
        [InlineData("2_1.txt", "147.8314224171732")]
        [InlineData("2_2.txt", "12")]
        [InlineData("2_3.txt", "708.5182971138222")]
        [InlineData("2_4.txt", "928.9661686596875")]
        [InlineData("3_1.txt", "192.8913838340144")]
        [InlineData("3_2.txt", "485.3161664078569")]
        [InlineData("3_3.txt", "808.9674158743229")]
        [InlineData("3_4.txt", "1048.7972750709457")]
        [InlineData("4_1.txt", "477.7720637438752")]
        [InlineData("5_1.txt", "478.9020930443910")]
        [InlineData("5_2.txt", "944.7345361548154")]
        [InlineData("10_1.txt", "494.47088338464058")]
        public void FromFile(string filename, string expectedCost)
        {
            decimal cost = Convert.ToDecimal(expectedCost); // decimal cannot be passed as InlineData
            var problem = FileReader.ReadInputFile($"../../../../Application/Input/{filename}");

            var hungarianAlgorithm = new HungarianAlgorithm(problem);
            var solution = hungarianAlgorithm.Solve(problem.CreateDistancesDecimalMatrix());

            solution.TotalAssignmentCost.ShouldBe(cost);
        }
    }
}
