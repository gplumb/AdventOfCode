using System;

namespace WaitForIt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(PartOne());
            Console.WriteLine("Hello, World!");
        }

        static double PartOne()
        {
            //var data = GetTestData1();
            var data = LoadFromFile("Input1.txt");
            var times = data[0].Replace("Time:", "").Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var records = data[1].Replace("Distance:", "").Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var result = 1f;

            for (long x = 0; x < times.Length; x++)
            {
                var time = long.Parse(times[x]);
                var record = long.Parse(records[x]);
                var wins = 0;

                for (int i = 0; i <= time; i++)
                {
                    var distance = i * (time - i);
                    wins += (distance > record) ? 1 : 0;
                }

                result *= wins;
            }

            return result;
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "Time:      7  15   30",
                "Distance:  9  40  200",
            };
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> LoadFromFile(string filename)
        {
            var reader = default(StreamReader);
            var data = new List<string>();
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                        data.Add(input);

                    return data;
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
