using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonsterMessages
{
    class RuleEntry
    {
        public string Data { get; set; } = "";

        public int RuleIndex { get; set; }

        public bool IsData => !string.IsNullOrWhiteSpace(Data);

        public RuleEntry(string item)
        {
            if(item.StartsWith("\""))
            {
                Data = item.Substring(1, item.Length - 2);
                return;
            }

            RuleIndex = int.Parse(item);
        }
    }

    class Data
    {
        public Dictionary<int, List<List<RuleEntry>>> Rules { get; } = new Dictionary<int, List<List<RuleEntry>>>();

        public Dictionary<int, List<string>> Evaluated { get; } = new Dictionary<int, List<string>>();

        public List<string> Messages { get; } = new List<string>();

        public void EvaluateRules()
        {
            for (int x = 0; x < Rules.Count; x++)
            {
                Evaluated[x] = Evaluate(x);
            }
        }

        public List<string> Evaluate(int index)
        {
            if (Evaluated.ContainsKey(index))
                return Evaluated[index];

            var rule = Rules[index];
            var result = new List<string>();

            // Iterate each entry in the rule
            foreach (var entry in rule)
            {
                var list = new List<string>() { "" };

                // Iterate each component in the rule
                foreach (var component in entry)
                {
                    if (component.IsData)
                    {
                        return new List<string>() { component.Data };
                    }

                    if(!Evaluated.ContainsKey(component.RuleIndex))
                        Evaluated[component.RuleIndex] = Evaluate(component.RuleIndex);

                    var newList = new List<string>();

                    for(int p = 0; p < list.Count; p++)
                    {
                        for(int b = 0; b < Evaluated[component.RuleIndex].Count; b++)
                        {
                            newList.Add(list[p] + Evaluated[component.RuleIndex][b]);
                        }
                    }

                    list = newList;
                }

                result.AddRange(list);
            }
            
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Puzzle 1
            //var data = LoadRulesAndMessages("TestData.txt");
            var data = LoadRulesAndMessages("Input1.txt");
            data.EvaluateRules();

            var count = 0;
            foreach(var item in data.Messages)
            {
                if (data.Evaluated[0].Any(x => x.Equals(item)))
                    count++;                
            }

            Console.WriteLine($"Matches: {count}");
        }

        static Data LoadRulesAndMessages(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);
            var result = new Data();

            var isRule = true;

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        if (input.Length == 0)
                        {
                            isRule = false;
                            continue;
                        }

                        if(isRule)
                        {
                            var split = input.Split(new string[] { ":", "|" }, StringSplitOptions.RemoveEmptyEntries);
                            var index = int.Parse(split[0]);
                            var data = new List<List<RuleEntry>>();

                            for (int x = 1; x < split.Length; x++)
                            {
                                var rules = new List<RuleEntry>();
                                var numbers = split[x].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                foreach (var item in numbers)
                                {
                                    rules.Add(new RuleEntry(item));
                                }

                                data.Add(rules);
                            }

                            result.Rules.Add(index, data);
                            continue;
                        }

                        result.Messages.Add(input);
                    }

                    return result;
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
