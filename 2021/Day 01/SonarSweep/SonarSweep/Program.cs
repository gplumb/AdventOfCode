using System;

namespace SonarSweep
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            
            //var data = LoadFromFile("Input1.txt");
            //var result = Puzzle1(data);

            var data = LoadFromFile("Input2.txt");
            var result = Puzzle2(data);

            Console.WriteLine($"Increases: {result}");
            Console.ReadLine();
        }

        static int Puzzle2(int[] data)
        {
            var count = 0;

            // Count the number of times a depth measurement increases in a sliding window
            for (int x = 3; x < data.Length; x++)
            {
                var itemOne = data[x - 1] + data[x - 2] + data[x - 3];
                var itemTwo = data[x] + data[x - 1] + data[x - 2];

                if (itemTwo > itemOne)
                    count++;
            }

            return count;
        }

        static int Puzzle1(int[] data)
        {
            var count = 0;

            // Count the number of times a depth measurement increases
            for (int x = 1; x < data.Length; x++)
            {
                if (data[x] > data[x - 1])
                    count++;
            }

            return count;
        }

        static int[] GetDemoData()
        {
            return new int[] { 199, 200, 208, 210, 200, 207, 240, 269, 260, 263 };
        }

        static int[] LoadFromFile(string filename)
        {
            var numbers = new List<int>();
            var reader = default(StreamReader);
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                        numbers.Add(int.Parse(input));

                    return numbers.ToArray();
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}