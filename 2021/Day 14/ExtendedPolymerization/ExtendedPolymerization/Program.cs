using System;
using System.Diagnostics;
using System.Linq;

namespace ExtendedPolymerization
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            //var result = Puzzle1(data, 10);
            var result = Puzzle2(data, 40);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(Polymer data, int iterations)
        {
            var letters = new Dictionary<char, long>();

            // I *really* hope they don't want the actual string data in part 2!
            var stack = new Stack<string>();
            var next = new Stack<string>();
            var count = 0L;

            // Let's build up an initial set of pairs from the target data
            for(int x = 0; x < data.Template.Length - 1; x++)
            {
                var pair = data.Template[x..(x + 2)];
                stack.Push(pair);

                // Count the first letter of each pair (we'll cover them all in this loop)...
                count = letters.ContainsKey(data.Template[x]) ? letters[data.Template[x]] : 0;
                letters[data.Template[x]] = (count + 1);
            }

            // ...except for the last one :)
            count = letters.ContainsKey(data.Template[data.Template.Length - 1]) ? letters[data.Template[data.Template.Length - 1]] : 0;
            letters[data.Template[data.Template.Length - 1]] = (count + 1);

            for (int i = 0; i < iterations; i++)
            {
                // Now do the work!
                while (stack.Count > 0)
                {
                    var pair = stack.Pop();

                    // Apply the rules to get the new letter
                    if (data.Rules.ContainsKey(pair))
                    {
                        var target = data.Rules[pair];

                        // Increment the letter count
                        count = letters.ContainsKey(target) ? letters[target] : 0;
                        letters[target] = (count + 1);

                        // Create the new pairs...
                        var pairOne = $"{pair[0]}{target}";
                        var pairTwo = $"{target}{pair[1]}";

                        // ...and push them to the next iteration
                        next.Push(pairOne);
                        next.Push(pairTwo);
                    }
                }

                // We're done with this iteration
                stack = next;
                next = new Stack<string>();
            }

            var mostCommon = letters.Max(x => x.Value);
            var leastCommon = letters.Min(x => x.Value);

            return mostCommon - leastCommon;
        }

        static long Puzzle2(Polymer data, int iterations)
        {
            var pairs = new Dictionary<string, long>();
            var letters = new Dictionary<char, long>();
            var count = 0L;

            for (int x = 0; x < data.Template.Length - 1; x++)
            {
                var pair = data.Template[x..(x + 2)];
                count = pairs.ContainsKey(pair) ? pairs[pair] : 0;
                pairs[pair] = (count + 1);

                // Count the first letter of each pair (we'll cover them all in this loop)...
                count = letters.ContainsKey(data.Template[x]) ? letters[data.Template[x]] : 0;
                letters[data.Template[x]] = (count + 1);
            }

            // ...except for the last one :)
            count = letters.ContainsKey(data.Template[data.Template.Length - 1]) ? letters[data.Template[data.Template.Length - 1]] : 0;
            letters[data.Template[data.Template.Length - 1]] = (count + 1);

            var target = '\0';
            var pairOne = "";
            var pairTwo = "";

            for (int i = 0; i < iterations; i++)
            {
                var newPairs = new Dictionary<string, long>();

                foreach (var pair in pairs)
                {
                    if (!data.Rules.ContainsKey(pair.Key))
                        continue;

                    target = data.Rules[pair.Key];
                    pairOne = $"{pair.Key[0]}{target}";
                    pairTwo = $"{target}{pair.Key[1]}";

                    count = newPairs.ContainsKey(pairOne) ? newPairs[pairOne] : 0;
                    newPairs[pairOne] = count + pair.Value;

                    count = newPairs.ContainsKey(pairTwo) ? newPairs[pairTwo] : 0;
                    newPairs[pairTwo] = count + pair.Value;

                    count = letters.ContainsKey(target) ? letters[target] : 0;
                    letters[target] = count + pair.Value;
                }

                pairs = newPairs;
            }

            var mostCommon = letters.Max(x => x.Value);
            var leastCommon = letters.Min(x => x.Value);
            return mostCommon - leastCommon;
        }

 
        static Polymer GetDemoData()
        {
            var data = new List<string>()
            {
                "NNCB",
                "",
                "CH -> B",
                "HH -> N",
                "CB -> H",
                "NH -> C",
                "HB -> C",
                "HC -> B",
                "HN -> C",
                "NN -> C",
                "BH -> H",
                "NC -> B",
                "NB -> B",
                "BN -> B",
                "BB -> N",
                "BC -> B",
                "CC -> N",
                "CN -> C",
            };

            return ParsePolymer(data);
        }

        static Polymer ParsePolymer(List<string> data)
        {
            var result = new Polymer() { Template = data[0] };

            for(int x = 2; x < data.Count; x++)
            {
                var split = data[x].Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
                result.Rules.Add(split[0], split[1][0]);
            }

            return result;
        }

        public class Polymer
        {
            public string Template { get; set; } = "";

            public Dictionary<string, char> Rules { get; set; } = new Dictionary<string, char>();
        }

        static Polymer LoadFromFile(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);
            var data = new List<string>();

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                        data.Add(input);

                    return ParsePolymer(data);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}