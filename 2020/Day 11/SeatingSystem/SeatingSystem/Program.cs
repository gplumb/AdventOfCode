using System;
using System.IO;
using System.Linq;

namespace SeatingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Puzzle 1
            //var isImmediate = true;
            //var tolerance = 4;

            // Puzzle 2
            var isImmediate = false;
            var tolerance = 5;

            //var grid = LoadSeatingGrid("TestData.txt");
            var grid = LoadSeatingGrid("Input1.txt");
            var next = grid.Iterate(isImmediate, tolerance);
            var iterations = 1;

            // Note. This is dangerous if we get a grid that can always be iterated
            while (!grid.Equals(next))
            {
                grid = next;
                next = grid.Iterate(isImmediate, tolerance);
                iterations++;
            }

            Console.WriteLine($"Iterations: {iterations}");
            Console.WriteLine($"Seat count: {next.CountOccupied()}");
        }

        static SeatGrid LoadSeatingGrid(string filename)
        {
            var reader = default(StreamReader);
            var result = new SeatGrid();

            try
            {
                using (reader = new StreamReader(filename))
                {
                    var input = reader.ReadToEnd().Split(Environment.NewLine);
                    result.Grid = new char[input[0].Length, input.Length];

                    for(int y = 0; y < input.Length; y++)
                    {
                        for(int c = 0; c < input[0].Length; c++)
                        {
                            result.Grid[c, y] = input[y][c];
                        }
                    }

                    return result;
                }
            }
            finally
            {
                reader?.Close();
            }
        }

        class SeatGrid
        {
            public char[,] Grid { get; set; }

            public int CountOccupied()
            {
                int total = 0;

                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    for (int x = 0; x < Grid.GetLength(0); x++)
                    {
                        if (Grid[x, y] == '#')
                        {
                            total++;
                        }
                    }
                }

                return total;
            }

            public void PrintGrid()
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    for (int x = 0; x < Grid.GetLength(0); x++)
                    {
                        Console.Write(Grid[x, y]);
                    }

                    Console.WriteLine();
                }
            }

            public SeatGrid Iterate(bool immediate, int tolerance)
            {
                SeatGrid result = new SeatGrid();
                result.Grid = new char[Grid.GetLength(0), Grid.GetLength(1)];

                // Iterate over every seat
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    for (int x = 0; x < Grid.GetLength(0); x++)
                    {
                        // Count occupied seats
                        var total = 0;

                        total += (TryGetSeat(immediate, x, y, -1, -1, out var ch) && ch == '#') ? 1 : 0;    // Top left
                        total += (TryGetSeat(immediate, x, y,  0, -1, out ch) && ch == '#') ? 1 : 0;        // Top
                        total += (TryGetSeat(immediate, x, y,  1, -1, out ch) && ch == '#') ? 1 : 0;        // Top right

                        total += (TryGetSeat(immediate, x, y, -1,  0, out ch) && ch == '#') ? 1 : 0;        // Left
                        total += (TryGetSeat(immediate, x, y,  1,  0, out ch) && ch == '#') ? 1 : 0;        // Right

                        total += (TryGetSeat(immediate, x, y, -1,  1, out ch) && ch == '#') ? 1 : 0;        // Bottom left
                        total += (TryGetSeat(immediate, x, y,  0,  1, out ch) && ch == '#') ? 1 : 0;        // Bottom
                        total += (TryGetSeat(immediate, x, y,  1,  1, out ch) && ch == '#') ? 1 : 0;        // Bottom right

                        // Copy the value over to the target...
                        result.Grid[x, y] = Grid[x, y];

                        // ...and deicde if it's going to mutate
                        
                        // Is the seat empty and no adjacent are occupied?
                        if (Grid[x, y] == 'L' && total == 0)
                        {
                            result.Grid[x, y] = '#';
                        }
                        // Is the seat occupied and needs to be emptied?
                        else if (Grid[x, y] == '#' && total >= tolerance)
                        {
                            result.Grid[x, y] = 'L';
                        }
                    }
                }

                return result;
            }

            public bool TryGetSeat(bool immediate, int x, int y, int dX, int dY, out char ch)
            {
                ch = '\0';

                var nX = x + dX;
                var nY = y + dY;

                if (nX < 0 || nX >= Grid.GetLength(0))
                    return false;

                if (nY < 0 || nY >= Grid.GetLength(1))
                    return false;

                ch = Grid[nX, nY];

                // Can we see further?
                if(!immediate && ch == '.')
                {
                    // Then keep looking recursively until we hit something
                    if (TryGetSeat(immediate, nX, nY, dX, dY, out var nCh))
                    {
                        ch = nCh;
                    }
                }

                return true;
            }

            public bool Equals(SeatGrid other)
            {
                if (Grid.GetLength(0) != other.Grid.GetLength(0))
                    return false;

                if (Grid.GetLength(1) != other.Grid.GetLength(1))
                    return false;

                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    for (int x = 0; x < Grid.GetLength(0); x++)
                    {
                        if (Grid[x, y] != other.Grid[x, y])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
