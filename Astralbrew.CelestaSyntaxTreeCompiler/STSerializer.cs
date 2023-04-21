using Astralbrew.Celesta.Compiler.AST;
using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace Astralbrew.CelestaSyntaxTreeCompiler
{
    internal class STSerializer
    {        
        Dictionary<DataTypeDefinition, byte> DataTypeIds = new Dictionary<DataTypeDefinition, byte>();

        Dictionary<string, byte> ScopeIds = new Dictionary<string, byte>();

        Dictionary<string, byte> VariableIds = new Dictionary<string, byte>();


        byte GetScopeId(string scopeName)
        {
            if(!ScopeIds.ContainsKey(scopeName))
            {
                ScopeIds[scopeName] = (byte)(ScopeIds.Count + 1);
            }
            return ScopeIds[scopeName];
        }

        byte GetVariableId(string variableName)
        {
            if(!VariableIds.ContainsKey(variableName))
            {
                VariableIds[variableName] = (byte)(VariableIds.Count + 1);
            }
            return VariableIds[variableName];
        }

        public void RegisterDataType(DataTypeDefinition dataType)
        {
            if (DataTypeIds.ContainsKey(dataType))
                return;
            DataTypeIds[dataType] = (byte)(DataTypeIds.Count + 1);
        }

        public SerializedTreeNode From(ISyntaxTreeNode treeNode)
        {
            SerializedTreeNode result = new SerializedTreeNode();

            result.NodeType = (byte)treeNode.Type;
            result.ScopeId = 0;
            result.OutputTypeId = DataTypeIds[treeNode.OutputType];

            if (treeNode.Type == SyntaxTreeNodeType.Block) 
            {
                var block = treeNode as BlockNode;
                result.ScopeId = GetScopeId(block.ScopeName);
                result.Children.AddRange(block.GetNodes().Select(n => From(n)));
            }
            else if(treeNode.Type == SyntaxTreeNodeType.Constant)
            {
                var constant = treeNode as ConstantNode;
                result.Metadata = ConstantToBytes(constant);
            }
            else if(treeNode.Type == SyntaxTreeNodeType.Variable)
            {
                var variable = treeNode as VariableNode;
                result.Metadata = BitConverter.GetBytes(GetVariableId(variable.Variable.Name));
            }
            else if(treeNode.Type == SyntaxTreeNodeType.Assignment)
            {
                var assign = treeNode as AssignNode;
                result.Children.Add(From(assign.RightHandSide));                
                result.Metadata = BitConverter.GetBytes(GetVariableId((assign.LeftHandSide as VariableNode).Variable.Name));
            }
            else
            {
                throw new ArgumentException($"Cannot serialize node of type {treeNode.Type}");
            }

            return result;
        }

        byte[] ConstantToBytes(ConstantNode constant)
        {
            if (constant.DataType == LanguageDefinition.PrimitiveTypes.Integer) 
            {
                return BitConverter.GetBytes(int.Parse(constant.Value));
            }

            if(constant.DataType == LanguageDefinition.PrimitiveTypes.String)
            {
                var strval = Regex.Unescape(constant.Value.Substring(1, constant.Value.Length - 2));
                return Encoding.ASCII.GetBytes(strval);
            }

            throw new ArgumentException($"Cannot serialize constant of type {constant.DataType}");
        }

        public byte[] ToBytes(SerializedTreeNode tn)
        {
            ushort offset = 0;            
            List<SerializedTreeNode> nodes = new List<SerializedTreeNode>();

            Queue<SerializedTreeNode> Q = new Queue<SerializedTreeNode>();
            Q.Enqueue(tn);

            while (Q.Count > 0)
            {
                var nd = Q.Dequeue();
                nd.Offset = offset;
                offset += nd.BytesLen;
                nodes.Add(nd);

                foreach (var c in nd.Children) 
                    Q.Enqueue(c);
            }

            var result = new byte[offset];

            foreach(var nd in nodes)
            {
                var bytes = nd.ToBytes();
                for (int i = 0; i < bytes.Length; i++)
                    result[nd.Offset + i] = bytes[i];
            }

            return result;
        }        
    }
}
