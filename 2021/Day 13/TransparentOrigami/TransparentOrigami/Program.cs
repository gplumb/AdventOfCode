using System;

namespace TransparentOrigami
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            // var result = Puzzle1(data);
            Puzzle2(data);

            // Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(OrigamiGrid grid)
        {
            var direction = grid.Instuctions[0].Direction;
            var target = grid.Instuctions[0].Target;

            if (direction == FoldDirection.X)
            {
                grid.FoldX(target);
            }
            else
            {
                grid.FoldY(target);
            }

            return grid.CountPoints();
        }

        static void Puzzle2(OrigamiGrid grid)
        {
            foreach(var item in grid.Instuctions)
            {
                if (item.Direction == FoldDirection.X)
                {
                    grid.FoldX(item.Target);
                    continue;
                }
                
                grid.FoldY(item.Target);
            }

            grid.DumpPoints();
        }

        static OrigamiGrid GetDemoData()
        {
            var data = new List<string>()
            {
                "6,10",
                "0,14",
                "9,10",
                "0,3",
                "10,4",
                "4,11",
                "6,0",
                "6,12",
                "4,1",
                "0,13",
                "10,12",
                "3,4",
                "3,0",
                "8,4",
                "1,10",
                "2,14",
                "8,10",
                "9,0",
                "",
                "fold along y=7",
                "fold along x=5",
            };

            return ParseInput(data);
        }

        static string[] INSTURCTION_SPLIT = new string[] { "fold along ", "=" };

        static OrigamiGrid ParseInput(List<string> data)
        {
            var maxX = -1;
            var maxY = -1;
            var coords = new List<(int x, int y)>();
            var folds = new List<Fold>();
            var isData = true;

            foreach (var line in data)
            {
                if(line.Length == 0)
                {
                    isData = false;
                    continue;
                }

                if(isData)
                {
                    var p = line.Split(",").Select(int.Parse).ToList();
                    maxX = (p[0] > maxX) ? p[0] : maxX;
                    maxY = (p[1] > maxY) ? p[1] : maxY;
                    coords.Add((p[0], p[1]));
                    continue;
                }

                // Read fold instructions
                var detail = line.Split(INSTURCTION_SPLIT, StringSplitOptions.RemoveEmptyEntries).ToList();
                var fold = new Fold();
                fold.Direction = (detail[0] == "x" || detail[0] == "X") ? FoldDirection.X : FoldDirection.Y;
                fold.Target = int.Parse(detail[1]);
                folds.Add(fold);
            }

            var result = new OrigamiGrid(maxX + 1, maxY + 1);
            result.Plot(coords);
            result.Instuctions = folds;
            return result;
        }

        public class OrigamiGrid
        {
            public char[,] Points;

            public List<Fold> Instuctions = new List<Fold>();

            public OrigamiGrid(int maxX, int maxY)
            {
                Points = new char[maxX, maxY];
            }

            public void FoldY(int target)
            {
                var newX = Points.GetLength(0);
                var oldY = Points.GetLength(1);
                var result = new char[newX, target];

                for (int y = 0; y < target; y++)
                {
                    for (int x = 0; x < newX; x++)
                    {
                        // Copy the old points over
                        result[x, y] = Points[x, y];

                        // Figure out the co-ord opposite the fold
                        var distance = y - target;
                        var ny = target - distance;
                        ny -= ny == oldY ? 1 : 0;

                        if (Points[x, ny] == '#')
                            result[x, y] = '#';
                    }
                }

                Points = result;
            }

            public void FoldX(int target)
            {
                var newY = Points.GetLength(1);
                var oldX = Points.GetLength(0);

                var result = new char[target, newY];

                for (int y = 0; y < newY; y++)
                {
                    for (int x = 0; x < target; x++)
                    {
                        // Copy the old points over
                        result[x, y] = Points[x, y];

                        // Figure out the co-ord opposite the fold
                        var distance = x - target;
                        var nx = target - distance;
                        nx -= nx == oldX ? 1 : 0;

                        if (Points[nx, y] == '#')
                            result[x, y] = '#';
                    }
                }

                Points = result;
            }

            public void DumpPoints()
            {
                Console.WriteLine();

                for (int y = 0; y < Points.GetLongLength(1); y++)
                {
                    for (int x = 0; x < Points.GetLongLength(0); x++)
                    {
                        Console.Write(Points[x, y] == '#' ? '#' : '.');
                    }

                    Console.WriteLine();
                }
            }

            public int CountPoints()
            {
                var result = 0;

                for (int y = 0; y < Points.GetLongLength(1); y++)
                {
                    for (int x = 0; x < Points.GetLongLength(0); x++)
                    {
                        result += (Points[x, y] == '#' ? 1 : 0);
                    }
                }

                return result;
            }

            internal void Plot(List<(int x, int y)> coords)
            {
                foreach(var p in coords)
                {
                    Points[p.x, p.y] = '#';
                }
            }
        }

        public enum FoldDirection
        {
            X,
            Y
        }

        public class Fold
        {
            public FoldDirection Direction;

            public int Target;
        }

        static OrigamiGrid LoadFromFile(string filename)
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

                    return ParseInput(data);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}