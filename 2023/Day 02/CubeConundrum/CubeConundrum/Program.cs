namespace CubeConundrum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(PartOne());
            // Console.WriteLine(PartTwo());
            Console.ReadLine();
        }


        class Game
        {
            public int Id { get; set; }

            public List<GameSet> Sets = new();


            public static Game Parse(string input)
            {
                var data = input.Split(":", StringSplitOptions.RemoveEmptyEntries);
                var sets = data[1].Split(";", StringSplitOptions.RemoveEmptyEntries);
                var gameId = int.Parse(data[0].Split(" ", StringSplitOptions.RemoveEmptyEntries)[1]);

                var result = new Game()
                {
                    Id = gameId,
                    Sets = new()
                };

                foreach (var set in sets)
                {
                    var cubes = set.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    var gameSet = new GameSet();

                    foreach (var entry in cubes)
                    {
                        var current = entry.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        var cube = new Cube()
                        {
                            Count = int.Parse(current[0])
                        };

                        switch (current[1])
                        {
                            case "red":
                                cube.Colour = Colour.Red;
                                break;

                            case "green":
                                cube.Colour = Colour.Green;
                                break;

                            case "blue":
                                cube.Colour = Colour.Blue;
                                break;

                            default:
                                throw new Exception("Bad input");
                        }

                        gameSet.Cubes.Add(cube);
                    }

                    result.Sets.Add(gameSet);
                }

                return result;
            }
        }

        class GameSet
        {
            public List<Cube> Cubes = new();
        }

        class Cube
        {
            public Colour Colour { get; set; }

            public int Count { get; set; }
        }

        enum Colour
        {
            Red,
            Green,
            Blue
        }

        static int PartOne()
        {
            // var data = GetTestData1();
            var data = LoadFromFile("Input1.txt");
            var games = new List<Game>();

            foreach (var entry in data)
            {
                games.Add(Game.Parse(entry.ToLowerInvariant()));
            }

            return SumValidGames(games, 12, 13, 14);
        }


        static int SumValidGames(List<Game> games, int red, int green, int blue)
        {
            int total = 0;

            foreach (var current in games)
            {
                var isValid = true;

                foreach (var set in current.Sets)
                {
                    if (isValid == false)
                        break;

                    foreach (var cube in set.Cubes)
                    {
                        switch (cube.Colour)
                        {
                            case Colour.Red:
                                if (cube.Count > red)
                                {
                                    isValid = false;
                                    break;
                                }

                                continue;

                            case Colour.Green:
                                if (cube.Count > green)
                                {
                                    isValid = false;
                                    break;
                                }

                                continue;

                            case Colour.Blue:
                                if (cube.Count > blue)
                                {
                                    isValid = false;
                                    break;
                                }

                                continue;

                            default:
                                throw new Exception("Bad input");
                        }
                    }
                }

                if (isValid)
                    total += current.Id;
            }

            return total;
        }


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
                "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
                "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
                "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
                "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
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
