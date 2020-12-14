using System;
using System.Collections.Generic;
using System.IO;

namespace ShuttleSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            //var schedule = LoadSchedule("TestData.txt");
            var schedule = LoadSchedule("Input1.txt");

            // Puzzle 1
            //var nearest = FindNearest(schedule);
            //var result = nearest.Item2 * nearest.Item1;
            //Console.WriteLine($"Bus Id * Wait time = {result}");

            // Puzzle 2
            var time = FindTimestamp(schedule.Item2);
            Console.WriteLine($"Timestamp = {time}");
        }

        private static Tuple<int, int> FindNearest(Tuple<int, List<int>> schedule)
        {
            // At timestamp 0, every bus simultaneously departed from the sea port.
            // After that, each bus travels to the airport, then various other locations, and finally returns
            // to the sea port to repeat its journey forever.
            var bus = -1;
            var shortest = int.MaxValue;
            var wait = 0;

            foreach(var id in schedule.Item2)
            {
                if (id == -1)
                    continue;

                // Find out the wait time based on the given first time
                wait = id - (schedule.Item1 % id);

                if(wait < shortest)
                {
                    shortest = wait;
                    bus = id;
                }
            }

            return new Tuple<int, int>(bus, shortest);
        }

        static Tuple<int, List<int>> LoadSchedule(string filename)
        {
            var reader = default(StreamReader);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    var time = int.Parse(reader.ReadLine());
                    var data = reader.ReadLine().Split(",");
                    var schedule = new List<int>();

                    for(int x=0; x < data.Length; x++)
                    {
                        schedule.Add(int.TryParse(data[x], out var result) ? result : -1);
                    }

                    return new Tuple<int, List<int>>(time, schedule);
                }
            }
            finally
            {
                reader?.Close();
            }
        }

        static long FindTimestamp(List<int> buses)
        {
            var interval = buses.Find(x => x > -1) * 1L;
            var start = buses.FindIndex(x => x > -1);
            var time = 0L;

            for (int x = start + 1; x < buses.Count; x++)
            {
                var bus = buses[x];

                if (bus == -1)
                    continue;

                // This is the target modulo for this bus relative to it's position in the sequence
                var modTarget = bus - (x % bus);

                // If we aren't conveniently at our modulo target, then
                // increment time until we hit it
                while (time % bus != modTarget)
                    time += interval;

                // We need to multiply the interval each time as this is the smallest known
                // chunk of time in which the pattern for the previous buses will repeat
                interval = interval * bus;
            }

            return time;
        }
    }
}
