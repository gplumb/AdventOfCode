using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EncodingError
{
    class Program
    {
        static void Main(string[] args)
        {
            //var numbers = LoadNumbers("TestData.txt");
            //var windowSize = 5;

            var numbers = LoadNumbers("Input1.txt");
            var windowSize = 25;

            // Puzzle 1
            var number = FindFirstMismatch(numbers, windowSize);
            Console.WriteLine($"First mismatch = {number}");

            // Puzzle 2
            var subset = FindContiguousSubset(numbers, number);
            subset.Sort();
            var sum = subset.First() + subset.Last();
            Console.WriteLine($"Encryption weakness = {sum}");
        }

        static List<long> LoadNumbers(string filename)
        {
            var numbers = new List<long>();
            var reader = default(StreamReader);
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        numbers.Add(long.Parse(input));
                    }

                    return numbers;
                }
            }
            finally
            {
                reader?.Close();
            }
        }

        private static long FindFirstMismatch(List<long> numbers, int windowSize)
        {
            var window = new List<long>();

            // Set up the initial window
            for (int i = 0; i < windowSize; i++)
            {
                window.Add(numbers[i]);
            }

            var matchFound = false;
            var mismatchIndex = -1;

            // Now iterate the remaining data
            for (int i = windowSize; i < numbers.Count; i++)
            {
                foreach (var entry in window)
                {
                    var possible = numbers[i] - entry;

                    if (window.Contains(possible))
                    {
                        matchFound = true;
                        break;
                    }
                }

                if (!matchFound)
                {
                    mismatchIndex = i;
                    break;
                }

                window.RemoveAt(0);
                window.Add(numbers[i]);
                matchFound = false;
            }

            return (mismatchIndex == -1) ? -1 : numbers[mismatchIndex];
        }

        private static List<long> FindContiguousSubset(List<long> numbers, long target)
        {
            var result = new List<long>();
            var total = 0L;

            foreach (var item in numbers)
            {
                total += item;
                result.Add(item);

                if(total > target)
                {
                    do
                    {
                        total -= result[0];
                        result.RemoveAt(0);
                    }
                    while (total > target);
                }

                if(total == target)
                {
                    return result;
                }
            }

            throw new Exception("Contiguous subset not found for target");
        }
    }
}
