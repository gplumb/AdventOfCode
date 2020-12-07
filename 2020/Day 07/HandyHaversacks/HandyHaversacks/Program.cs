using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HandyHaversacks
{
    class Bag
    {
        public string Name { get; set; }

        public Dictionary<Bag, int> Contents = new Dictionary<Bag, int>();
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var bags = LoadBags("TestData.txt");
            var bags = LoadBags("Input1.txt");

            // Problem 1
            //var count = CountParents(bags, "shiny gold");
            //Console.WriteLine($"Possible parent bag count: {count}");

            // Problem 2
            var count = CountChildren(bags, "shiny gold");
            Console.WriteLine($"Child bag count: {count}");
        }

        static List<Bag> LoadBags(string filename)
        {
            var bags = new List<Bag>();
            var reader = default(StreamReader);
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        var bagname = Regex.Match(input, "(.+) bags contain").Groups[1];

                        var parent = new Bag()
                        {
                            Name = bagname.Value
                        };

                        bags.Add(parent);

                        if (Regex.IsMatch(input, "no other bags"))
                            continue;

                        foreach(Match match in Regex.Matches(input, "([0-9]+)([a-zA-Z ]+)(?=bag|bags)"))
                        {
                            parent.Contents.Add(new Bag()
                            {
                                Name = match.Groups[2].Value.Trim()
                            }, int.Parse(match.Groups[1].Value));
                        }

                    }

                    return bags;
                }
            }
            finally
            {
                reader?.Close();
            }
        }

        static int CountParents(List<Bag> bags, string target)
        {
            var stack = new Stack<Bag>();
            var count = 0;

            foreach (var item in bags)
            {
                if (item.Name.Equals(target))
                    continue;

                stack.Push(item);

                while (stack.Count > 0)
                {
                    var next = stack.Pop();

                    if (next.Name == target)
                    {
                        count++;
                        stack.Clear();
                        continue;
                    }

                    foreach (var child in next.Contents)
                    {
                        var branch = bags.Find(x => x.Name.Equals(child.Key.Name));
                        stack.Push(branch);
                    }
                }
            }

            return count;
        }

        static int CountChildren(List<Bag> bags, string target)
        {
            var count = 0;
            var bag = bags.Find(x => x.Name.Equals(target));

            foreach (var child in bag.Contents)
            {
                count += child.Value + (child.Value * CountChildren(bags, child.Key.Name));
            }

            return count;
        }
    }
}
