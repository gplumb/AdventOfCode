using System;
using System.Net.NetworkInformation;
using System.Reflection;

namespace SeedFertilizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // SpotChecks();
            Console.WriteLine(PartOne());
        }


        /// <summary>
        /// Yes, this should be xUnit
        /// </summary>
        static void SpotChecks()
        {
            var range = Range.Parse("50 98 2");
            var range2 = Range.Parse("52 50 48");

            var map = new RangeMap();
            map.AddRange(range);
            map.AddRange(range2);

            // Not mapped
            if (map[1] != 1)  throw new Exception("Fail");
            if(map[10] != 10) throw new Exception("Fail");
            
            // Mapped
            if(map[98] != 50)  throw new Exception("Fail");
            if (map[99] != 51) throw new Exception("Fail");
            if(map[53] != 55)  throw new Exception("Fail");

            // Not mapped
            if(map[112] != 112) throw new Exception("Fail");

            // Unit tests from problem description
            if (map[79] != 81) throw new Exception("Fail");
            if (map[14] != 14) throw new Exception("Fail");
            if (map[55] != 57) throw new Exception("Fail");
            if (map[13] != 13) throw new Exception("Fail");
        }


        static long PartOne()
        {
            //var data = GetTestData1();
            var data = LoadFromFile("Input1.txt");
            var almanac = Almanac.Parse(data);

            var lowest = long.MaxValue;

            foreach (var seed in almanac.Seeds)
            {
                var location = almanac.GetLocationForSeed(seed);
                lowest = (location < lowest) ? location : lowest;
            }

            return lowest;
        }


        class Almanac
        {
            public List<long> Seeds = new();

            public RangeMap Seed_To_Soil = new();
            public RangeMap Soil_To_Fertilizer = new();
            public RangeMap Fertilizer_To_Water = new();
            public RangeMap Water_To_Light = new();
            public RangeMap Light_To_Temperature = new();
            public RangeMap Temperature_To_Humidity = new();
            public RangeMap HumidityToLocation = new();

            public long GetLocationForSeed(long seed)
            {
                var index = Seed_To_Soil[seed];
                index = Soil_To_Fertilizer[index];
                index = Fertilizer_To_Water[index];
                index = Water_To_Light[index];
                index = Light_To_Temperature[index];
                index = Temperature_To_Humidity[index];
                return HumidityToLocation[index];
            }

            public static Almanac Parse(List<string> data)
            {
                var result = new Almanac();
                var map = default(RangeMap);
                var isNew = false;

                foreach (var item in data)
                {
                    // Ignore empty lines
                    if (item.Length == 0)
                        continue;

                    // Are we at the start of a category?
                    isNew = !char.IsNumber(item[0]);

                    if (isNew)
                    {
                        var parts = item.Split(":", StringSplitOptions.RemoveEmptyEntries);

                        switch (parts[0].ToLowerInvariant())
                        {
                            case "seeds":
                                foreach (var n in parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries))
                                {
                                    result.Seeds.Add(long.Parse(n));
                                }
                                continue;

                            case "seed-to-soil map":
                                map = result.Seed_To_Soil;
                                continue;

                            case "soil-to-fertilizer map":
                                map = result.Soil_To_Fertilizer;
                                continue;

                            case "fertilizer-to-water map":
                                map = result.Fertilizer_To_Water;
                                continue;

                            case "water-to-light map":
                                map = result.Water_To_Light;
                                continue;

                            case "light-to-temperature map":
                                map = result.Light_To_Temperature;
                                continue;

                            case "temperature-to-humidity map":
                                map = result.Temperature_To_Humidity;
                                continue;

                            case "humidity-to-location map":
                                map = result.HumidityToLocation;
                                continue;
                        }

                        throw new Exception("Bad data");
                    }

                    // Parse the range
                    map.AddRange(Range.Parse(item));                    
                }

                return result;
            }
        }


        class RangeMap
        {
            SortedDictionary<long, Range> Ranges = new();

            public long this[long index]
            {
                get
                {
                    if (index < 0)
                        throw new Exception("Invalid index");

                    // This works because we use a sorted dictionary
                    foreach (var r in Ranges.Values)
                    {
                        // Did we find something un-mapped on the lower-bound?
                        if (index < r.Source)
                            return index;

                        // Do we have something mapped?
                        if (index >= r.Source && index <= r.MaxSource)
                            return r[index];
                    }

                    // If we get this far, we have something un-mapped on the upper-bound
                    return index;
                }
            }

            public void AddRange(Range range) => Ranges.Add(range.Source, range);
        }


        class Range
        {
            public long Source;

            public long Destination;

            public long RangeLength;

            public long MaxSource => Source + RangeLength - 1;

            public long this[long index]
            {
                get
                {
                    if (index < Source || index > MaxSource)
                        return long.MinValue;

                    long offset = index - Source;
                    return Destination + offset;
                }
            }

            

            public static Range Parse(string text)
            {
                var parts = text.Split(" ");

                return new Range()
                {
                    Destination = long.Parse(parts[0]),
                    Source = long.Parse(parts[1]),
                    RangeLength = long.Parse(parts[2]),
                };
            }
        }


        // source category
        // destination category


        /// <summary>
        /// Test data
        /// </summary>
        static List<string> GetTestData1()
        {
            return new List<string>()
            {
                "seeds: 79 14 55 13",
                "",
                "seed-to-soil map:",
                "50 98 2",
                "52 50 48",
                "",
                "soil-to-fertilizer map:",
                "0 15 37",
                "37 52 2",
                "39 0 15",
                "",
                "fertilizer-to-water map:",
                "49 53 8",
                "0 11 42",
                "42 0 7",
                "57 7 4",
                "",
                "water-to-light map:",
                "88 18 7",
                "18 25 70",
                "",
                "light-to-temperature map:",
                "45 77 23",
                "81 45 19",
                "68 64 13",
                "",
                "temperature-to-humidity map:",
                "0 69 1",
                "1 0 69",
                "",
                "humidity-to-location map:",
                "60 56 37",
                "56 93 4"
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
