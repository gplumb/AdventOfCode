using System;

namespace PassagePathing
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var data = GetDemoData1();
            //var data = GetDemoData2();
            var data = LoadFromFile("Input1.txt");

            //var result = Puzzle1(data);
            var result = Puzzle2(data);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(Graph graph)
        {
            var paths = graph.FindPaths("start", "end", false);
            return paths.Count;
        }

        static long Puzzle2(Graph graph)
        {
            var paths = graph.FindPaths("start", "end", true);
            return paths.Count;
        }

        static Graph GetDemoData1()
        {
            var data = new List<string>()
            {
                "start-A",
                "start-b",
                "A-c",
                "A-b",
                "b-d",
                "A-end",
                "b-end",
            };

            return ParseGraph(data);
        }

        static Graph GetDemoData2()
        {
            var data = new List<string>()
            {
                "dc-end",
                "HN-start",
                "start-kj",
                "dc-start",
                "dc-HN",
                "LN-dc",
                "HN-end",
                "kj-sa",
                "kj-HN",
                "kj-dc",
            };

            return ParseGraph(data);
        }


        static Graph ParseGraph(List<string> data)
        {
            var connections = new List<(string source, string dest)>();
            var result = new Graph();

            // Add nodes first
            foreach(var entry in data)
            {
                var conn = entry.Split("-");

                // This check is guarded, so it's safe to call without additional checking
                result.AddCave(conn[0]);
                result.AddCave(conn[1]);
                
                // We stash these for later so we can ensure we've discovered
                // all of the nodes first
                connections.Add((source: conn[0], dest: conn[1]));
            }

            // Now add vertices
            foreach (var conn in connections)
            {
                result.AddConnection(conn.source, conn.dest);
            }

            return result;
        }

        public class Cave
        {
            public string Data;

            public bool IsBig => char.IsUpper(Data[0]);

            public bool IsSmall => char.IsLower(Data[0]);


            public List<Cave> Neighbours = new List<Cave>();

            public Cave(string data)
            {
                Data = data;
            }
        }

        public class Path : List<Cave>
        {
            private Dictionary<string, int> SmallCaveCounts = new Dictionary<string, int>();
            private bool AllowMultiple = true;

            public Path()
            {
            }

            public Path(Path path)
            {
                foreach(var item in path)
                    base.Add(item);
            }

            public static Path New(Cave start)
            {
                return new Path() { start };
            }
            
            public Path Spawn(Cave cave)
            {
                var result = new Path(this);
                result.SmallCaveCounts = new Dictionary<string, int>(SmallCaveCounts);
                result.Add(cave);
                result.AllowMultiple = AllowMultiple;

                if (cave.IsSmall)
                {
                    if (result.SmallCaveCounts.ContainsKey(cave.Data))
                    {
                        result.SmallCaveCounts[cave.Data]++;

                        if (result.SmallCaveCounts[cave.Data] == 2)
                            result.AllowMultiple = false;
                    }
                    else
                    {
                        result.SmallCaveCounts[cave.Data] = 1;
                    }
                }

                return result;
            }


            public bool CanVisitCave(Cave cave)
            {
                if (cave.IsBig)
                    return true;

                if(SmallCaveCounts.ContainsKey(cave.Data))
                {
                    return AllowMultiple;
                }

                return true;
            }

            public override string ToString()
            {
                string result = "";
                
                for(int x = 0; x < Count; x++)
                {
                    result += this[x].Data;

                    if (x < Count - 1)
                        result += ",";
                }

                return result;
            }
        }

        public class Graph
        {
            private Dictionary<string, Cave> _vertices = new Dictionary<string, Cave>();


            /// <summary>
            /// Add a vertex to the graph
            /// </summary>
            public void AddCave(string data)
            {
                if (!_vertices.ContainsKey(data))
                {
                    _vertices.Add(data, new Cave(data));
                }
            }


            /// <summary>
            /// Add an edge to the graph
            /// </summary>
            public void AddConnection(string source, string dest)
            {
                _vertices[source].Neighbours.Add(_vertices[dest]);
                _vertices[dest].Neighbours.Add(_vertices[source]);
            }


            /// <summary>
            /// Find paths using BFS
            /// </summary>
            public HashSet<string> FindPaths(string source, string dest, bool limit)
            {
                var result = new HashSet<string>();
                var end = _vertices[dest];
                var start = _vertices[source];
                var completePath = "";

                var queue = new Queue<Path>();
                queue.Enqueue(Path.New(_vertices[source]));

                while (queue.Count > 0)
                {
                    var currentPath = queue.Dequeue();
                    var currentNode = currentPath.Last();

                    if (currentNode.Data.Equals(end.Data))
                    {
                        completePath = currentPath.ToString();

                        if (!result.Contains(completePath))
                        {
                            result.Add(completePath);
                            // Console.WriteLine(completePath);
                        }
                    }
                    else
                    {
                        foreach (var child in currentNode.Neighbours)
                        {
                            if (child.Data.Equals(start.Data))
                                continue;

                            // Do we have a small cave?
                            if (child.IsSmall)
                            {
                                // Skip over it if it already exists in this path
                                if (!limit && currentPath.Any(x => x.Data.Equals(child.Data)))
                                    continue;

                                if (limit && !currentPath.CanVisitCave(child))
                                    continue;
                            }

                            queue.Enqueue(currentPath.Spawn(child));
                        }
                    }
                }

                return result;
            }
        }

        static Graph LoadFromFile(string filename)
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

                    return ParseGraph(data);
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}