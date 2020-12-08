using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace HandheldHalting
{
    class Program
    {
        public enum OpCode
        {
            Acc = 0,
            Jump = 1,
            NoOp = 2
        }

        [Serializable]
        public class Operation
        {
            public OpCode OpCode { get; set; }

            public int Operand { get; set; }

            public bool Executed { get; set; }
        }

        public class Accumulator
        {
            public long Value { get; set; }

            public void Clear()
            {
                Value = 0;
            }
        }

        static void Main(string[] args)
        {
            //var program = LoadProgram("TestData.txt");
            var program = LoadProgram("Input1.txt");
            var acc = new Accumulator();

            // Problem 1
            //var success = ExecuteProgram(program, acc);

            // Problem 2
            // We are going to modify every instruction that is a JMP to a NOP, and vice versa
            // and keep re-running the code until we find a winner
            for (int x = 0; x < program.Count; x++)
            {
                var copy = DeepClone(program);
                acc.Clear();

                switch (copy[x].OpCode)
                {
                    case OpCode.Jump:
                        copy[x].OpCode = OpCode.NoOp;
                        break;

                    case OpCode.NoOp:
                        copy[x].OpCode = OpCode.Jump;
                        break;

                    case OpCode.Acc:
                    default:
                        continue;
                }

                if(ExecuteProgram(copy, acc))
                    break;
            }

            Console.WriteLine($"Accumulator value = {acc}");
        }

        static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        static bool ExecuteProgram(List<Operation> program, Accumulator acc)
        {
            var pc = 0;
            var success = true;

            do
            {
                var op = program[pc];

                if (op.Executed)
                {
                    success = false;
                    break;
                }

                switch (op.OpCode)
                {
                    case OpCode.Acc:
                        acc.Value += op.Operand;
                        pc++;
                        break;

                    case OpCode.Jump:
                        pc += op.Operand;
                        break;

                    case OpCode.NoOp:
                        pc++;
                        break;

                    default:
                        throw new Exception("Guru meditation error!");
                }

                op.Executed = true;
            }
            while (pc < program.Count);

            return success;
        }

        static List<Operation> LoadProgram(string filename)
        {
            var program = new List<Operation>();
            var reader = default(StreamReader);
            var input = default(string);

            try
            {
                using (reader = new StreamReader(filename))
                {
                    while ((input = reader.ReadLine()) != null)
                    {
                        var pair = input.Split(" ");
                        var opCode = default(OpCode);

                        switch (pair[0])
                        {
                            case "nop":
                                opCode = OpCode.NoOp;
                                break;

                            case "acc":
                                opCode = OpCode.Acc;
                                break;

                            case "jmp":
                                opCode = OpCode.Jump;
                                break;

                            default:
                                throw new Exception("Invalid input");
                        }

                        program.Add(new Operation()
                        {
                            OpCode = opCode,
                            Operand = int.Parse(pair[1])
                        });
                    }

                    return program;
                }
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
