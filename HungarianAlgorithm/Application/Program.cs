﻿using Hungarian;

namespace Application
{
    static class Program
    {
        public static void Main(string[] args)
        {
            string pathToInputDirectory = "../../../Input";
            string filename = "2_2.txt";
            ProblemInstance problemInstance = FileReader.ReadInputFile($"{pathToInputDirectory}/{filename}"); // Only for debug purposes
            //ProblemInstance problemInstance = FileReader.ReadInputFile($"{pathToInputDirectory}/{args[0]}"); // To be used in regular approach

            Hungarian.Hungarian algorithm = new Hungarian.Hungarian(problemInstance);
            algorithm.Solve();
        }
    }
}

//int[,] costs = new[,] { { 1 } };
//Hungarian.Hungarian.Solve(costs);