using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.CelestaSyntaxTreeCompiler.Assembler
{
    internal static class Opcodes
    {
        private static readonly string[] StackInstructions = new string[]
        {
            "nop", // 0

            "add", // 1
            "sub", // 2
            "mul", // 3
            "div", // 4
            "mod", // 5

            "eq",  // 6
            "neq", // 7
            "lt",  // 8
            "le",  // 9
            "gt",  // 10
            "ge",  // 11

            "and", // 12
            "or",  // 13

            "strcat",  // 14
            "strcln", // 15
            
            "dec", // 16
            "zero", // 17

            "strfix", // 18
        };

        private static readonly string[] NonStackInstructions = new string[]
        {
            "call",     // 0
            "ret",      // 1
            "jmp",      // 2
            "jt",       // 3
            "jf",       // 4
            "mov",      // 5
            "syscall",  // 6
            "alloc",    // 7
            "dealloc",  // 8
            "strld",    // 9
            "ld",       // 11            
            "exit",     // 12
        };        

        public static byte GetOpcode(string instruction)
        {
            try
            {
                var tokens = instruction.Split(' ');

                byte modifier = 0;
                byte instrCode = 0;
                bool isStack = true;
                int instrIndex = 0;

                if (tokens[0] == "push")
                {
                    modifier = 0;
                    instrIndex = 1;
                }
                else if (tokens[0] == "stack")
                {
                    modifier = 1;
                    instrIndex = 1;
                }
                else if (tokens[0] == "hybrid")
                {
                    modifier = 2;
                    instrIndex = 1;
                }
                else if (tokens[0] == "pop")
                {
                    if (tokens.Length > 1 && tokens[1] == "test") 
                    {
                        modifier = 4;
                        instrIndex = 2;
                    }
                    else
                    {
                        modifier = 3;
                        instrIndex = 1;
                    }
                }
                else if (tokens[0] == "peek")
                {
                    if (tokens.Length > 1 && tokens[1] == "test") 
                    {
                        modifier = 6;
                        instrIndex = 2;
                    }
                    else
                    {
                        modifier = 5;
                        instrIndex = 1;
                    }
                }
                else
                {
                    modifier = 7;
                    instrIndex = 0;
                    isStack = false;
                }

                modifier <<= 5;

                if (instrIndex < tokens.Length)
                {
                    var instrName = tokens[instrIndex];

                    if (isStack)
                    {
                        if (!StackInstructions.Contains(instrName))
                            throw new Exception();
                        instrCode = (byte)Array.IndexOf(StackInstructions, instrName);
                    }
                    else
                    {
                        if (!NonStackInstructions.Contains(instrName))
                            throw new Exception();
                        instrCode = (byte)Array.IndexOf(NonStackInstructions, instrName);
                    }
                }

                if (instrIndex < tokens.Length - 1) 
                    throw new Exception();

                return (byte)(modifier | instrCode);
            }
            catch
            {
                throw new ArgumentException($"Invalid instruction: {instruction}");
            }
        }

        public static string GetInstructionName(byte opcode)
        {
            try
            {
                var modifiers = new string[] { "push", "stack", "hybrid", "pop", "pop test", "peek", "peek test" };
                var modifier = (opcode & 0b11100000) >> 5;
                var instrCode = opcode & 31;
                string name = "";
                if (modifier != 7)
                {
                    name = modifiers[modifier];
                    if (instrCode != 0)
                        name += " " + StackInstructions[instrCode];
                }
                else
                {
                    name += NonStackInstructions[instrCode];
                }
                return name;
            }
            catch
            {
                throw new ArgumentException($"Invalid opcode : 0x{opcode:X2}");
            }
        }


    }
}
