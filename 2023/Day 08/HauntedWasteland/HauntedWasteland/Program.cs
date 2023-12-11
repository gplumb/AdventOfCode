using System.Collections;
using System.Runtime.CompilerServices;

namespace HauntedWasteland
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
            //var data = GetTestData2();
            var data = LoadFromFile("Input1.txt");
            var network = new Network(data);
            var steps = 0L;
            var step = "AAA";

            foreach (char item in network)
            {
                // Console.WriteLine(step);

                if (step == "ZZZ")
                    break;
                
                var direction = network.Nodes[step];
                step = (item == 'L') ? step = direction.Item1 : direction.Item2;
                steps++;
            }

            return steps;
        }

        static char[] SPLIT_CHARS = new char[] { '=', ' ', '(', ')', ','};

        public class Network : IEnumerable
        {
            public string pattern = "";

            public Dictionary<string, (string, string)> Nodes = new();


            public Network(List<string> documents)
            {
                pattern = documents[0];

                foreach (var item in documents[2..])
                {
                    var components = item.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
                    Nodes.Add(components[0], (components[1], components[2]));
                }
            }

            public IEnumerator GetEnumerator()
            {
                int index = -1;

                while (true)
                {
                    index++;
                    
                    if (index == pattern.Length)
                        index = 0;

                    yield return pattern[index];
                }
            }
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "RL",
                "",
                "AAA = (BBB, CCC)",
                "BBB = (DDD, EEE)",
                "CCC = (ZZZ, GGG)",
                "DDD = (DDD, DDD)",
                "EEE = (EEE, EEE)",
                "GGG = (GGG, GGG)",
                "ZZZ = (ZZZ, ZZZ)"
            };
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData2()
        {
            return new List<string>()
            {
                "LLR",
                "",
                "AAA = (BBB, BBB)",
                "BBB = (AAA, ZZZ)",
                "ZZZ = (ZZZ, ZZZ)",
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
