using System;
using System.Diagnostics;
using System.Linq;

namespace TheTreacheryOfWhales
{
    public class Program
    {
        static void Main(string[] args)
        {
            // var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");
            
            // var result = Puzzle1(data);     // 337488
            var result = Puzzle2(data);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(List<int> data)
        {
            return CalculatePosition(data, false);
        }

        static long Puzzle2(List<int> data)
        {
            return CalculatePosition(data, true);
        }

        static long CalculatePosition(List<int> data, bool incrementalBurn)
        {
            var minValue = int.MaxValue;
            var total = 0;

            for (int i = data.Min(); i < data.Max(); i++)
            {
                total = 0;

                for (int x = 0; x < data.Count; x++)
                    total += incrementalBurn ? CalculateBurnRate(Math.Abs(data[x] - i)) : Math.Abs(data[x] - i);

                if (total < minValue)
                    minValue = total;
            }

            return minValue;
        }

        static int CalculateBurnRate(int target)
        {
            var rate = 0;
            
            for (int i = 1; i < target + 1; i++)
                rate += i;

            return rate;
        }

        static List<int> GetDemoData()
        {
            return "16,1,2,0,4,2,7,1,2,14".Split(",").Select(int.Parse).ToList();
        }

        static List<int> LoadFromFile(string filename)
        {
            var reader = default(StreamReader);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    return reader.ReadToEnd().Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}