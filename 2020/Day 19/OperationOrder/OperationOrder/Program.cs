using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OperationOrder
{
    class Program
    {
        enum Operation
        {
            Add,
            Multiply
        }

        static void Main(string[] args)
        {
            //var expression = "1 + 2 * 3 + 4 * 5 + 6";                               // 71 or 231
            //var expression = "1 + (2 * 3) + (4 * (5 + 6))";                         // 51
            //var expression = "2 * 3 + (4 * 5)";                                     // 26 or 46
            //var expression = "5 + (8 * 3 + 9 + 3 * 4 * 3)";                         // 437 or 1445
            //var expression = "5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))";           // 12240 or 669060
            //var expression = "((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2";     // 13632 or 23340
            
            //var result = EvaluateExpression(expression);
            //var result = EvaluateAdvanced(expression);

            var list = LoadExpressions("Input1.txt");
            var sum = 0L;

            foreach (var exp in list)
            {
                // Puzzle 1
                //sum += EvaluateExpression(exp);

                // Puzzle 2
                sum += EvaluateAdvanced(exp);
            }

            Console.WriteLine($"Sum = {sum}");
        }

        private static List<string> Tokenzie(string text)
        {
            var tokens = new List<string>();
            var snippet = new StringBuilder();
            var bracketCount = 0;
            var newToken = false;

            foreach(var ch in text)
            {
                if (ch == ' ')
                {
                    if (bracketCount > 0)
                    {
                        snippet.Append(ch);
                        continue;
                    }

                    newToken = true;
                }
                else if (ch == ')')
                {
                    snippet.Append(ch);
                    bracketCount--;
                }
                else if (ch == '(')
                {
                    snippet.Append(ch);
                    bracketCount++;
                    continue;
                }
                else
                {
                    snippet.Append(ch);
                    continue;
                }

                if(newToken)
                {
                    tokens.Add(snippet.ToString());
                    snippet.Clear();
                    newToken = false;
                }
            }

            if(snippet.Length > 0)
                tokens.Add(snippet.ToString());

            return tokens;
        }

        static long EvaluateExpression(string expression)
        {
            var tokens = Tokenzie(expression);

            long? lhs = null;
            long? rhs = null;
            var operation = Operation.Multiply;

            foreach (var item in tokens)
            {
                switch (item)
                {
                    case "+":
                        operation = Operation.Add;
                        break;

                    case "*":
                        operation = Operation.Multiply;
                        break;

                    default:
                        // Are we dealing with nesting?
                        var value = 0L;

                        if(item.StartsWith("("))
                        {
                            // Yes, strip the brackets and evaluate it
                            var subExpr = item.Substring(1, item.Length - 2);
                            value = EvaluateExpression(subExpr);
                        }
                        else
                        {
                            value = int.Parse(item);
                        }

                        if(!lhs.HasValue)
                        {
                            lhs = value;
                        }
                        else
                        {
                            rhs = value;
                        }

                        break;
                }

                if(lhs.HasValue && rhs.HasValue)
                {
                    switch (operation)
                    {
                        case Operation.Add:
                            lhs = lhs + rhs;
                            rhs = null;
                            break;

                        case Operation.Multiply:
                            lhs = lhs * rhs;
                            rhs = null;
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            return lhs.Value;
        }

        static long EvaluateAdvanced(string expression)
        {
            var tokens = Tokenzie(expression);
            var reduced = new List<string>();

            // Expand out all of the brackets
            while (tokens.Any(x => x.StartsWith("(")))
            {
                foreach (var item in tokens)
                {
                    if (item.StartsWith("("))
                    {
                        // Yes, strip the brackets and evaluate it
                        var subExpr = item.Substring(1, item.Length - 2);
                        var value = EvaluateAdvanced(subExpr).ToString();
                        reduced.Add(value);
                    }
                    else
                    {
                        reduced.Add(item);
                    }
                }

                tokens = reduced;
                // reduced.Clear();
            }

            // Now we cheat and put brackets around all of the plus operations to give it precedence
            var builder = new StringBuilder("");

            for(int x=0; x < tokens.Count; x++)
            {
                if(tokens[x] == "+")
                {
                    tokens[x - 1] = "(" + tokens[x - 1];
                    tokens[x + 1] = tokens[x + 1] + ")";
                }
            }

            var exp = string.Join(" ", tokens);
            var result = EvaluateExpression(exp);
            return result;
        }

        static List<string> LoadExpressions(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);
            var result = new List<string>();

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                        result.Add(input);

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
