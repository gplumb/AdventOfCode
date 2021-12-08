using System;
using System.Diagnostics;
using System.Linq;

namespace SevenSegmentSearch
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            // Part 2: Simple example
            // var data = ParseInput(new List<string>() { "acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf" })[0];
            // var result = Decode(data);

            //var result = Puzzle1(data);
            var result = Puzzle2(data);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(List<(string[] Signals, string[] Output)> data)
        {
            // Count unique digit instances
            var total = 0;

            foreach(var entry in data)
            {
                foreach(var digit in entry.Output)
                {
                    switch(digit.Length)
                    {
                        case 2:
                        case 3:
                        case 4:
                        case 7:
                            total++;
                            break;
                    }
                }
            }

            return total;
        }

        static long Puzzle2(List<(string[] Signals, string[] Output)> data)
        {
            var total = 0;

            foreach (var entry in data)
                total += Decode(entry);

            return total;
        }

        static int Decode((string[] Signals, string[] Output) entry)
        {
            // Here we store a map of bits and the current candidate numbers
            var remaining = new List<string>(entry.Signals);
            var known = new Dictionary<int, string>();
            var map = new Dictionary<string, int>();

            // First off, populate known items
            MapKnown(remaining, known);

            // The remaining numbers are 2, 3, 5 (which have 5 segments) and 0, 9, 6 (which have 6 segments)
            // By drawing out the segments for every digit and comparing the known ones to each remaining group
            // entry, it's possible to infer the second group's values based on segment omission from known values
            // and then the first one's by segment overlap
            //
            //  Ie. We know we've found a 6 if a segment from '1' is not present in it
            //      We know we've found a 0 if a segment from '4' is not present in it
            //      The 9 is discovered by removing the other candidates from this group
            //
            //      We know we've found a 5 if all segments from '6' overlap it
            //      We know we've found a 3 if all segments from '9' overlap it
            //      The 2 is discovered by removing the other candidates from this group

            var sixSegments = remaining.Where(x => x.Length == 6).Select(x => x).ToList();
            var fiveSegments = remaining.Where(x => x.Length == 5).Select(x => x).ToList();

            // Find 6
            var match = sixSegments.First(x => IsSegmentOmitted(x, known[1]));
            sixSegments.Remove(match);
            known[6] = match;

            // Find 0
            match = sixSegments.First(x => IsSegmentOmitted(x, known[4]));
            sixSegments.Remove(match);
            known[0] = match;

            // Infer 9
            known[9] = sixSegments[0];

            // Find 5
            match = fiveSegments.First(x => IsSegmentOverlapped(x, known[6]));
            fiveSegments.Remove(match);
            known[5] = match;

            // Find 3
            match = fiveSegments.First(x => IsSegmentOverlapped(x, known[9]));
            fiveSegments.Remove(match);
            known[3] = match;

            // Infer 2
            known[2] = fiveSegments[0];

            // Build a map with sorted characters values (to normalize out random order of outputs)
            foreach (var pair in known)
                map.Add(String.Concat(pair.Value.OrderBy(c => c)), pair.Key);

            // Now we can decode!
            int result = 0;
            result += (map[String.Concat(entry.Output[0].OrderBy(c => c))] * 1000);
            result += (map[String.Concat(entry.Output[1].OrderBy(c => c))] * 100);
            result += (map[String.Concat(entry.Output[2].OrderBy(c => c))] * 10);
            result += map[String.Concat(entry.Output[3].OrderBy(c => c))];

            return result;
        }

        static bool IsSegmentOmitted(string candidate, string known)
        {
            foreach (var ch in known)
            {
                if (!candidate.Contains(ch))
                    return true;
            }

            return false;
        }


        static bool IsSegmentOverlapped(string candidate, string known)
        {
            foreach(var ch in candidate)
            {
                if (!known.Contains(ch))
                    return false;
            }

            return true;
        }

        static void MapKnown(List<string> remaining, Dictionary<int, string> known)
        {
            for (int i = 0; i < remaining.Count; i++)
            {
                var current = remaining[i];
                var remove = false;

                if (!remove)
                {
                    switch (current.Length)
                    {
                        case 2:
                            known[1] = current;
                            remove = true;
                            break;

                        case 3:
                            known[7] = current;
                            remove = true;
                            break;

                        case 4:
                            known[4] = current;
                            remove = true;
                            break;

                        case 7:
                            known[8] = current;
                            remove = true;
                            break;
                    }
                }

                if (remove)
                {
                    remaining.RemoveAt(i);
                    i--;
                }
            }
        }

        static void AddCandidate(Dictionary<char, List<int>> dict, char ch, int num)
        {
            var list = dict[ch];

            if (!list.Contains(num))
                list.Add(num);
        }

        static List<(string[] Signals, string[] Output)> GetDemoData()
        {
            var data = new List<string>()
            {
                "be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe",
                "edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc",
                "fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg",
                "fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb",
                "aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea",
                "fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb",
                "dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe",
                "bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef",
                "egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb",
                "gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce"
            };

            return ParseInput(data);
        }

        static List<(string[] Signals, string[] Output)> ParseInput(List<string> data)
        {
            var result = new List<(string[] Signals, string[] Output)>();

            foreach (var entry in data)
            {
                var input = entry.Split('|', StringSplitOptions.RemoveEmptyEntries);
                result.Add((Signals: input[0].Split(" ", StringSplitOptions.RemoveEmptyEntries), Output: input[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)));
            }

            return result;
        }

        static List<(string[] Signals, string[] Output)> LoadFromFile(string filename)
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

                    return ParseInput(data);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}