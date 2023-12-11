namespace MirageMaintenance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(PartOne());  
            Console.ReadLine();
        }


        static long PartOne()
        {
            // var data = GetTestData1();
            var data = LoadFromFile("Input1.txt");
            var sum = 0L;

            foreach (var item in data)
            {
                var numbers = item.Split(' ').Select(int.Parse).ToList();
                sum += Solve(numbers);
            }

            return sum;
        }


        static long Solve(List<int> input)
        {
            // Have we reached the lowest level?
            if (input.All(i => i == 0))
                return 0;

            var differences = new List<int>();

            for (int i = 0; i < input.Count - 1; i++)
                differences.Add(input[i + 1] - input[i]);

            return input.Last() + Solve(differences);
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "0 3 6 9 12 15",
                "1 3 6 10 15 21",
                "10 13 16 21 30 45"
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
