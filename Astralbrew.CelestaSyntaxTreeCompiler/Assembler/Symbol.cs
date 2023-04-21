using Astralbrew.Celesta.Compiler.AST;

namespace Astralbrew.CelestaSyntaxTreeCompiler.Assembler
{
    internal class Symbol : ISymbol
    {
        public string Name { get; }
        public int Offset { get; set; }

        public bool ReadOnly { get; }

        public Symbol(string name, bool readOnly)
        {
            Name = name;
            ReadOnly = readOnly;
        }

        public Symbol(ConstantNode c) : this(c.ToString(), true) { }
        public Symbol(VariableNode v) : this(v.ToString(), false) { }

        public override string ToString()
        {
            return $"{Name} [0x{Offset:X4}]";
        }
    }
}
