using System;
using System.Collections.Generic;

namespace RambunctiousRecitation
{
    class Program
    {
        // Note. Should really build some unit tests for this

        // static List<int> Numbers = new List<int>() { 0, 3, 6 };      // 436 and 175594
        // static List<int> Numbers = new List<int>() { 1, 3, 2 };      // 1 and 2578
        // static List<int> Numbers = new List<int>() { 2, 1, 3 };      // 10 and 3544142
        // static List<int> Numbers = new List<int>() { 1, 2, 3 };      // 27 and 261214
        // static List<int> Numbers = new List<int>() { 2, 3, 1 };      // 78 and 6895259
        // static List<int> Numbers = new List<int>() { 3, 2, 1 };      // 438 and 18
        // static List<int> Numbers = new List<int>() { 3, 1, 2 };      // 1836 and 362

        // Puzzle data
        static List<int> Numbers = new List<int>() { 16, 11, 15, 0, 1, 7 };

        static void Main(string[] args)
        {
            var memory = Memorize(Numbers, out var turn, out var last);

            // Puzzle 1
            // var target = 2020;

            // Puzzle 2
            var target = 30000000;

            // Now we play the game
            for (; turn < target + 1; turn++)
            {
                // Has the last number been spoken before?
                if(last.Item2 == -1)
                {
                    // No
                    StoreNumber(memory, 0, turn, out last);
                    continue;
                }

                // Yes, then when?
                var entry = memory[last.Item1];
                var newNumber = entry.Item1 - entry.Item2;

                StoreNumber(memory, newNumber, turn, out last);
            }

            Console.WriteLine($"Last number spoken: {last.Item1}");
        }

        static void StoreNumber(Dictionary<int, (int, int)> memory, int number, int turn, out (int, int) last)
        {
            if (memory.ContainsKey(number))
            {
                var value = memory[number];
                memory[number] = (turn, value.Item1);
            }
            else
            {
                memory[number] = (turn, -1);
            }

            last = (number, memory[number].Item2);

            // Console.WriteLine($"Number {number} on turn {turn} (Previously seen on {memory[number].Item2})");
            // Console.WriteLine($"Turn: {turn}: {number}");
        }

        static Dictionary<int, (int, int)> Memorize(List<int> numbers, out int turn, out (int, int) last)
        {
            // The tuple is the memory of the a given entry's occurence and previous occurence
            var memory = new Dictionary<int, (int, int)>();
            last = (-1, -1);

            for (turn = 1; turn < Numbers.Count + 1; turn++)
            {
                var n = Numbers[turn - 1];
                StoreNumber(memory, n, turn, out last);
            }

            return memory;
        }
    }
}
