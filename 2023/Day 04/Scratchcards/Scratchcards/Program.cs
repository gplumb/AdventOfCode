using System.Net.Sockets;

namespace Scratchcards
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine(PartOne());
            Console.WriteLine(PartTwo());
            Console.ReadLine();
        }


        static long PartOne()
        {
            var data = GetTestData1();
            // var data = LoadFromFile("Input1.txt");
            var total = 0L;

            foreach (var line in data)
            {
                total += Game.Parse(line).Worth;
            }

            return total;
        }


        static long PartTwo()
        {
            // var data = GetTestData1();
            var data = LoadFromFile("Input1.txt");

            var cards = new Dictionary<long, Game>();
            var counts = new Dictionary<long, long>();
            var stack = new Stack<Game>();

            foreach (var line in data)
            {
                var game = Game.Parse(line);
                cards.Add(game.Id, game);
                counts.Add(game.Id, 0);
                stack.Push(game);
            }

            while (stack.Count > 0)
            {
                var game = stack.Pop();
                counts[game.Id]++;

                for(int x = 0; x < game.Matches; x++)
                {
                    var newCardId = game.Id + 1 + x;
                    
                    if (cards.ContainsKey(newCardId))
                        stack.Push(cards[newCardId]);
                }
            }

            return counts.Values.Sum();
        }


        public class Game
        {
            public long Id;

            public List<long> WinningNumbers = new();

            public List<long> ActualNumbers = new();


            public long Worth
            {
                get
                {
                    var matches = WinningNumbers.Intersect(ActualNumbers).ToList().Count;
                    var total = 0;

                    if (matches > 0)
                    {
                        for (int x = 0; x < matches; x++)
                        {
                            total = (x == 0) ? 1 : total * 2;
                        }
                    }

                    return total;
                }
            }


            public long Matches
            {
                get
                {
                    return WinningNumbers.Intersect(ActualNumbers).ToList().Count;
                }
            }


            public static Game Parse(string input)
            {
                var parts = input.Split(":", StringSplitOptions.RemoveEmptyEntries);
                var card = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var id = long.Parse(card[1]);

                parts = parts[1].Split("|", StringSplitOptions.RemoveEmptyEntries);
                var winning = new List<long>();
                var actual = new List<long>();

                foreach (var item in parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries))
                    winning.Add(long.Parse(item));

                foreach (var item in parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries))
                    actual.Add(long.Parse(item));

                return new Game()
                {
                    Id = id,
                    WinningNumbers = winning,
                    ActualNumbers = actual
                };
            }
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
                "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
                "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
                "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
                "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
                "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11"
            };
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> LoadFromFile(string filename)
        {
            var reader = default(StreamReader);
            var data = new List<string>();
            var input = default(string);

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
