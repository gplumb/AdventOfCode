using System;

namespace Dive
{
    public class Program
    {
        static void Main(string[] args)
        {
            // var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");
            
            //var result = Puzzle1(data);
            var result = Puzzle2(data);

            Console.WriteLine($"Position: {result}");
            Console.ReadLine();
        }

        static int Puzzle2(List<(string Direction, int Units)> data)
        {
            var aim = 0;
            var depth = 0;
            var horizontal = 0;

            foreach (var op in data)
            {
                switch (op.Direction)
                {
                    case "forward":
                        horizontal += op.Units;
                        depth += (aim * op.Units);
                        break;

                    case "down":
                        aim += op.Units;
                        break;

                    case "up":
                        aim -= op.Units;
                        break;

                    default:
                        throw new Exception("Unexpected data");
                }
            }

            return depth * horizontal;
        }

        static int Puzzle1(List<(string Direction, int Units)> data)
        {
            var depth = 0;
            var horizontal = 0;

            foreach(var op in data)
            {
                switch (op.Direction)
                {
                    case "forward":
                        horizontal += op.Units;
                        break;

                    case "down":
                        depth += op.Units;
                        break;

                    case "up":
                        depth -= op.Units;
                        break;

                    default:
                        throw new Exception("Unexpected data");
                }
            }

            return depth * horizontal;
        }

        static List<(string Direction, int Units)> GetDemoData()
        {
            return new List<(string Direction, int Units)>()
            {
                (Direction: "forward", Units: 5),
                (Direction: "down", Units: 5),
                (Direction: "forward", Units: 8),
                (Direction: "up", Units: 3),
                (Direction: "down", Units: 8),
                (Direction: "forward", Units: 2)
            };
        }

        static List<(string Direction, int Units)> LoadFromFile(string filename)
        {
            var numbers = new List<int>();
            var reader = default(StreamReader);
            var result = new List<(string Direction, int Units)>();
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        var data = input.Split(" ");
                        result.Add((data[0], int.Parse(data[1])));
                    }

                    return result;
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}