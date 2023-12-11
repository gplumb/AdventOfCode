using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CamelCards
{
    internal class Program
    {
        static Dictionary<char, int> CardValues = new()
        {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'J', 11 },
            { 'T', 10 },
            { '9', 9 },
            { '8', 8 },
            { '7', 7 },
            { '6', 6 },
            { '5', 5 },
            { '4', 4 },
            { '3', 3 },
            { '2', 2 },
        };


        /// <summary>
        /// The numbers will be used for sorting later
        /// </summary>
        public enum HandType
        {
            FiveOfAKind = 100,
            FourOfAKind = 90,
            FullHouse = 80,
            ThreeOfAKind = 70,
            TwoPair = 60,
            OnePair = 50,
            HighCard = 40
        }

        static void Main(string[] args)
        {
            // SpotChecks();
            Console.WriteLine(PartOne());
        }


        static long PartOne()
        {
            //var data = GetTestData1();
            var data = LoadFromFile("Input1.txt");
            var hands = new List<Hand>();

            foreach (var item in data)
            {
                var datum = item.Split(" ");
                var hand = new Hand(datum[0]);
                hand.Bet = long.Parse(datum[1]);
                hands.Add(hand);
            }

            // Ascending sort (weakest first)
            hands.Sort();

            var total = 0L;
            var rank = 0;

            foreach (var hand in hands)
            {
                rank++;
                total += (hand.Bet * rank);
            }

            return total;
        }


        static void SpotChecks()
        {
            // Yeah yeah yeah, this should be xUnit
            if (new Hand("AAAAA").Type != HandType.FiveOfAKind)
                throw new Exception("Bug!");

            if (new Hand("AA8AA").Type != HandType.FourOfAKind)
                throw new Exception("Bug!");

            if (new Hand("23332").Type != HandType.FullHouse)
                throw new Exception("Bug!");

            if (new Hand("TTT98").Type != HandType.ThreeOfAKind)
                throw new Exception("Bug!");

            if (new Hand("23432").Type != HandType.TwoPair)
                throw new Exception("Bug!");

            if (new Hand("A23A4").Type != HandType.OnePair)
                throw new Exception("Bug!");

            if (new Hand("23456").Type != HandType.HighCard)
                throw new Exception("Bug!");

            // Full house always beats 3 of a kind
            var hands = new List<Hand>();
            hands.Add(new Hand("TTT98"));
            hands.Add(new Hand("23332"));

            // Sort descending (higherst first)            
            hands.Sort((x, y) => y.CompareTo(x));

            if (hands[0].Type != HandType.FullHouse)
                throw new Exception("Bug!");

            // Check equal values
            hands.Clear();
            hands.Add(new Hand("2AAAA"));
            hands.Add(new Hand("33332"));

            hands.Sort((x, y) => y.CompareTo(x));

            if (!hands[0].Data.Equals("33332"))
              throw new Exception("Bug!");


            hands.Clear();
            hands.Add(new Hand("77888"));
            hands.Add(new Hand("77788"));

            hands.Sort((x, y) => y.CompareTo(x));

            if (!hands[0].Data.Equals("77888"))
                throw new Exception("Bug!");
        }


        public class Hand : IComparable
        {
            public int[] Cards = new int[5];            
            public HandType Type;
            public string Data = "";

            public long Bet = 0;

            public Hand(string text)
            {
                if (string.IsNullOrWhiteSpace(text) || text.Length < 5)
                    throw new Exception("Bad data");

                Data = text.ToUpper();

                // Stash the cards, values and totals (the later will be used for sorting if types match)
                for (int x = 0; x < 5; x++)
                {
                    Cards[x] = CardValues[Data[x]];
                }

                ClassifyHand();
            }

            public int CompareTo(object? other)
            {
                if (other == null)
                    return 1;

                var otherHand = other as Hand;

                if (otherHand == null)
                    return 1;

                if (Type != otherHand.Type)
                    return Type.CompareTo(otherHand.Type);

                for(int x = 0; x < 5; x++)
                {
                    if (Cards[x] > otherHand.Cards[x])
                        return 1;

                    if (Cards[x] < otherHand.Cards[x])
                        return -1;

                    // We carry on for equal ones
                }

                // They are the same
                return 0;
            }

            private void ClassifyHand()
            {
                var counts = new Dictionary<int, int>();

                foreach (var card in Cards)
                {
                    if (!counts.ContainsKey(card))
                        counts.Add(card, 0);

                    counts[card]++;
                }

                // Yup, this is not as efficient as a single-pass loop that checks things on the fly, but for such a
                // small amount of data we don't really care
                if (counts.Any(x => x.Value.Equals(5)))
                {
                    Type = HandType.FiveOfAKind;
                    return;
                }

                if (counts.Any(x => x.Value.Equals(4)))
                {
                    Type = HandType.FourOfAKind;
                    return;
                }

                if (counts.Any(x => x.Value.Equals(3)))
                {
                    if (counts.Any(x => x.Value.Equals(2)))
                    {
                        Type = HandType.FullHouse;
                        return;
                    }

                    Type = HandType.ThreeOfAKind;
                    return;
                }

                if (counts.Where(x => x.Value == 2).Count() == 2)
                {
                    Type = HandType.TwoPair;
                    return;
                }

                if (counts.Where(x => x.Value == 2).Count() == 1)
                {
                    Type = HandType.OnePair;
                    return;
                }

                if (counts.Where(x => x.Value == 1).Count() == 5)
                {
                    Type = HandType.HighCard;
                    return;
                }

                throw new Exception("Guru meditation error!");
            }
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "32T3K 765",
                "T55J5 684",
                "KK677 28",
                "KTJJT 220",
                "QQQJA 483",
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
