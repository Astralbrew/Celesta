using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Runtime.Modules
{
    public static class MathModule
    {
        static MathModule()
        {
            var Int = LanguageDefinition.PrimitiveTypes.Integer;

            _Module.RegisterFunction("abs", new DataTypeDefinition[] { Int }, Int, x => Math.Abs((int)x));
            _Module.RegisterFunction("sqr", new DataTypeDefinition[] { Int }, Int, x => (int)x * (int)x);
        }

        private static Module _Module = new Module();
        public static Module Module => _Module;
    }
}
