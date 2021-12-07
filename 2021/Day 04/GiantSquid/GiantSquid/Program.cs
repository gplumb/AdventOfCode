using System;
using System.Diagnostics;
using System.Linq;

namespace GiantSquid
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData();
            var data = LoadFromFile("Input1.txt");

            //var result = Puzzle1(data.Calls, data.Grids);
            var result = Puzzle2(data.Calls, data.Grids);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static int Puzzle1(List<int> calls, List<Grid> grids)
        {
            foreach(var call in calls)
            {
                foreach(var grid in grids)
                {
                    grid.MarkNumber(call);

                    if(grid.DidWin)
                    {
                        grid.DumpGrid();
                        return grid.TotalUnmarked * call;
                    }
                }
            }

            return -1;
        }

        static int Puzzle2(List<int> calls, List<Grid> grids)
        {
            var lastWinner = -1;

            foreach (var call in calls)
            {
                for (int x = 0; x < grids.Count; x++)
                {
                    grids[x].MarkNumber(call);

                    if (grids[x].DidWin)
                    {
                        lastWinner = grids[x].TotalUnmarked * call;
                        grids.RemoveAt(x);                     
                        x--;
                    }
                }
            }

            return lastWinner;
        }

        public class Grid
        {
            private int _size;
            private Dictionary<int, bool> _data;

            public bool DidWin => EvaluateGrid();

            public int TotalMarked => _data.Where(x => x.Value).Select(x => x.Key).Sum();

            public int TotalUnmarked => _data.Where(x => !x.Value).Select(x => x.Key).Sum();

            public Grid(int size, List<int> numbers)
            {
                _size = size;
                _data = new Dictionary<int, bool>();
                numbers.ForEach(x => _data.Add(x, false));
            }
            
            public void MarkNumber(int number)
            {
                if (_data.ContainsKey(number))
                    _data[number] = true;
            }

            public bool EvaluateGrid()
            {
                var candidate = true;
                var keys = _data.Keys.ToList();

                // Check horizontals
                for (var y = 0; y < _size; y++)
                {
                    candidate = true;

                    for (var x = 0; x < _size; x++)
                    {
                        var index = (y * _size) + x;

                        if (!_data[keys[index]])
                        {
                            candidate = false;
                            break;
                        }
                    }

                    if (candidate)
                        return true;
                }

                // Check verticals
                for (var y = 0; y < _size; y++)
                {
                    candidate = true;

                    for (var x = 0; x < _size; x++)
                    {
                        var index = y + (x * _size);

                        if (!_data[keys[index]])
                        {
                            candidate = false;
                            break;
                        }
                    }

                    if (candidate)
                        return true;
                }

                return false;
            }
            public void DumpGrid()
            {
                var keys = _data.Keys.ToList();

                for (var y = 0; y < _size; y++)
                {
                    for (var x = 0; x < _size; x++)
                    {
                        var index = (y * _size) + x;
                        Console.Write(string.Format("{0:00}", keys[index]));
                        Console.Write(":");
                        Console.Write(_data[keys[index]] ? "#" : " ");
                        Console.Write(" ");
                    }

                    Console.WriteLine("");
                }

                Console.WriteLine("");
            }
        }

        static (List<int> Calls, List<Grid> Grids) GetDemoData()
        {
            var calls = new List<int>() { 7, 4, 9, 5, 11, 17, 23, 2, 0, 14, 21, 24, 10, 16, 13, 6, 15, 25, 12, 22, 18, 20, 8, 19, 3, 26, 1 };

            var gridOne   = "22 13 17 11 0 8 2 23 4 24 21 9 14 16 7 6 10 3 18 5 1 12 20 15 19".Split(" ").Select(int.Parse).ToList();
            var gridTwo   = "3 15 0 2 22 9 18 13 17 5 19 8 7 25 23 20 11 10 24 4 14 21 16 12 6".Split(" ").Select(int.Parse).ToList();
            var gridThree = "14 21 17 24 4 10 16 15 9 19 18 8 23 26 20 22 11 13 6 5 2 0 12 3 7".Split(" ").Select(int.Parse).ToList();

            var grids = new List<Grid>()
            {
                new Grid(5, gridOne),
                new Grid(5, gridTwo),
                new Grid(5, gridThree),
            };

            return (calls, grids);
        }

        static (List<int> Calls, List<Grid> Grids) LoadFromFile(string filename)
        {
            var reader = default(StreamReader);
            var result = new List<string>();
            var input = default(string);
            var grids = new List<Grid>();

            var gridSize = 5;

            try
            {
                using (reader = new StreamReader(filename))
                {
                    var calls = reader.ReadLine().Split(",").Select(int.Parse).ToList();

                    while ((input = reader.ReadLine()) != null)
                    {
                        if (input.Length == 0)
                            continue;

                        var numbers = new List<int>();

                        for(int x = 0; x < gridSize; x++)
                        {
                            input = (x == 0) ? input : reader.ReadLine();
                            var newNumbers = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                            numbers.AddRange(newNumbers);
                        }

                        grids.Add(new Grid(5, numbers));
                    }

                    return (calls, grids);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}