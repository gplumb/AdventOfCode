using System;
using System.Diagnostics;
using System.Linq;

namespace Lanternfish
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            // var result = Puzzle1(data, 7, 2, 18);        // 26
            // var result = Puzzle1(data, 7, 2, 80);        // 5934
            // var result = Puzzle1(data, 7, 2, 80);        // 351188

            // var result = Puzzle2(data, 7, 2, 18);        // 26
            // var result = Puzzle2(data, 7, 2, 80);        // 5934
            // var result = Puzzle2(data, 7, 2, 80);        // 351188

            var result = Puzzle2(data, 7, 2, 256);          // 1595779846729

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(List<int> data, int rate, int spawnCost, int days)
        {
            for(int x = 0; x < days; x++)
            {
                // Cap count to the current list size
                // (This way we don't break enumeration and can just add new fish to the end)
                var count = data.Count;

                for(var f = 0; f < count; f++)
                {
                    data[f]--;

                    if(data[f] < 0)
                    {
                        data[f] = rate - 1;
                        data.Add(rate + spawnCost - 1);
                    }
                }

                // Console.WriteLine($"day 1: {string.Join(',', data)}");
            }

            return data.Count;
        }

        static long Puzzle2(List<int> data, int rate, int spawnCost, int days)
        {
            // To save space, we track the number of ages instead
            var ages = new Dictionary<int, long>();

            data.ForEach(x => {
                if (ages.ContainsKey(x))
                    ages[x]++;
                else
                    ages[x] = 1;
            });

            for (int d = 0; d < days; d++)
            {
                var newAges = new Dictionary<int, long>();
                
                for(int i =0; i < rate + spawnCost; i++)
                    newAges[i] = 0;

                for (int x = 0; x < ages.Count + 1; x++)
                {
                    if (!ages.ContainsKey(x))
                        continue;

                    if (x == 0)
                    {
                        // Spawn new fish
                        newAges[rate + spawnCost - 1] = ages[x];
                        newAges[rate - 1] += ages[x];
                        continue;
                    }

                    newAges[x - 1] += ages[x];
                }

                ages = newAges;
            }
            
            return ages.Values.Sum();
        }

        static List<int> GetDemoData()
        {
            return "3,4,3,1,2".Split(",").Select(int.Parse).ToList();
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