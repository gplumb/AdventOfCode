using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomCustoms
{
    class Program
    {
        static void Main(string[] args)
        {
            var count = 0;
            //var data = ReadInput("TestData.txt");
            var data = ReadInput("Input1.txt");


            // Puzzle 1
            /*
            var set = new HashSet<char>();

            foreach(var entry in data)
            {
                foreach (var ch in entry.Item2)
                {
                    if (set.Contains(ch))
                        continue;

                    set.Add(ch);
                }

                count += set.Count;
                set.Clear();
            }
            */


            // Puzzle 2
            var dict = new Dictionary<char, int>();

            foreach (var entry in data)
            {
                // Store the counts of each letter
                foreach (var ch in entry.Item2)
                {
                    if(dict.TryGetValue(ch, out var number))
                    {
                        dict[ch]++;
                        continue;
                    }

                    dict[ch] = 1;
                }

                // Now iterate the counts and see if they match the number
                // of people in each group to count
                foreach(var kvp in dict)
                {
                    if (kvp.Value == entry.Item1)
                        count++;
                }

                dict.Clear();
            }

            Console.WriteLine($"Answer count = {count}");
        }


        /// <summary>
        /// Read input data into line-separated in groups
        /// </summary>
        static List<Tuple<int, string>> ReadInput(string filename)
        {
            StreamReader reader = null;
            var input = "";
            var data = new List<Tuple<int,string>>();
            var group = new StringBuilder();
            var groupCount = 0;

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while((input = reader.ReadLine()) != null)
                    {
                        if(input.Length == 0)
                        {
                            data.Add(new Tuple<int, string>(groupCount, group.ToString()));
                            group.Clear();
                            groupCount = 0;
                            continue;
                        }

                        groupCount++;
                        group.Append(input);
                    }

                    if (group.Length > 0)
                        data.Add(new Tuple<int, string>(groupCount, group.ToString()));

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
