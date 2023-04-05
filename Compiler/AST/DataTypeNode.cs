using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Data.SymbolDefinitions;

namespace Astralbrew.Celesta.Compiler.AST
{
    internal class DataTypeNode : AbstractNode
    {
        public DataTypeNode(DataTypeDefinition dataType) : base(SyntaxTreeNodeType.DataType, LanguageDefinition.PrimitiveTypes.DataType)
        {
            DataType = dataType;
        }

        public DataTypeDefinition DataType { get; }
        
        public override string ToString() => DataType.Name;
    }
}