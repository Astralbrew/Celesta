namespace Astralbrew.CelestaSyntaxTreeCompiler.Assembler
{
    internal class Label : IAssemblyItem, ISymbol
    {
        public string Name { get; }

        public int Offset { get; set; }

        public Label(string name)
        {
            Name = name;
        }

        public override string ToString() => $".{Name} [0x{Offset:X4}]";
    }
}