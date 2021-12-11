using System;
using System.Diagnostics;
using System.Linq;

namespace DumboOctopus
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            //var result = Puzzle1(data, 100);
            var result = Puzzle2(data);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(int[,] grid, int steps)
        {
            var totalFlashes = 0;
            var maxY = grid.GetLength(1);
            var maxX = grid.GetLength(0);

            for (int c = 0; c < steps; c++)
            {
                // The squares to process per step
                var stack = new Stack<(int x, int y)>();
                var flashed = new List<(int x, int y)>();

                for (int y = 0; y < maxY; y++)
                    for (int x = 0; x < maxX; x++)
                        stack.Push((x: x, y: y));

                // Apply the flashes to the grid for this iteration
                while (stack.Count > 0)
                {
                    var coord = stack.Pop();
                    var number = grid[coord.x, coord.y];

                    // We've already flashed, so do nothing for this iteration
                    if(flashed.Contains((x: coord.x, y: coord.y)))
                        continue;

                    // Are we about to flash?
                    if (number == 9)
                    {
                        // Yup!
                        totalFlashes++;
                        grid[coord.x, coord.y] = 0;
                        flashed.Add((x: coord.x, y: coord.y));

                        // Now add it's immediate neighbours to the stack
                        for (int ny = -1; ny <= 1; ny++)
                        {
                            for (int nx = -1; nx <=1; nx++)
                            {
                                int newX = coord.x + nx;
                                int newY = coord.y + ny;

                                if (newX == coord.x && newY == coord.y)
                                    continue;

                                if (newX < 0 || newX >= maxX)
                                    continue;

                                if (newY < 0 || newY >= maxY)
                                    continue;

                                stack.Push((x: newX, y: newY));
                            }
                        }

                        continue;
                    }

                    // Get that energy up!
                    grid[coord.x, coord.y]++;
                }

                /*
                // Debugging
                Console.WriteLine();
                Console.WriteLine($"Step {c + 1}:");
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    for (int x = 0; x < grid.GetLength(0); x++)
                    {
                        Console.Write(grid[x, y]);
                    }

                    Console.WriteLine();
                }
                */
            }

            return totalFlashes;
        }


        static long Puzzle2(int[,] grid)
        {
            var maxY = grid.GetLength(1);
            var maxX = grid.GetLength(0);
            var total = maxX * maxY;
            var c = 0;

            var stack = new Stack<(int x, int y)>();
            var flashed = new List<(int x, int y)>();

            while (flashed.Count < total)
            {
                c++;
                flashed.Clear();

                // The squares to process per step
                for (int y = 0; y < maxY; y++)
                    for (int x = 0; x < maxX; x++)
                        stack.Push((x: x, y: y));

                // Apply the flashes to the grid for this iteration
                while (stack.Count > 0)
                {
                    var coord = stack.Pop();
                    var number = grid[coord.x, coord.y];

                    // We've already flashed, so do nothing for this iteration
                    if (flashed.Contains((x: coord.x, y: coord.y)))
                        continue;

                    // Are we about to flash?
                    if (number == 9)
                    {
                        // Yup!
                        grid[coord.x, coord.y] = 0;
                        flashed.Add((x: coord.x, y: coord.y));

                        // Now add it's immediate neighbours to the stack
                        for (int ny = -1; ny <= 1; ny++)
                        {
                            for (int nx = -1; nx <= 1; nx++)
                            {
                                int newX = coord.x + nx;
                                int newY = coord.y + ny;

                                if (newX == coord.x && newY == coord.y)
                                    continue;

                                if (newX < 0 || newX >= maxX)
                                    continue;

                                if (newY < 0 || newY >= maxY)
                                    continue;

                                stack.Push((x: newX, y: newY));
                            }
                        }

                        continue;
                    }

                    // Get that energy up!
                    grid[coord.x, coord.y]++;
                }
            }

            return c;
        }

        static int[,] GetDemoData()
        {
            var data = new List<string>()
            {
                "5483143223",
                "2745854711",
                "5264556173",
                "6141336146",
                "6357385478",
                "4167524645",
                "2176841721",
                "6882881134",
                "4846848554",
                "5283751526"
            };

            return GetGrid(data);
        }


        static int[,] GetGrid(List<string> data)
        {
            var result = new int[data.Count, data.Count];

            for(int y = 0; y < data.Count; y++)
            {
                for(int x = 0; x < data[y].Length; x++)
                {
                    result[x, y] = (int)Char.GetNumericValue(data[y][x]);
                }
            }

            return result;
        }

        static int [,] LoadFromFile(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);
            var data = new List<string>();

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                        data.Add(input);

                    return GetGrid(data);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}