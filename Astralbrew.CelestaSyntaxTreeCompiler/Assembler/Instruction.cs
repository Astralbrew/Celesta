namespace Astralbrew.CelestaSyntaxTreeCompiler.Assembler
{
    internal class Instruction : IAssemblyItem
    {
        public byte Opcode { get; }
        public ISymbol Operand1 { get; }
        public ISymbol Operand2 { get; }
        public Instruction(byte opcode, ISymbol operand1, ISymbol operand2)
        {
            Opcode = opcode;
            Operand1 = operand1;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
            Operand2 = operand2;
        }

        public Instruction(string instr, ISymbol operand1 = null, ISymbol operand2 = null)
        {
            Opcode = Opcodes.GetOpcode(instr);
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public override string ToString()
        {
            var result = Opcodes.GetInstructionName(Opcode).PadRight(14, ' ');

            if(Operand1!=null)
            {
                result += " " + Operand1.ToString();
                if (Operand2 != null)
                {
                    result += ", " + Operand2.ToString();
                }
            }

            return "    " + result;
        }

        public uint GetCode()
        {
            var offset1 = Operand1?.Offset ?? 0;
            var offset2 = Operand2?.Offset ?? 0;
            if (offset1 >= 4096 || offset2 >= 4096)
                throw new ArgumentException("Invalid offset");
            return (uint)((Opcode << 24) | (offset1 << 12) | offset2);
        }
    }
}
