﻿using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using Astralbrew.Celesta.Utils;

namespace Astralbrew.Celesta.Compiler.AST
{
    public class ConstantNode : AbstractNode
    {
        public ConstantNode(string value, DataTypeDefinition dataType) : base(SyntaxTreeNodeType.Constant, dataType)
        {
            Value = value;            
        }

        public ConstantNode(string value) : this(value, GuessDataType(value)) 
        {            
        }

        public string Value { get; }
        public DataTypeDefinition DataType { get => OutputType; }

        private static DataTypeDefinition GuessDataType(string value)
        {
            if (int.TryParse(value, out int _))
                return LanguageDefinition.PrimitiveTypes.Integer;
            if (double.TryParse(value, out double _))
                return LanguageDefinition.PrimitiveTypes.Decimal;
            if (value == "true" || value == "false")
                return LanguageDefinition.PrimitiveTypes.Boolean;
            if (value.MatchesCStyleString())
                return LanguageDefinition.PrimitiveTypes.String;
            throw new ConstantIdentificationException($"Failed to guess data type for constant value '{value}'");
        }        

        public override string ToString() => "#"+Value+GetScope();       

        public string GetScope()
        {
            ISyntaxTreeNode node = this;
            while (node != null && !(node is BlockNode)) 
            {
                node = node.Parent;
            }

            if (node == null) return "";
            return (node as BlockNode).ScopeName;
        }
    }
}
