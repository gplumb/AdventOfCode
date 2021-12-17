using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PacketDecoder
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

        static (int x1, int x2, int y1, int y2) GetDemoData()
        {
            return ParseTarget("target area: x = 20..30, y = -10..-5");
        }

        static (int x1, int x2, int y1, int y2) ParseTarget(string data)
        {
            var parts = data.Replace("target area: x", "").Replace("=", "").Replace("y", "").Split(",", StringSplitOptions.RemoveEmptyEntries);
            var xCoords = parts[0].Split('.', StringSplitOptions.RemoveEmptyEntries);
            var yCoords = parts[1].Split('.', StringSplitOptions.RemoveEmptyEntries);

            return (int.Parse(xCoords[0]), int.Parse(xCoords[1]), int.Parse(yCoords[0]), int.Parse(yCoords[1]));
        }

        static long Puzzle1((int x1, int x2, int y1, int y2) target)
        {
            int ty = target.y1 + 1;
            return (-ty * (-ty + 1)) / 2;
        }

        static long Puzzle2((int x1, int x2, int y1, int y2) target)
        {
            // Trajectory of motion formula: https://physicscatalyst.com/article/what-is-trajectory/

            var count = 0L;
            int maxt = Math.Max(-2 * target.y1 + 1, target.x2);

            for (int vyo = target.y1; vyo <= -target.y1; vyo++)
            {
                for (int vxo = 1; vxo <= target.x2; vxo++)
                {
                    for (int t = 1; t <= maxt; t++)
                    {
                        int y = vyo * t - t * (t - 1) / 2;
                        int x = (t < vxo) ? (vxo * t - t * (t - 1) / 2) : (vxo * (vxo + 1) / 2);

                        if (target.y1 <= y && y <= target.y2 && target.x1 <= x && x <= target.x2)
                        {
                            count++;
                            break;
                        }
                    }
                }
            }

            return count;
        }

        static (int x1, int x2, int y1, int y2) LoadFromFile(string filename)
        {
            var reader = default(StreamReader);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    return ParseTarget(reader.ReadToEnd());
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}