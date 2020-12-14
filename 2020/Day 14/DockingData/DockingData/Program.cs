using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DockingData
{
    class Program
    {
        static void Main(string[] args)
        {
            // Puzzle 1
            //var memory = LoadMaskedMemory("TestData.txt");
            var memory = LoadMaskedMemory("Input1.txt");
            var total = memory.Values.Sum();
            Console.WriteLine($"Total = {total}");
        }

        private static long ApplyMaskToValue(Mask mask, long value)
        {
            // Un-comment this to see the workings

            /*
            Console.WriteLine(mask.Value);
            Console.WriteLine(Convert.ToString(mask.UpperMask, 2).PadLeft(36, '0'));
            Console.WriteLine(Convert.ToString(mask.LowerMask, 2).PadLeft(36, '0'));
            Console.WriteLine();
            Console.WriteLine("Input: ");
            Console.WriteLine(Convert.ToString(value, 2).PadLeft(36, '0'));

            // Overwrite 1s first (OR)
            long d = value | mask.UpperMask;

            Console.WriteLine();
            Console.WriteLine("Pass of line 1: ");
            Console.WriteLine(Convert.ToString(d, 2).PadLeft(36, '0'));

            // Overwrite 2s next (AND NOT)
            d &= ~mask.LowerMask;

            Console.WriteLine();
            Console.WriteLine("Pass of line 2: ");
            Console.WriteLine(Convert.ToString(d, 2).PadLeft(36, '0'));
            Console.WriteLine($"Number = {d}");

            return d;
            */

            long d = value | mask.UpperMask;
            d &= ~mask.LowerMask;
            return d;
        }

        static Dictionary<int, long> LoadMaskedMemory(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);
            var memory = new Dictionary<int, long>();
            var mask = new Mask();

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        // Do we have a new mask?
                        if (input.StartsWith("mask"))
                        {
                            mask = Mask.Parse(input);
                            continue;
                        }

                        var programData = input.Split(new char[] { '[', '=', ']', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var location = int.Parse(programData[1]);
                        var value = int.Parse(programData[2]);

                        memory[location] = ApplyMaskToValue(mask, value);
                    }

                    return memory;
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }

    public class Mask
    {
        public string Value { get; set; }

        public long UpperMask { get; set; }

        public long LowerMask { get; set; }

        public static Mask Parse(string text)
        {
            var mask = text[7..];
            var upperMask = 0L;
            var lowerMask = 0L;
            var list = new List<(int, long)>();

            for (int i = 0; i < mask.Length; i++)
            {
                switch (mask[i])
                {
                    case '0':
                        lowerMask = (lowerMask << 1) + 1;
                        upperMask = upperMask << 1;
                        break;

                    case '1':
                        lowerMask = lowerMask << 1;
                        upperMask = (upperMask << 1) + 1;
                        break;

                    // Skip
                    default:
                        lowerMask = lowerMask << 1;
                        upperMask = upperMask << 1;
                        break;
                }
            }

            return new Mask()
            {
                Value = mask,
                UpperMask = upperMask,
                LowerMask = lowerMask
            };
        }
    }
}
