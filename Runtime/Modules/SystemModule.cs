using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Runtime.Modules
{
    internal static class SystemModule
    {
        static SystemModule()
        {
            var Str = LanguageDefinition.PrimitiveTypes.String;
            var Int = LanguageDefinition.PrimitiveTypes.Integer;            
            var Void = LanguageDefinition.PrimitiveTypes.Void;

            _Module.RegisterFunction("load", new DataTypeDefinition[] { Str }, Void, path =>
            {
                var interpreter = new Interpreter(RuntimeContext.DefaultRuntimeContext);
                interpreter.Evaluate(File.ReadAllText((string)path));
                return null;
            });

            _Module.RegisterFunction("print", new DataTypeDefinition[] { Str }, Void, val =>
            {
                Console.Write(val);
                return null;
            });

            _Module.RegisterFunction("readint", new DataTypeDefinition[0], Int, () =>
            {
                return int.Parse(Console.ReadLine());
            });


        }

        private static Module _Module = new Module();
        public static Module Module => _Module;
    }
}
