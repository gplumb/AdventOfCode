using System;
using System.Collections.Generic;
using System.IO;

namespace ConwayCubes
{
    public class Universe
    {
        public (int min, int max) BoundsX { get; private set; }
        public (int min, int max) BoundsY { get; private set; }
        public (int min, int max) BoundsZ { get; private set; }
        public (int min, int max) BoundsW { get; private set; }

        public HashSet<(int x, int y, int z, int w)> Cubes { get; set; }

        public bool ClampW { get; set; }

        public Universe(int size, bool clampW)
        {
            BoundsX = (0, size);
            BoundsY = (0, size);
            BoundsZ = (-(size / 2), -(size / 2) + size);
            BoundsW = clampW ? (0, 1) : (-(size / 2), -(size / 2) + size);

            ClampW = clampW;
            Cubes = new HashSet<(int x, int y, int z, int w)>();
        }

        public void WriteSlice(int z, int w)
        {
            for (int y = BoundsY.min; y < BoundsY.max; y++)
            {
                for (int x = BoundsX.min; x < BoundsX.max; x++)
                {
                    var point = (x, y, z, w);
                    Console.Write(Cubes.Contains(point) ? '#' : '.');
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public int CountNeighbours((int x, int y, int z, int w) point)
        {
            var count = 0;

            for(int nz = point.z - 1; nz < point.z + 2; nz++)
            {
                for (int ny = point.y - 1; ny < point.y + 2; ny++)
                {
                    for (int nx = point.x - 1; nx < point.x + 2; nx++)
                    {
                        for (int nw = point.w - 1; nw < point.w + 2; nw++)
                        {
                            if (nz == point.z && ny == point.y && nx == point.x && nw == point.w)
                                continue;

                            if (Cubes.Contains((nx, ny, nz, nw)))
                                count++;
                        }
                    }
                }
            }

            return count;
        }

        public void Iterate(bool output)
        {
            // Expand the universe
            BoundsX = (BoundsX.min - 1, BoundsX.max + 1);
            BoundsY = (BoundsY.min - 1, BoundsY.max + 1);
            BoundsZ = (BoundsZ.min - 1, BoundsZ.max + 1);
            BoundsW = ClampW ? BoundsW : (BoundsW.min - 1, BoundsW.max + 1);

            var newCubes = new HashSet<(int x, int y, int z, int w)>();

            for (int w = BoundsW.min; w < BoundsW.max; w++)
            {
                for (int z = BoundsZ.min; z < BoundsZ.max; z++)
                {
                    for (int y = BoundsY.min; y < BoundsY.max; y++)
                    {
                        for (int x = BoundsX.min; x < BoundsX.max; x++)
                        {
                            var point = (x, y, z, w);
                            var count = CountNeighbours(point);
                            var isActive = Cubes.Contains(point);

                            if (isActive)
                            {
                                if (count == 2 || count == 3)
                                    newCubes.Add(point);

                                continue;
                            }

                            if (count == 3)
                            {
                                newCubes.Add(point);
                            }
                        }
                    }
                }
            }

            Cubes = newCubes;

            if (output && ClampW)
            {
                for (int z = BoundsZ.min; z < BoundsZ.max; z++)
                {
                    WriteSlice(z, 0);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Puzzle 1
            //var universe = LoadUniverse("TestData.txt", true);
            //var universe = LoadUniverse("Input1.txt", true);

            // Puzzle 2
            //var universe = LoadUniverse("TestData.txt", false);
            var universe = LoadUniverse("Input1.txt", false);

            for (int i = 0; i < 6; i++)
                universe.Iterate(false);

            var count = universe.Cubes.Count;
            Console.WriteLine($"Count = {count}");
        }

        static Universe LoadUniverse(string filename, bool clampW)
        {
            var reader = default(StreamReader);
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    input = reader.ReadLine();
                    var size = input.Length;
                    var universe = new Universe(size, clampW);
                    var y = 0;

                    do
                    {
                        for (int x = 0; x < size; x++)
                        {
                            // Only add the coords for a positive coord match
                            if (input[x] == '#')
                            {
                                universe.Cubes.Add((x, y, 0, 0));
                            }
                        }

                        y++;
                    }
                    while ((input = reader.ReadLine()) != null);

                    return universe;
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
