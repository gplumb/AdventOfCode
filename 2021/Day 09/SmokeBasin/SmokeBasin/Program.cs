using System;
using System.Diagnostics;
using System.Linq;

namespace SmokeBasin
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

        static int Puzzle1(int[,] heightMap)
        {
            var minima = FindLowPoints(heightMap);

            // Now sum up the totals
            var result = 0;

            foreach (var entry in minima)
                result += (entry.n + 1);

            return result;
        }

        static int Puzzle2(int[,] heightMap)
        {
            var maxX = heightMap.GetLength(0);
            var maxY = heightMap.GetLength(1);

            var minima = FindLowPoints(heightMap);
            var basins = new List<int>();

            foreach(var (startX, startY, startN) in minima)
            {
                var basin = new List<int>();

                var seen = new HashSet<Location>();
                var work = new Stack<Location>();
                var current = -1;
                var target = default(Location);

                work.Push(new Location() { x = startX, y = startY });

                while (work.Count > 0)
                {
                    var entry = work.Pop();
                    current = heightMap[entry.x, entry.y];

                    if (seen.Contains(entry))
                        continue;

                    seen.Add(entry);

                    if (current == 9)
                        continue;

                    // Only add this entry if we haven't seen it already
                    basin.Add(current);

                    // Left
                    if (entry.x - 1 >= 0)
                        work.Push(new Location() { x = entry.x - 1, y = entry.y });

                    // Right
                    if (entry.x + 1 < maxX)
                        work.Push(new Location() { x = entry.x + 1, y = entry.y });

                    // Up
                    if (entry.y - 1 >= 0)
                        work.Push(new Location() { x = entry.x, y = entry.y - 1 });

                    // Down
                    if (entry.y + 1 < maxY)
                        work.Push(new Location() { x = entry.x, y = entry.y + 1 });
                }

                basins.Add(basin.Count);
            }

            var items = basins.Select(x => x).OrderByDescending(x => x).Take(3).ToList();
            return items[0] * items[1] * items[2];
        }

        struct Location
        {
            public int x;
            public int y;
        }

        private static List<(int x, int y, int n)> FindLowPoints(int[,] heightMap)
        {
            var minima = new List<(int x, int y, int n)>();
            var maxX = heightMap.GetLength(0);
            var maxY = heightMap.GetLength(1);

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    var number = heightMap[x, y];

                    // Get neighbours (if any)
                    var left = (x - 1 >= 0) ? heightMap[x - 1, y] : -1;
                    var right = (x + 1 < maxX) ? heightMap[x + 1, y] : -1;
                    var top = (y - 1 >= 0) ? heightMap[x, y - 1] : -1;
                    var bottom = (y + 1 < maxY) ? heightMap[x, y + 1] : -1;

                    // If the numbers are lower, we don't have a minima, so we can continue
                    if (left > -1 && left <= number)
                        continue;

                    if (right > -1 && right <= number)
                        continue;

                    if (top > -1 && top <= number)
                        continue;

                    if (bottom > -1 && bottom <= number)
                        continue;

                    minima.Add((x: x, y: y, n: number));
                }
            }

            return minima;
        }

        static int[,] GetDemoData()
        {
            var data = new List<string>() { "2199943210", "3987894921", "9856789892", "8767896789", "9899965678" };
            return ParseHeightMap(data);
        }

        static int[,] ParseHeightMap(List<string> data)
        {
            var maxY = data.Count;
            var maxX = data[0].Length;
            var result = new int[maxX, maxY];

            for(int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    result[x, y] = (int)Char.GetNumericValue(data[y][x]);
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

                    return ParseHeightMap(data);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}