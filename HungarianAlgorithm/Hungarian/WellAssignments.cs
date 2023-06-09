﻿namespace Hungarian
{
    public class WellAssignments : IWriteable
    {
        public readonly int WellIndex;

        public readonly IList<(int index, decimal cost)> SuppliedHouses;

        public WellAssignments(int wellIndex, IList<(int index, decimal cost)> suppliedHouses)
        {
            WellIndex = wellIndex;
            SuppliedHouses = suppliedHouses;
        }

        public void Write(TextWriter? textWriter = null)
        {
            textWriter ??= Console.Out; //For some reason cannot be put as parameter

            textWriter.Write($"{WellIndex + 1} -> ");
            foreach(var (index, _) in SuppliedHouses) 
            { 
                textWriter.Write($"{index + 1}, ");
            }

            textWriter.WriteLine();
        }

        public decimal GetTotalAssignmentCost()
        {
            return SuppliedHouses.Select(suppliedHouse => suppliedHouse.cost).Sum();
        }
    }
}
