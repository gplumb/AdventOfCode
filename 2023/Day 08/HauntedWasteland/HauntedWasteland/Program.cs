using System.Collections;
using System.Runtime.CompilerServices;

namespace HauntedWasteland
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine(PartOne());
            Console.WriteLine(PartTwo());
            Console.ReadLine();
        }


        static long PartTwo()
        {
            // Can't brute force this one (it'll take FOREVER)
            // Based on previous years, this looks like one for LCM...
            // var data = GetTestData3();
            var data = LoadFromFile("Input1.txt");
            var network = new Network(data);
            var nodes = new List<string>();
            var count = 0L;

            // We will track every time a node ends in z for each of the inputs
            // and multiply outwards to get our large number
            var cycles = new List<long>();

            // Find our starting nodes and stash them
            foreach (var kvp in network.Nodes)
            {
                if (kvp.Key.EndsWith("A"))
                    nodes.Add(kvp.Key);
            }

            // Find the first 'Z' for each of our starting nodes
            foreach (var start in nodes)
            {
                var step = start;
                count = 0L;

                foreach (char item in network)
                {
                    if (step.EndsWith("Z"))
                        break;

                    var direction = network.Nodes[step];
                    step = (item == 'L') ? step = direction.Item1 : direction.Item2;
                    count++;
                }

                cycles.Add(count);
            }

            // Now get the largest common multiple
            var result = cycles[0];

            for (int i = 1; i < cycles.Count; i++)
            {
                result = LowestCommonMultiple(result, cycles[i]);
            }

            return result;
        }


        static long LowestCommonMultiple(long x, long y)
        {
            return x * y / GreatestCommonDivisor(x, y);
        }


        static long GreatestCommonDivisor(long x, long y)
        {
            while (y != 0)
            {
                var t = y;
                y = x % y;
                x = t;
            }

            return x;
        }


        static long PartOne()
        {
            // var data = GetTestData1();
            //var data = GetTestData2();
            var data = LoadFromFile("Input1.txt");
            var network = new Network(data);
            var count = 0L;
            var step = "AAA";

            foreach (char item in network)
            {
                // Console.WriteLine(step);

                if (step == "ZZZ")
                    break;
                
                var direction = network.Nodes[step];
                step = (item == 'L') ? step = direction.Item1 : direction.Item2;
                count++;
            }

            return count;
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
        static List<string> GetTestData3()
        {
            return new List<string>()
            {
                "LR",
                "",
                "11A = (11B, XXX)",
                "11B = (XXX, 11Z)",
                "11Z = (11B, XXX)",
                "22A = (22B, XXX)",
                "22B = (22C, 22C)",
                "22C = (22Z, 22Z)",
                "22Z = (22B, 22B)",
                "XXX = (XXX, XXX)"
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
