using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TicketTranslation
{
    public class Rule
    {
        public string Name { get; set; }

        public (int, int) LowerRange { get; set; }

        public (int, int) UpperRange { get; set; }

        public bool IsValid(int number)
        {
            return ((number >= LowerRange.Item1 && number <= LowerRange.Item2) ||
                    (number >= UpperRange.Item1 && number <= UpperRange.Item2));
        }
    }

    public class TicketInfo
    {
        public List<Rule> Rules { get; set; } = new List<Rule>();
        public List<int> YourTicket { get; set; } = new List<int>();

        public List<List<int>> NearbyTickets { get; set; } = new List<List<int>>();
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Puzzle 1
            //var info = LoadTicketInfo("TestData1.txt");
            //var info = LoadTicketInfo("Input1.txt");
            //var result = FindScanningErrorRate(info);
            //Console.WriteLine($"Scanning error rate = {result}");

            // Puzzle 2
            //var info = LoadTicketInfo("TestData2.txt");
            var info = LoadTicketInfo("Input1.txt");
            _ = FindScanningErrorRate(info);

            var order = InferOrderFromOthers(info);

            //Console.WriteLine($"{info.YourTicket[0]} = {order[0]}");
            //Console.WriteLine($"{info.YourTicket[1]} = {order[1]}");
            //Console.WriteLine($"{info.YourTicket[2]} = {order[2]}");

            var total = 1L;
            for (int x = 0; x < order.Count; x++)
            {
                if(order[x].StartsWith("departure", StringComparison.OrdinalIgnoreCase))
                {
                    // Console.WriteLine("x");
                    total *= info.YourTicket[x];
                }

                // Console.Write($"{info.YourTicket[x]} : {order[x]}");
                // Console.WriteLine();
            }

            Console.WriteLine($"Total = {total}");
        }

        private static long FindScanningErrorRate(TicketInfo info)
        {
            var invalid = new List<long>();
            var discard = new List<int>();

            for(int x = 0; x < info.NearbyTickets.Count; x++)
            {
                var nearby = info.NearbyTickets[x];
                var bad = false;

                foreach(var entry in nearby)
                {
                    var invalidCount = 0;

                    // Check this number against all the rules
                    foreach (var rule in info.Rules)
                    {
                        // Is this number invalid for the current rule?
                        if(!rule.IsValid(entry))
                            invalidCount++;
                    }

                    // Is this number invalid for every rule?
                    if (invalidCount == info.Rules.Count)
                    {
                        invalid.Add(entry);
                        bad = true;
                    }
                }

                if (bad)
                {
                    discard.Add(x);
                }
            }

            discard.Reverse();

            for (int x = 0; x < discard.Count; x++)
                info.NearbyTickets.RemoveAt(discard[x]);

            return invalid.Sum();
        }

        private static List<string> InferOrderFromOthers(TicketInfo info)
        {
            var possible = new Dictionary<int, HashSet<Rule>>();
            var matched = new SortedDictionary<int, string>();

            for (var position = 0; position < info.NearbyTickets[0].Count; position++)
            {
                possible[position] = info.Rules.ToHashSet();

                foreach (var t in info.NearbyTickets)
                {
                    foreach (var r in info.Rules)
                    {
                        if (!r.IsValid(t[position]))
                        {
                            possible[position].Remove(r);
                        }
                    }
                }
            }

            while (possible.Count > 0)
            {
                var match = possible.First(kvp => kvp.Value.Count == 1);
                var rule = match.Value.Single();

                matched.Add(match.Key, rule.Name);
                possible.Remove(match.Key);

                foreach (var p in possible)
                {
                    p.Value.Remove(rule);
                }
            }

            return matched.Values.ToList();
        }

        static TicketInfo LoadTicketInfo(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);
            var result = new TicketInfo();

            try
            {
                using (reader = new StreamReader(filename))
                {
                    // Read the rules
                    while ((input = reader.ReadLine()).Length > 0)
                    {
                        var name = Regex.Match(input, "([a-zA-Z ]*)(?=:)").Value;
                        var ranges = Regex.Matches(input, "[0-9]*").Where(x => x.Value.Length > 0).Select(x => x.Value).ToList();

                        result.Rules.Add(new Rule()
                        {
                            Name = name,
                            LowerRange = (int.Parse(ranges[0]), int.Parse(ranges[1])),
                            UpperRange = (int.Parse(ranges[2]), int.Parse(ranges[3]))
                        });
                    }


                    // Read your ticket
                    input = reader.ReadLine();
                    if (!input.Equals("your ticket:", StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Unexpected input!");

                    var numbers = reader.ReadLine().Split(",");
                    for(int x = 0; x < numbers.Length; x++)
                    {
                        result.YourTicket.Add(int.Parse(numbers[x]));
                    }

                    // Read nearby tickets
                    input = reader.ReadLine();
                    input = reader.ReadLine();
                    if (!input.Equals("nearby tickets:", StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Unexpected input!");

                    while ((input = reader.ReadLine()) != null)
                    {
                        var newList = new List<int>();
                        numbers = input.Split(",");

                        for (int x = 0; x < numbers.Length; x++)
                        {
                            newList.Add(int.Parse(numbers[x]));
                        }

                        result.NearbyTickets.Add(newList);
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
