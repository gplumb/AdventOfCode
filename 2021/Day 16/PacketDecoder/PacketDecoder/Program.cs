using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PacketDecoder
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region Test data

            //var demoPacket1 = Decode("D2FE28");
            //var demoPacket2 = Decode("38006F45291200");
            //var demoPacket3 = Decode("EE00D40C823060");

            //var demoPacket4 = Decode("8A004A801A8002F478");
            //var sum = demoPacket4.SumVersions();

            //var demoPacket5 = Decode("620080001611562C8802118E34");
            //var sum = demoPacket5.SumVersions();

            //var demoPacket6 = Decode("C0015000016115A2E0802F182340");
            //var sum = demoPacket6.SumVersions();

            //var demoPacket7 = Decode("A0016C880162017C3686B18A3D4780");
            //var sum = demoPacket7.SumVersions();



            //var demoPacket8 = Decode("C200B40A82");
            //var result = demoPacket8.Execute();

            //var demoPacket9 = Decode("04005AC33890");
            //var result = demoPacket9.Execute();

            //var demoPacket10 = Decode("880086C3E88112");
            //var result = demoPacket10.Execute();

            //var demoPacket11 = Decode("CE00C43D881120");
            //var result = demoPacket11.Execute();

            //var demoPacket12 = Decode("D8005AC2A8F0");
            //var result = demoPacket12.Execute();

            //var demoPacket13 = Decode("F600BC2D8F");
            //var result = demoPacket13.Execute();

            //var demoPacket14 = Decode("9C005AC2F8F0");
            //var result = demoPacket14.Execute();

            //var demoPacket15 = Decode("9C005AC2F8F0");
            //var result = demoPacket15.Execute();

            #endregion

            var data = LoadFromFile("Input1.txt");

            //var result = Puzzle1(data);
            var result = Puzzle2(data);

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        static long Puzzle1(List<string> data)
        {
            return Decode(data[0]).SumVersions();
        }

        static long Puzzle2(List<string> data)
        {
            return Decode(data[0]).Execute();
        }

        static Dictionary<char, string> HexToBinaryMap = new Dictionary<char, string>()
        {
            { '0', "0000" }, { '1', "0001" }, { '2', "0010" }, { '3', "0011" },
            { '4', "0100" }, { '5', "0101" }, { '6', "0110" }, { '7', "0111" },
            { '8', "1000" }, { '9', "1001" }, { 'A', "1010" }, { 'B', "1011" },
            { 'C', "1100" }, { 'D', "1101" }, { 'E', "1110" }, { 'F', "1111" }
        };

        public enum OpType
        {
            Sum = 0,
            Product = 1,
            Minimum = 2,
            Maximum = 3,
            Literal = 4,
            GreaterThan = 5,
            LessThan = 6,
            Equals = 7,
        }

        public class Packet
        {
            public int Version { get; set; }

            public OpType TypeId { get; set; }

            public long Value { get; set; }

            public List<Packet> SubPackets = new List<Packet>();

            public long SumVersions()
            {
                return AddVersions(this);
            }

            public long AddVersions(Packet packet)
            {
                long result = packet.Version;

                foreach(var child in packet.SubPackets)
                {
                    result += AddVersions(child);
                }

                return result;
            }

            public long Execute()
            {
                switch (TypeId)
                {
                    case OpType.Sum:
                        return SubPackets.Select(x => x.Execute()).Sum();

                    case OpType.Product:
                        return SubPackets.Select(x => x.Execute()).Aggregate((x, y) => x * y);

                    case OpType.Minimum:
                        return SubPackets.Select(x => x.Execute()).Min();

                    case OpType.Maximum:
                        return SubPackets.Select(x => x.Execute()).Max();

                    case OpType.Literal:
                        return Value;

                    case OpType.GreaterThan:
                        return SubPackets[0].Execute() > SubPackets[1].Execute() ? 1L : 0L;

                    case OpType.LessThan:
                        return SubPackets[0].Execute() < SubPackets[1].Execute() ? 1L : 0L;

                    case OpType.Equals:
                        return SubPackets[0].Execute() == SubPackets[1].Execute() ? 1L : 0L;

                    default:
                        throw new Exception("Guru meditation y'all!");
                }
            }
        }

        static Packet Decode(string data)
        {
            var binary = HexToBinary(data);
            var result = ParsePacket(binary);
            return result.packet;
        }

        static string HexToBinary(string data)
        {
            var builder = new StringBuilder();

            foreach (char c in data)
                builder.Append(HexToBinaryMap[c]);

            return builder.ToString();
        }

        static (string remaining, Packet packet) ParsePacket(string data)
        {
            var i = 0;

            var result = new Packet()
            {
                Version = Convert.ToInt32(data[0..3], 2),
                TypeId = (OpType)Convert.ToInt32(data[3..6], 2)
            };

            // Are we dealing with a literal?
            if (result.TypeId == OpType.Literal)
            {
                var builder = new StringBuilder();

                // Fetch groups of 5
                for (i = 6; ; i += 5)
                {
                    var part = data[i..(i + 5)];
                    builder.Append(part[1..5]);

                    // Do we have our last part?
                    if (part[0] == '0')
                    {
                        break;
                    }
                }

                result.Value = Convert.ToInt64(builder.ToString(), 2);
                return (data[(i + 5)..], result);
            }

            // No, we're dealing with an operator
            // Read length type id
            var ltid = data[6..7];
            var subPacketData = "";
            var remainder = "";

            if (ltid == "0")
            {
                // Next 15 bits are a number that represents the total length in bits of the sub-packets
                var subPacketLength = Convert.ToInt32(data[7..22], 2);
                subPacketData = data[22..(22 + subPacketLength)];
                remainder = data[(22 + subPacketLength)..];

                do
                {
                    var child = ParsePacket(subPacketData);
                    result.SubPackets.Add(child.packet);
                    subPacketData = child.remaining;
                }
                while (subPacketData.Length > 0);
            }
            else
            {
                // Next 11 bits are a number that represents the number of sub-packets immediately contained by this packet
                var subPackets = Convert.ToInt32(data[7..18], 2);
                subPacketData = data[18..];

                for (i = 0; i < subPackets; i++)
                {
                    var child = ParsePacket(subPacketData);
                    result.SubPackets.Add(child.packet);
                    subPacketData = child.remaining;
                }

                remainder = subPacketData;
            }

            return (remainder, result);
        }


        static List<string> GetDemoData()
        {
            var data = new List<string>()
            {
                "1163751742",
                "1381373672",
                "2136511328",
                "3694931569",
                "7463417111",
                "1319128137",
                "1359912421",
                "3125421639",
                "1293138521",
                "2311944581",
            };

            return data;
        }

        static List<string> LoadFromFile(string filename)
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