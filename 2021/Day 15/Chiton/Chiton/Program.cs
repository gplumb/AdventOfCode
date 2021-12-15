using System;
using System.Diagnostics;
using System.Linq;

namespace Chiton
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            //var result = Puzzle1(data);
            var result = Puzzle2(data);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(int[,] grid)
        {
            return CalculateRisk(grid, false);
        }

        static long Puzzle2(int[,] grid)
        {
            return CalculateRisk(grid, true);
        }

        private static long CalculateRisk(int[,] grid, bool expandMap)
        {
            var maxX = expandMap ? grid.GetLength(0) * 5 : grid.GetLength(0);
            var maxY = expandMap ? grid.GetLength(1) * 5 :  grid.GetLength(1);
            var maxSourceX = grid.GetLength(0);
            var maxSourceY = grid.GetLength(1);

            var cheapest = new int[maxX, maxY];

            for (int y = 0; y < maxY; y++)
                for (int x = 0; x < maxX; x++)
                    cheapest[x, y] = int.MaxValue;

            // Don't count the starting risk
            cheapest[0, 0] = 0;

            bool finished = false;
            var newRisk = -1;

            while (!finished)
            {
                finished = true;

                for (var y = 0; y < maxY; y++)
                {
                    for (var x = 0; x < maxX; x++)
                    {
                        var currentRisk = cheapest[x, y];
                        var neighbours = new List<(int x, int y)>();

                        if (x + 1 < maxX)
                            neighbours.Add((x + 1, y));

                        if (x - 1 >= 0)
                            neighbours.Add((x - 1, y));

                        if (y + 1 < maxY)
                            neighbours.Add((x, y + 1));

                        if (y - 1 >= 0)
                            neighbours.Add((x, y - 1));

                        foreach (var entry in neighbours)
                        {
                            if(expandMap)
                            {
                                newRisk = currentRisk + WrapRisk(entry.x, entry.y, maxSourceX, maxSourceY, grid);
                            }
                            else
                            {
                                newRisk = currentRisk + grid[entry.x, entry.y];
                            }

                            if (cheapest[entry.x, entry.y] > newRisk)
                            {
                                finished = false;
                                cheapest[entry.x, entry.y] = newRisk;
                            }
                        }
                    }
                }
            }

            return cheapest[maxX - 1, maxY - 1];
        }

        static int WrapRisk(int x, int y, int maxSourceX, int maxSourceY, int[,] grid)
        {
            if (x < maxSourceX && y < maxSourceY)
                return grid[x, y];

            int risk = 0;
            
            if (y >= maxSourceY)
                risk = WrapRisk(x, y - maxSourceY, maxSourceX, maxSourceX, grid) + 1;
            
            if (x >= maxSourceX)
                risk = WrapRisk(x - maxSourceX, y, maxSourceX, maxSourceX, grid) + 1;

            risk = risk % 10;
            risk = (risk == 0) ? 1 : risk;
            return risk;
        }

        static int[,] GetDemoData()
        {
            var data = new List<string>()
            {
                "1163751742",
                "1381373672",
                "2136511328",
                "3694931569",
                "7463417111",
                "1319128137",
                "1359912421",
                "3125421639",
                "1293138521",
                "2311944581",
            };

            return ParseGrid(data);
        }

        static int[,] ParseGrid(List<string> data)
        {
            var maxX = data[0].Length;
            var maxY = data.Count;

            var result = new int[maxX, maxY];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    result[x, y] = (int)char.GetNumericValue(data[y][x]);
                }
            }

            return result;
        }

        static int[,] LoadFromFile(string filename)
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

                    return ParseGrid(data);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}