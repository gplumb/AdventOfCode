using System;
using System.Diagnostics;
using System.Linq;

namespace SyntaxScoring
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            //var result = Puzzle1(data);
            var result = Puzzle2(data);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(List<string> data)
        {
            return Process(data, true);
        }

        static long Puzzle2(List<string> data)
        {
            return Process(data, false);
        }

        static long Process(List<string> data, bool scoreCorrupt)
        {
            var corruptScores = new Dictionary<char, int>() { {')', 3}, {']', 57}, {'}', 1197}, {'>', 25137} };
            var repairScores = new Dictionary<char, int>() { { ')', 1 }, { ']', 2 }, { '}', 3 }, { '>', 4 } };

            // Track corrupt counts
            var invalid = new Dictionary<char, int>() { {')', 0}, {']', 0}, {'}', 0}, {'>', 0} };

            var repaired = new List<string>();
            var corrupted = new List<string>();
            var isCorrupt = false;
            var stack = new Stack<char>();
            var score = 0L;

            foreach (var entry in data)
            {
                stack.Clear();
                isCorrupt = false;

                foreach (var ch in entry)
                {
                    switch (ch)
                    {
                        case ')':
                            if (stack.Peek() != '(')
                            {
                                isCorrupt = true;
                                invalid[')']++;
                                break;
                            }

                            _ = stack.Pop();
                            break;

                        case ']':
                            if (stack.Peek() != '[')
                            {
                                isCorrupt = true;
                                invalid[']']++;
                                break;
                            }

                            _ = stack.Pop();
                            break;

                        case '}':
                            if (stack.Peek() != '{')
                            {
                                isCorrupt = true;
                                invalid['}']++;
                                break;
                            }

                            _ = stack.Pop();
                            break;

                        case '>':
                            if (stack.Peek() != '<')
                            {
                                isCorrupt = true;
                                invalid['>']++;
                                break;
                            }

                            _ = stack.Pop();
                            break;

                        default:
                            stack.Push(ch);
                            break;
                    }

                    if (isCorrupt)
                    {
                        break;
                    }
                }

                if (isCorrupt)
                {
                    corrupted.Add(entry);
                }
                else
                {
                    var fix = "";
                    while (stack.Count > 0)
                    {
                        switch(stack.Pop())
                        {
                            case '(': fix += ')'; break;
                            case '[': fix += ']'; break;
                            case '{': fix += '}'; break;
                            case '<': fix += '>'; break;
                        }
                    }

                    repaired.Add(fix);
                }
            }

            if (scoreCorrupt)
            {
                foreach (var pair in invalid)
                {
                    score += (pair.Value * corruptScores[pair.Key]);
                }

                return score;
            }

            var scoreTable = new List<long>();

            foreach (var entry in repaired)
            {
                score = 0;

                foreach (var ch in entry)
                    score = (score * 5) + repairScores[ch];

                scoreTable.Add(score);
            }

            scoreTable.Sort();
            return scoreTable[scoreTable.Count / 2];
        }


        static List<string> GetDemoData()
        {
            return new List<string>()
            {
                "[({(<(())[]>[[{[]{<()<>>",
                "[(()[<>])]({[<{<<[]>>(",
                "{([(<{}[<>[]}>{[]{[(<()>",
                "(((({<>}<{<{<>}{[]{[]{}",
                "[[<[([]))<([[{}[[()]]]",
                "[{[{({}]{}}([{[{{{}}([]",
                "{<[[]]>}<{[{[{[]{()[[[]",
                "[<(<(<(<{}))><([]([]()",
                "<{([([[(<>()){}]>(<<{{",
                "<{([{{}}[<[[[<>{}]]]>[]]",
            };
        }

        static List<string> LoadFromFile(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);
            var data = new List<string>();

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                        data.Add(input);

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