using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.Compile
{
    internal class CompileTimeFunction
    {
        public string Name { get; }
        public CompileTimeType[] ArgumentTypes { get; }
        public CompileTimeType ReturnType { get; }

        public CompileTimeFunction(string name, CompileTimeType[] argumentTypes, CompileTimeType returnType)
        {
            Name = name;
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CompileTimeFunction) || obj == null) 
                return false;
            var f = obj as CompileTimeFunction;
            return Name == f.Name
                && HasSameArgumentsWith(f)
                && ReturnType == f.ReturnType;            
        }

        public override int GetHashCode()
        {
            int hashCode = -1446926924;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<CompileTimeType[]>.Default.GetHashCode(ArgumentTypes);
            hashCode = hashCode * -1521134295 + EqualityComparer<CompileTimeType>.Default.GetHashCode(ReturnType);
            return hashCode;
        }

        public static bool operator ==(CompileTimeFunction f1, CompileTimeFunction f2) => f1.Equals(f2);        
        public static bool operator !=(CompileTimeFunction f1, CompileTimeFunction f2) => !f1.Equals(f2);

        public override string ToString()
        {
            return $"function {Name}({string.Join(", ", ArgumentTypes.Select(t => t.Name))})->{ReturnType.Name}";
        }

        public bool HasSameArgumentsWith(CompileTimeFunction other)
        {
            return ArgumentTypes.SequenceEqual(other.ArgumentTypes);
        }
    }
}
