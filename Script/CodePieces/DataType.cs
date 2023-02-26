using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class DataType : ICodePiece
    {
        public string Name;

        public DataType(string name)
        {
            Name = name;
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return new CompileTimeType("DataType");
        }

        public override string ToString()
        {
            return $"TypeId:{Name}";
        }
    }
}
