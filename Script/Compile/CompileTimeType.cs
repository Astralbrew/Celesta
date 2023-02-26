using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.Compile
{
    public class CompileTimeType
    {
        public string Name { get; }
        public CompileTimeType(string name)
        {
            Name = name;
        }

        public static bool operator ==(CompileTimeType t1, CompileTimeType t2) => t1.Name == t2.Name;
        public static bool operator !=(CompileTimeType t1, CompileTimeType t2) => t1.Name != t2.Name;

        public static CompileTimeType Void => new CompileTimeType("Void");

        public override bool Equals(object obj)
        {
            return obj is CompileTimeType type && Name == type.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}
