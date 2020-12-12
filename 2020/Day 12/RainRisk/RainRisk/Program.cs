using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RainRisk
{
    class Program
    {
        public enum Action
        {
            MoveNorth,
            MoveSouth,
            MoveEast,
            MoveWest,

            TurnLeft,
            TurnRight,
            MoveForward
        }


        static void Main(string[] args)
        {
            //var nav = LoadNavigation("TestData.txt");
            var nav = LoadNavigation("Input1.txt");

            // Puzzle 1
            //var ship = new Ship(90);
            //ship.Process(nav, false);

            // Puzzle 2
            var ship = new Ship(90, 10, 1);
            ship.ProcessWithWaypoint(nav, false);

            Console.WriteLine(ship.Manhattan);
        }

        public class Ship
        {
            private int rotation = 0;   // Direction of the ship
            private int x = 0;          // Represents east/west
            private int y = 0;          // Represents north/south

            public int wx = 0;          // Represents waypoint east/west
            public int wy = 0;          // Represents waypoint north/south

            public Ship(int rotation)
            {
                this.rotation = rotation;
            }

            public Ship(int rotation, int waypointX, int waypointY)
            {
                this.rotation = rotation;
                wx = waypointX;
                wy = waypointY;
            }

            public int Manhattan => Math.Abs(x) + Math.Abs(y);

            public void Process(List<Tuple<Action, int>> navigation, bool debug)
            {
                foreach (var item in navigation)
                {
                    switch (item.Item1)
                    {
                        case Action.MoveNorth: y += item.Item2; break;
                        case Action.MoveSouth: y -= item.Item2; break;
                        case Action.MoveEast:  x += item.Item2; break;
                        case Action.MoveWest:  x -= item.Item2; break;

                        case Action.TurnLeft:
                            for (int r = 0; r < ((item.Item2 + 1) / 90); r++)
                            {
                                rotation -= 90;
                                rotation = (rotation == -90) ? 270 : rotation;
                            }
                            break;

                        case Action.TurnRight:
                            for (int r = 0; r < ((item.Item2 + 1) / 90); r++)
                            {
                                rotation += 90;
                                rotation = (rotation > 270) ? 0 : rotation;
                            }
                            break;

                        case Action.MoveForward:
                            // This only works because we rotation in 90 degree increments
                            switch (rotation)
                            {
                                case 0: y += item.Item2; break;
                                case 90: x += item.Item2; break;
                                case 180: y -= item.Item2; break;
                                case 270: x -= item.Item2; break;
                                default:
                                    throw new Exception("Doh!");
                            }
                            break;

                        default:
                            break;
                    }

                    if(debug)
                        Console.WriteLine($"{item.Item1} {item.Item2}: {PrintCurrent(true)}");
                }
            }

            public void ProcessWithWaypoint(List<Tuple<Action, int>> navigation, bool debug)
            {
                foreach (var item in navigation)
                {
                    switch (item.Item1)
                    {
                        case Action.MoveNorth: wy += item.Item2; break;
                        case Action.MoveSouth: wy -= item.Item2; break;
                        case Action.MoveEast:  wx += item.Item2; break;
                        case Action.MoveWest:  wx -= item.Item2; break;

                        case Action.TurnLeft:
                            for (int r = 0; r < ((item.Item2 + 1) / 90); r++)
                            {
                                var oldWx = wx;
                                var oldWy = wy;

                                wx = -oldWy;
                                wy = oldWx;
                            }
                            break;

                        case Action.TurnRight:
                            for (int r = 0; r < ((item.Item2 + 1) / 90); r++)
                            {
                                var oldWx = wx;
                                var oldWy = wy;

                                wx = oldWy;
                                wy = -oldWx;
                            }
                            break;

                        case Action.MoveForward:
                            x += (wx * item.Item2);
                            y += (wy * item.Item2);
                            break;

                        default:
                            break;
                    }

                    if (debug)
                        Console.WriteLine($"{item.Item1} {item.Item2}: The ship is {PrintCurrent(false)}. The waypoint is: {PrintWaypoint()}");
                }
            }

            public string PrintCurrent(bool includeRotation)
            {
                var builder = new StringBuilder();

                if (includeRotation)
                {
                    switch (rotation)
                    {
                        case 0: builder.Append("facing north, "); break;
                        case 90: builder.Append("facing east, "); break;
                        case 180: builder.Append("facing south, "); break;
                        case 270: builder.Append("facing west, "); break;
                    }
                }

                if(x < 0)
                {
                    builder.Append($"west {Math.Abs(x)}, ");
                }
                else
                {
                    builder.Append($"east {Math.Abs(x)}, ");
                }

                if (y < 0)
                {
                    builder.Append($"south {Math.Abs(y)}");
                }
                else
                {
                    builder.Append($"north {Math.Abs(y)}");
                }

                return builder.ToString();
            }


            public string PrintWaypoint()
            {
                var builder = new StringBuilder();

                if (wx < 0)
                {
                    builder.Append($"west {Math.Abs(wx)}, ");
                }
                else
                {
                    builder.Append($"east {Math.Abs(wx)}, ");
                }

                if (wy < 0)
                {
                    builder.Append($"south {Math.Abs(wy)}");
                }
                else
                {
                    builder.Append($"north {Math.Abs(wy)}");
                }

                return builder.ToString();
            }
        }

        static List<Tuple<Action, int>> LoadNavigation(string filename)
        {
            var reader = default(StreamReader);
            var input = default(string);
            var result = new List<Tuple<Action, int>>();

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        var action = Action.MoveForward;
                        var value = int.Parse(input[1..]);

                        switch (char.ToUpperInvariant(input[0]))
                        {
                            case 'N': action = Action.MoveNorth; break;
                            case 'S': action = Action.MoveSouth; break;
                            case 'E': action = Action.MoveEast; break;
                            case 'W': action = Action.MoveWest; break;
                            case 'L': action = Action.TurnLeft; break;
                            case 'R': action = Action.TurnRight; break;
                            case 'F': action = Action.MoveForward; break;

                            default:
                                throw new Exception("Unknown value");
                        }

                        // Assuming the input is always going to be 90 degrees...
                        if ((input[0] == 'L' || input[0] == 'R') && (value == 0 || value % 90 > 0))
                            throw new Exception("Nooooo!");
                        
                        result.Add(new Tuple<Action, int>(action, int.Parse(input[1..])));
                    }

                    return result;
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
