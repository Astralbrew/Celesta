using Astralbrew.Celesta.Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Astralbrew.CelestaSyntaxTreeCompiler.Assembler
{
    internal class RomSymbol : ISymbol
    {
        public int Offset { get; set; }

        public int Size => ((Value.Length + 3) / 4) * 4;
        public string Name { get; }
        public byte[] Value { get; }

        public RomSymbol(string name, byte[] value)
        {
            Name = name;
            Value = value;
        }

        public static RomSymbol Make(string name, int val)
        {
            byte[] intBytes = BitConverter.GetBytes(val);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return new RomSymbol(name, intBytes);
        }

        public static RomSymbol Make(string name, string val)
        {
            using(var ms = new MemoryStream())
            {
                using(var bw=new BinaryWriter(ms))
                {
                    var bytes = Encoding.ASCII.GetBytes(val);
                    bw.Write((ushort)(bytes.Length + 1));
                    bw.Write(bytes);
                    bw.Write(0);
                    return new RomSymbol(name, ms.ToArray());
                }
            }            
        }

        public static RomSymbol Make(string name, ConstantNode constant)
        {
            var value = constant.Value;

            if (value[0]=='"')
            {
                return Make(name, Regex.Unescape(value.Substring(1, value.Length - 2)));
            }
            if (int.TryParse(value, out int nb)) 
            {
                return Make(name, nb);
            }

            if(value=="True")
            {
                return Make(name, 1);
            }

            if(value=="False")
            {
                return Make(name, 0);
            }
            throw new ArgumentException($"Invalid constant: {name}, {constant} ");
        }

        public override string ToString()
        {
            return $"rom:{Name} [0x{Offset:X4}]";
        }
    }
}
