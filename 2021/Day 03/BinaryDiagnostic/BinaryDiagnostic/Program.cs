using System;

namespace BinaryDiagnostic
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

        static int Puzzle2(List<string> data)
        {
            var oxygenGenerator = GetRate(data, true);  // 23
            var carbonDioxideScrubber = GetRate(data, false);  // 10

            return oxygenGenerator * carbonDioxideScrubber;
        }

        static int GetRate(List<string> data, bool msb)
        {
            var cols = data[0].Length;
            var contents = new List<string>(data);

            var totalOn = 0;
            var totalOff = 0;

            for (int c = 0; c < cols; c++)
            {
                totalOn = 0;
                totalOff = 0;

                foreach(var item in contents)
                {
                    var set = item[c] == '1';
                    totalOn += (set) ? 1 : 0;
                    totalOff += (set) ? 0 : 1;
                }

                var keepOnes = (totalOn == totalOff) ? (msb ? true : false) : (msb ? (totalOn > totalOff) : (totalOn < totalOff));
                var newData = new List<string>();

                foreach (var item in contents)
                {
                    var set = item[c] == '1';

                    if(set && keepOnes)
                    {
                        newData.Add(item);
                        continue;
                    }

                    if (!set && !keepOnes)
                    {
                        newData.Add(item);
                        continue;
                    }
                }

                if(newData.Count == 1)
                {
                    return Convert.ToInt32(newData[0], 2);
                }
                
                contents = newData;
            }

            return -1;
        }

        static int Puzzle1(List<string> data)
        {
            var cols = data[0].Length;
            var totalOn = 0;
            var totalOff = 0;

            var gamma = 0;
            var epsilon = 0;

            for (int c = 0; c < cols; c++)
            {
                totalOn = 0;
                totalOff = 0;

                gamma = gamma << 1;
                epsilon = epsilon << 1;

                foreach (var item in data)
                {
                    var set = item[c] == '1';
                    totalOn += (set) ? 1 : 0;
                    totalOff += (set) ? 0 : 1;
                }

                if (totalOn > totalOff)
                {
                    gamma |= 1 << 0;
                    epsilon &= ~(1 << 0);
                }
                else
                {
                    epsilon |= 1 << 0;
                    gamma &= ~(1 << 0);
                }
            }

            return epsilon * gamma;
        }

        static List<string> GetDemoData()
        {
            return new List<string>()
            {
                "00100",
                "11110",
                "10110",
                "10111",
                "10101",
                "01111",
                "00111",
                "11100",
                "10000",
                "11001",
                "00010",
                "01010",
            };
        }

        static List<string> LoadFromFile(string filename)
        {
            var numbers = new List<int>();
            var reader = default(StreamReader);
            var result = new List<string>();
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        result.Add(input);
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