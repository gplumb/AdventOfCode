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
        public HashSet<(int x, int y, int z)> Cubes { get; set; }

        public Universe(int size)
        {
            BoundsX = (0, size);
            BoundsY = (0, size);
            BoundsZ = (-(size / 2), -(size / 2) + size);

            Cubes = new HashSet<(int x, int y, int z)>();
        }

        public void WriteSlice(int z)
        {
            for (int y = BoundsY.min; y < BoundsY.max; y++)
            {
                for (int x = BoundsX.min; x < BoundsX.max; x++)
                {
                    var point = (x, y, z);
                    Console.Write(Cubes.Contains(point) ? '#' : '.');
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public int CountNeighbours((int x, int y, int z) point)
        {
            var count = 0;

            for(int nz = point.z - 1; nz < point.z + 2; nz++)
            {
                for (int ny = point.y - 1; ny < point.y + 2; ny++)
                {
                    for (int nx = point.x - 1; nx < point.x + 2; nx++)
                    {
                        if (nz == point.z && ny == point.y && nx == point.x)
                            continue;

                        if (Cubes.Contains((nx, ny, nz)))
                            count++;
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

            var newCubes = new HashSet<(int x, int y, int z)>();

            for (int z = BoundsZ.min; z < BoundsZ.max; z++)
            {
                for (int y = BoundsY.min; y < BoundsY.max; y++)
                {
                    for (int x = BoundsX.min; x < BoundsX.max; x++)
                    {
                        var point = (x, y, z);
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

            Cubes = newCubes;

            if (output)
            {
                for (int z = BoundsZ.min; z < BoundsZ.max; z++)
                {
                    WriteSlice(z);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Puzzle 1
            //var universe = LoadUniverse("TestData.txt");
            var universe = LoadUniverse("Input1.txt");

            for (int i = 0; i < 6; i++)
                universe.Iterate(false);

            var count = universe.Cubes.Count;
            Console.WriteLine($"Count = {count}");
        }

        static Universe LoadUniverse(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    input = reader.ReadLine();
                    var size = input.Length;
                    var universe = new Universe(size);
                    var y = 0;

                    do
                    {
                        for (int x = 0; x < size; x++)
                        {
                            // Only add the coords for a positive coord match
                            if (input[x] == '#')
                            {
                                universe.Cubes.Add((x, y, 0));
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
