using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdapterArray
{
    class Adapters
    {
        public int[] DistanceTotals { get; set; }

        public long JoltRating { get; set; }

        public Adapters(long distance)
        {
            DistanceTotals = new int[distance];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Example 1 (Part 1)
            //var numbers = LoadNumbers("TestData1.txt");
            //var adapters = FindAdapters(numbers, 3, 3);

            // Example 2 (Part 1)
            //var numbers = LoadNumbers("TestData2.txt");
            //var adapters = FindAdapters(numbers, 3, 3);

            // Problem 1
            //var numbers = LoadNumbers("Input1.txt");
            //var adapters = FindAdapters(numbers, 3, 3);
            //var answer = adapters.DistanceTotals[0] * adapters.DistanceTotals[2];
            //Console.WriteLine($"Total = {adapters.JoltRating}. Total d1 = {adapters.DistanceTotals[0]}. Total d3 = {adapters.DistanceTotals[2]}");
            //Console.WriteLine($"Answer = {answer}");

            // Example 1 (Part 2)
            //var numbers = LoadNumbers("TestData1.txt");
            //var permutations = CountPossiblePermutations(numbers, 3);

            // Example 2 (Part 2)
            //var numbers = LoadNumbers("TestData2.txt");
            //var permutations = CountPossiblePermutations(numbers, 3);

            // Problem 2
            var numbers = LoadNumbers("Input1.txt");
            var permutations = CountPossiblePermutations(numbers, 3);

            Console.WriteLine($"Permutations = {permutations}");
        }

        static List<int> LoadNumbers(string filename)
        {
            var numbers = new List<int>();
            var reader = default(StreamReader);
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        numbers.Add(int.Parse(input));
                    }

                    return numbers;
                }
            }
            finally
            {
                reader?.Close();
            }
        }

        private static Adapters FindAdapters(List<int> numbers, int maxDistance, int joltRating)
        {
            var result = new Adapters(maxDistance);
            var currentDistance = 0L;
            numbers.Sort();

            if (joltRating > maxDistance)
                throw new Exception("Looks suspect!");

            for(int x=0; x < numbers.Count; x++)
            {
                // Do we need to move further down the list?
                if(numbers[x] < result.JoltRating)
                    continue;

                currentDistance = (numbers[x] * 1L) - result.JoltRating;

                if (currentDistance > maxDistance || currentDistance == 0)
                    break;

                result.JoltRating += currentDistance;
                result.DistanceTotals[currentDistance - 1]++;
            }

            // Don't forget the final jolt rating
            result.JoltRating += joltRating;
            result.DistanceTotals[joltRating - 1]++;

            return result;
        }

        private static long CountPossiblePermutations(List<int> numbers, int maxDistance)
        {
            var adapter = new Adapters(maxDistance);
            numbers.Sort();

            var pathCounts = new long[numbers.Last() + 1];
            var total = 0L;

            pathCounts[0] = 1;

            for (int x = 0; x < numbers.Count; x++)
            {
                // We want to keep running totals against the current adapter
                // based on the previous ones
                total = 0;

                for (int p = 1; p < maxDistance + 1; p++)
                {
                    // Index to look back on previous total
                    var backtrack = numbers[x] - p;

                    // Bounds check
                    if (backtrack >= 0)
                    {
                        total += pathCounts[backtrack];
                    }
                }

                // Store the running totals against the current adapter
                pathCounts[numbers[x]] = total;
            }

            return pathCounts.Last();
        }
    }
}
