using System;
using System.IO;

namespace PasswordPhilosophy
{
    class Program
    {
        private static string[] input = new string[] { "1-3 a: abcde", "1-3 b: cdefg", "2-9 c: ccccccccc" };

        static void Main(string[] args)
        {
            var validCount = 0;
            var total = 0;

            foreach(var item in ReadInput())
            {
                var entry = Entry.Parse(item);
                total++;

                // Puzzle 1
                //if (entry.IsValidCount())
                //    validCount++;

                // Puzzle 2
                if (entry.IsValidPosition())
                    validCount++;
            }

            Console.WriteLine($"Valid passwords: {validCount} out of {total}");
        }

        static string[] ReadInput()
        {
            // return input;

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

    public class Entry
    {
        public int min { get; private set; }

        public int max { get; private set; }

        public char character { get; private set; }

        public string password { get; private set; }

        public static Entry Parse(string text)
        {
            // Note, Regex might have been better, but this will do for now
            var fields = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var ranges = fields[0].Split('-', StringSplitOptions.RemoveEmptyEntries);

            return new Entry()
            {
                min = int.Parse(ranges[0]),
                max = int.Parse(ranges[1]),
                character = fields[1][0],
                password = fields[2]
            };
        }


        /// <summary>
        /// Puzzle one uses the count of characters to determine validity
        /// </summary>
        public bool IsValidCount()
        {
            // The number of times we've seen our lead character
            var seen = 0;

            // LINQ would have been less verbose here
            foreach(var ch in password)
            {
                if (ch == character)
                    seen++;
            }

            return seen >= min && seen <= max;
        }


        /// <summary>
        /// Puzzle two uses the position of characters to determine validity
        /// </summary>
        public bool IsValidPosition()
        {
            var isMin = password[min - 1] == character;
            var isMax = password[max - 1] == character;

            if (isMin && !isMax)
                return true;

            if (!isMin && isMax)
                return true;

            return false;
        }
    }
}
