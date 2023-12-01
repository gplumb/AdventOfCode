using System.Collections.Generic;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Trebuchet
{
    internal class Program
    {
        static Regex Numbers = new Regex("[0-9]", RegexOptions.Compiled);

        static Dictionary<string, int> FirstNumber = new Dictionary<string, int>()
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
        };

        static Dictionary<string, int> LastNumber = new Dictionary<string, int>()
        {
            { "eno", 1 },
            { "owt", 2 },
            { "eerht", 3 },
            { "ruof", 4 },
            { "evif", 5 },
            { "xis", 6 },
            { "neves", 7 },
            { "thgie", 8 },
            { "enin", 9 },
        };


        /// <summary>
        /// Main
        /// </summary>
        static void Main(string[] args)
        {
            // Console.WriteLine(PartOne());
            Console.WriteLine(PartTwo());
            Console.ReadLine();
        }


        static int PartOne()
        {
            // var input = GetTestData();
            var input = LoadFromFile("input1.txt");
            var total = 0;

            foreach (var line in input)
            {
                var number = 0;
                var matches = Numbers.Matches(line);

                if (matches.Count == 0)
                    throw new Exception("Bad input");

                if (matches.Count == 1)
                {
                    var digit = int.Parse(matches[0].Value);
                    number = (digit * 10) + digit;
                }
                else
                {
                    number = (int.Parse(matches[0].Value) * 10) + int.Parse(matches[^1].Value);
                }

                total += number;
            }

            return total;
        }


        static int PartTwo()
        {
            // var input = GetTestData2();
            var input = LoadFromFile("input1.txt");
            var total = 0;

            foreach (var line in input)
            {
                var number = 0;
                var first = 0;
                var last = 0;

                // Find first number
                first = FindNumber(line, FirstNumber);

                // Find second number
                last = FindNumber(Reverse(line), LastNumber);
                last = (last == -1) ? first : last;

                number = (first * 10) + last;
                total += number;
            }

            return total;
        }


        /// <summary>
        /// Reverse the given string
        /// </summary>
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        /// <summary>
        /// Match the number in the given map
        /// </summary>
        private static int FindNumber(string input, Dictionary<string, int> dictionary)
        {
            // Check for spelt numbers first
            for (int x = 0; x < input.Length; x++)
            {
                if (char.IsNumber(input[x]))
                    return int.Parse("" + input[x]);

                if (TryGetDigit(input, dictionary, x, 3, out var number))
                    return number;

                if (TryGetDigit(input, dictionary, x, 4, out number))
                    return number;

                if (TryGetDigit(input, dictionary, x, 5, out number))
                    return number;
            }

            return -1;
        }


        /// <summary>
        /// Try to find a number or known text string representing a number
        /// </summary>
        static bool TryGetDigit(string input, Dictionary<string, int> dictionary, int start, int length, out int number)
        {
            if (start + length > input.Length)
            {
                number = -1;
                return false;
            }

            var key = input[start..(start + length)];

            // See if we match a known dictionary pattern
            if (dictionary.ContainsKey(key))
            {
                number = dictionary[key];
                return true;
            }

            number = -1;
            return false;
        }

        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "1abc2",
                "pqr3stu8vwx",
                "a1b2c3d4e5f",
                "treb7uchet"
            };
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData2()
        {
            return new List<string>()
            {
                "two1nine",
                "eightwothree",
                "abcone2threexyz",
                "xtwone3four",
                "4nineeightseven2",
                "zoneight234",
                "7pqrstsixteen"
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