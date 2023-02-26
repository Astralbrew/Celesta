using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal sealed class CodeBlock : ICodePiece
    {
        public ICodePiece[] Content { get; internal set; }

        public CodeBlock(params ICodePiece[] content)
        {
            Content = content;
            Flatten();
        }

        public void Flatten()
        {
            List<ICodePiece> pieces = new List<ICodePiece>();

            foreach (ICodePiece piece in Content)
            {
                if (piece.GetType() == typeof(CodeBlock))
                {
                    var p = (piece as CodeBlock);
                    p.Flatten();
                    pieces.AddRange(p.Content);
                }
                else pieces.Add(piece);
            }

            Content = pieces.ToArray();
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context) => CompileTimeType.Void;

        public override string ToString()
        {
            return string.Join("\n", Content.ToList().Select(c => c.ToString()));
        }
    }
}
