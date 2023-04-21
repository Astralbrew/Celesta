using Astralbrew.Celesta.Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.CelestaSyntaxTreeCompiler
{
    internal class SerializedTreeNode
    {
        public byte NodeType { get; set; }
        public byte ScopeId { get; set; }
        public byte OutputTypeId { get; set; }
        public List<SerializedTreeNode> Children { get; set; } = new List<SerializedTreeNode>();
        public byte[] Metadata { get; set; } = new byte[0];

        
        public ushort Offset { get; set; }

        public ushort BytesLen => (ushort)(3 + 1 + 2 * Children.Count + Metadata.Length);

        public byte[] ToBytes()
        {
            using(var ms=new MemoryStream())
            {
                using(var bw = new BinaryWriter(ms))
                {
                    bw.Write(NodeType);
                    bw.Write(ScopeId);
                    bw.Write(OutputTypeId);
                    bw.Write((byte)Children.Count);
                    foreach(var c in Children)
                    {
                        bw.Write(c.Offset);
                    }
                    bw.Write(Metadata);
                }
                return ms.ToArray();
            }
        }
    }
}
