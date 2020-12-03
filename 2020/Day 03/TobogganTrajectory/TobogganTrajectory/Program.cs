using System;
using System.IO;

namespace TobogganTrajectory
{
    class Program
    {
        private static string[] input = new string[]
        {
            "..##.......",
            "#...#...#..",
            ".#....#..#.",
            "..#.#...#.#",
            ".#...##..#.",
            "..#.##.....",
            ".#.#.#....#",
            ".#........#",
            "#.##...#...",
            "#...##....#",
            ".#..#...#.#"
        };


        static void Main(string[] args)
        {
            var data = ReadInput();
            var grid = BuildGrid(data);

            // Puzzle 1
            var treeCount = CountTrees(grid, 0, 0, 3, 1);
            Console.WriteLine($"Tree count = {treeCount}");

            // Puzzle 2
            var slope1 = CountTrees(grid, 0, 0, 1, 1);
            var slope2 = CountTrees(grid, 0, 0, 3, 1);
            var slope3 = CountTrees(grid, 0, 0, 5, 1);
            var slope4 = CountTrees(grid, 0, 0, 7, 1);
            var slope5 = CountTrees(grid, 0, 0, 1, 2);

            long multiple = (1l * slope1) * slope2 * slope3 * slope4 * slope5;
            Console.WriteLine($"All slopes multiple = {multiple}");
        }


        private static int CountTrees(char[,] grid, int startX, int startY, int stepX, int stepY)
        {
            var x = startX;
            var y = startY;

            var maxX = grid.GetLength(0);
            var treeCount = 0;

            // y moves down the grid each iteration
            for (; y < grid.GetLength(1); y += stepY)
            {
                // Evaluate our position and update the tree count
                treeCount += (grid[x, y] == '#') ? 1 : 0;

                // Move "x" along 3 places, being sure to account for horizontal wrapping
                x = (x + stepX) % maxX;
            }

            return treeCount;
        }


        /// <summary>
        /// Does exactly what it says on the tin
        /// </summary>
        static string[] ReadInput()
        {
            // return input;

            StreamReader reader = null;

            try
            {
                using (reader = new StreamReader("Input1.txt"))
                {
                    return reader.ReadToEnd().Split(Environment.NewLine);
                }
            }
            finally
            {
                reader?.Close();
            }
        }


        /// <summary>
        /// Builds a grid from the given input
        /// </summary>
        /// <remarks>
        /// Assumes uniform input
        /// </remarks>
        private static char[,] BuildGrid(string[] input)
        {
            var result = new char[input[0].Length, input.Length];

            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[0].Length; x++)
                {
                    result[x, y] = input[y][x];
                }
            }

            return result;
        }
    }
}
