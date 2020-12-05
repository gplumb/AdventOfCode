using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryBoarding
{
    class Program
    {
        private static string TestInput = "FBFBBFFRLR";         // Seat Id = 357
        //private static string TestInput = "BFFFBBFRRR";       // Seat Id =  567
        //private static string TestInput = "FFFBBBFRRR";       // Seat Id =  119
        //private static string TestInput = "BBFFBBFRLL";       // Seat Id =  820

        static void Main(string[] args)
        {
            // Test data
            //var id = ParseSeatId(TestInput);

            // Puzzle 1
            /*
            var maxSeatId = -1;
            foreach(var item in ReadInput("Input1.txt"))
            {
                var id = ParseSeatId(item);
                maxSeatId = (id > maxSeatId) ? id : maxSeatId;
            }

            Console.WriteLine($"Max seat id = {maxSeatId}");
            */

            // Puzzle 2
            var seats = new List<int>();
            foreach (var item in ReadInput("Input1.txt"))
            {
                var id = ParseSeatId(item);
                seats.Add(id);
            }

            // Sort the seat ids into order
            seats.Sort();
            var seatId = -1;

            for(int x = 0; x < seats.Count - 1; x++)
            {
                var predictedNext = seats[x] + 2;

                if(seats[x + 1] == predictedNext)
                {
                    seatId = predictedNext - 1;
                    break;
                }
            }

            if (seatId == -1)
            {
                Console.WriteLine("Not found");
            }
            else
            {
                Console.WriteLine($"My seat is {seatId}");
            }
        }


        private static int ParseSeatId(string input)
        {
            // Note. Don't bother validating the boarding characters for now
            if (input.Length != 10)
                throw new Exception("Invalid boarding pass");

            var row = BinarySearch(input, 'F', 0, 0, 127, 7);
            var col = BinarySearch(input, 'L', 7, 0, 7, 3);
            return (row * 8) + col;
        }


        private static int BinarySearch(string input, char lowIndicator, int offset, int min, int max, int iterations)
        {
            for (int x = 0; x < iterations - 1; x++)
            {
                // Lower partition
                if (input[offset + x] == lowIndicator)
                {
                    max = min + ((max + 1 - min) / 2) - 1;
                }
                // Upper partition
                else
                {
                    min += ((max + 1 - min) / 2);
                }
            }

            return input[offset + (iterations - 1)] == lowIndicator ? min : max;
        }


        /// <summary>
        /// Does exactly what it says on the tin
        /// </summary>
        static string[] ReadInput(string filename)
        {
            StreamReader reader = null;

            try
            {
                using (reader = new StreamReader("Input1.txt"))
                {
                    return reader.ReadToEnd().Split(Environment.NewLine);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
