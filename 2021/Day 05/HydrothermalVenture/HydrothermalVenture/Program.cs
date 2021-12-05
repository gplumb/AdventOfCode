using System;
using System.Diagnostics;
using System.Linq;

namespace BinaryDiagnostic
{
    public class Program
    {
        static void Main(string[] args)
        {
            // var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            //var result = Puzzle1(data);
            var result = Puzzle2(data);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static int Puzzle1((List<(int x1, int y1, int x2, int y2)> points, int[,] grid) data)
        {
            return ApplyVectors(data, false);
        }

        static int Puzzle2((List<(int x1, int y1, int x2, int y2)> points, int[,] grid) data)
        {
            return ApplyVectors(data, true);
        }

        private static int ApplyVectors((List<(int x1, int y1, int x2, int y2)> points, int[,] grid) data, bool includeDiagonal)
        {
            foreach (var point in data.points)
            {
                // Are we dealing with vertical?
                if (point.x1 == point.x2)
                {
                    // Yes
                    int start = (point.y1 > point.y2) ? point.y2 : point.y1;
                    int end = (point.y1 > point.y2) ? point.y1 : point.y2;

                    for (int y = start; y < end + 1; y++)
                        data.grid[point.x1, y]++;

                    continue;
                }

                // No. Do we have a horizontal?
                if (point.y1 == point.y2)
                {
                    int start = (point.x1 > point.x2) ? point.x2 : point.x1;
                    int end = (point.x1 > point.x2) ? point.x1 : point.x2;

                    for (int x = start; x < end + 1; x++)
                        data.grid[x, point.y1]++;

                    continue;
                }

                // We have a diagonal. Do we skip it?
                if(!includeDiagonal)
                    continue;

                int count = Math.Abs(point.x1 - point.x2);
                int intervalX = (point.x2 - point.x1) / count;
                int intervalY = (point.y2 - point.y1) / count;

                var pX = point.x1;
                var pY = point.y1;

                for (int c = 0; c < count + 1; c++)
                {
                    data.grid[pX, pY]++;

                    pX += intervalX;
                    pY += intervalY;
                }
            }


            // Now count up everything that has 2+ hits
            var total = 0;

            for (int y = 0; y < data.grid.GetLength(1); y++)
            {
                // Console.WriteLine("");

                for (int x = 0; x < data.grid.GetLength(0); x++)
                {
                    // Console.Write((data.grid[x, y] == 0) ? "." : data.grid[x, y].ToString());
                    total += (data.grid[x, y] >= 2) ? 1 : 0;
                }
            }

            // Console.WriteLine("");

            return total;
        }

        static (List<(int x1, int y1, int x2, int y2)> points, int[,] grid) GetDemoData()
        {
            var input = new string[]
            {
                "0,9 -> 5,9",
                "8,0 -> 0,8",
                "9,4 -> 3,4",
                "2,2 -> 2,1",
                "7,0 -> 7,4",
                "6,4 -> 2,0",
                "0,9 -> 2,9",
                "3,4 -> 1,4",
                "0,0 -> 8,8",
                "5,5 -> 8,2",
            };

            return FormatInput(input);
        }

        private static (List<(int x1, int y1, int x2, int y2)> points, int[,] grid) FormatInput(string[] input)
        {
            var points = new List<(int, int, int, int)>();
            var split = new string[] { "->", "," };
            var largest = -1;

            foreach (var item in input)
            {
                var segments = item.Split(split, StringSplitOptions.RemoveEmptyEntries);
                var entry = (x1: int.Parse(segments[0]), y1: int.Parse(segments[1]), x2: int.Parse(segments[2]), y2: int.Parse(segments[3]));

                // Used to try and infer grid size
                largest = (entry.x1 > largest) ? entry.x1 : largest;
                largest = (entry.y1 > largest) ? entry.y1 : largest;
                largest = (entry.x2 > largest) ? entry.x2 : largest;
                largest = (entry.y2 > largest) ? entry.y2 : largest;

                points.Add(entry);
            }

            return (points: points, grid: new int[largest + 1, largest + 1]);
        }

        static (List<(int x1, int y1, int x2, int y2)> points, int[,] grid) LoadFromFile(string filename)
        {
            var reader = default(StreamReader);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    var input = reader.ReadToEnd().Split(Environment.NewLine);
                    return FormatInput(input);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}