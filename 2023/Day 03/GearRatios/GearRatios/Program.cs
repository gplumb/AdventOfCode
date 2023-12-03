
namespace GearRatios
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Solve(true));
            Console.WriteLine(Solve(false));
            Console.ReadLine();
        }


        static int number = 0;
        static int nLength = 0;
        static int startX = -1;
        static int startY = -1;
        static bool isCounting = false;


        static long Solve(bool isPartTwo)
        {
            // var input = GetTestData1();
            var input = LoadFromFile("Input1.txt");
            var data = ToGrid(input);

            // Now search for gears
            var total = 0L;
            var isGear = false;

            var maxX = data.GetLength(0);
            var maxY = data.GetLength(1);
            var candidates = new List<Candidate>();

            for (int gY = 0; gY < maxY; gY++)
            {
                // Check for wrap-around in the grid!
                if (isCounting)
                {
                    isGear = IsPart(data, maxX, maxY, out var candidate);
                    total += (isGear) ? number : 0;
                    number = 0;

                    if (candidate != null)
                        candidates.Add(candidate);
                }

                for (int gX = 0; gX < maxX; gX++)
                {
                    var ch = data[gX, gY];

                    // Do we have blank?
                    if (ch == '.')
                    {
                        // Were we counting a new number?
                        if (!isCounting)
                            continue;

                        // Stop counting and look around the number
                        isGear = IsPart(data, maxX, maxY, out var candidate);
                        total += (isGear) ? number : 0;
                        number = 0;

                        if (candidate != null)
                            candidates.Add(candidate);
                    }
                    // Do we have a number?
                    else if (char.IsNumber(ch))
                    {
                        // Carry on counting
                        if (isCounting)
                        {
                            number = number * 10 + int.Parse("" + ch);
                            nLength++;
                            continue;
                        }

                        // Start counting
                        number = int.Parse("" + ch);
                        isCounting = true;
                        startX = gX;
                        startY = gY;
                        nLength = 1;
                    }
                    // We have a symbol
                    else
                    {
                        if (isCounting)
                        {
                            isGear = IsPart(data, maxX, maxY, out var candidate);
                            total += (isGear) ? number : 0;
                            number = 0;

                            if (candidate != null)
                                candidates.Add(candidate);
                        }
                    }
                }
            }

            if (isPartTwo)
            {
                total = 0;

                var starMatches = candidates.Where(x => x.symbol.Equals('*')).Select(x => x).ToList();

                // Note. The stars are adjacent to exactly 2 numbers, so we can cheat with linq
                var matchingPairs = starMatches.SelectMany((x, i) => starMatches.Skip(i + 1), (x, y) => new { First = x, Second = y })
                                             .Where(pair => pair.First.symbolX == pair.Second.symbolX && pair.First.symbolY == pair.Second.symbolY)
                                             .ToList();

                total = 0;

                foreach (var pair in matchingPairs)
                {
                    // Console.WriteLine($"{pair.First.number} * {pair.Second.number}");
                    total = total + (pair.First.number * pair.Second.number);
                }
            }

            return total;
        }


        class Candidate
        {
            public int number;
            public char symbol;
            public int symbolX;
            public int symbolY;
        }


        private static bool IsPart(char[,] data, int maxX, int maxY, out Candidate candidate)
        {
            var isPart = false;
            candidate = null;

            for (int sY = startY - 1; sY < startY + 2; sY++)
            {
                if (isPart == true)
                    break;

                if (sY < 0 || sY >= maxY)
                    continue;

                for (int sX = startX - 1; sX < startX + nLength + 1; sX++)
                {
                    if (sX < 0 || sX >= maxX)
                        continue;

                    var ch2 = data[sX, sY];

                    if (ch2 == '.' || char.IsNumber(ch2))
                        continue;

                    candidate = new Candidate()
                    {
                        number = number,
                        symbol = ch2,
                        symbolX = sX,
                        symbolY = sY
                    };

                    // Console.WriteLine(number);
                    isPart = true;
                    break;
                }
            }

            isCounting = false;
            nLength = -1;
            startX = -1;
            startY = -1;

            return isPart;
        }


        private static char[,] ToGrid(List<string> input)
        {
            var x = input[0].Length;
            var y = input.Count;

            var grid = new char[x, y];

            for(int gY = 0; gY < y; gY++)
            {
                for (int gX = 0; gX < x; gX++)
                {
                    grid[gX, gY] = input[gY][gX];
                }
            }

            return grid;
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "467..114..",
                "...*......",
                "..35..633.",
                "......#...",
                "617*......",
                ".....+.58.",
                "..592.....",
                "......755.",
                "...$.*....",
                ".664.598.."
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
