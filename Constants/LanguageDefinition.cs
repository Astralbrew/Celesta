using Astralbrew.Celesta.Data;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Astralbrew.Celesta.Constants
{
    public static class LanguageDefinition
    {
        public static readonly string[] Operators = new string[]
        {
            "+", "-", "*", "/", "%", "==", "!=", "<", ">", "<=", ">=", "and", "or", "xor"
        };

        public static readonly string[] Keywords = new string[]
        {
            "if", "then", "else", "endif", "while", "do", "endwhile", "for", "endfor", "end"
        };

        public static class PrimitiveTypes
        {
            public static DataTypeDefinition DataType => new DataTypeDefinition("datatype");
            public static DataTypeDefinition Integer => new DataTypeDefinition("int", (int)0);
            public static DataTypeDefinition Decimal => new DataTypeDefinition("decimal", (double)0);
            public static DataTypeDefinition String => new DataTypeDefinition("string", "");
            public static DataTypeDefinition Boolean => new DataTypeDefinition("bool", false);
            public static DataTypeDefinition Void => new DataTypeDefinition("void", new NoOutput());
            public static DataTypeDefinition Object => new DataTypeDefinition("object");

            public static IEnumerable<DataTypeDefinition> Enumerate()
                => typeof(PrimitiveTypes).GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .Where(p => p.PropertyType == typeof(DataTypeDefinition))
                    .Select(p => p.GetValue(null) as DataTypeDefinition);
        }

        public static int Priority(string op, bool isUnary)
        {
            bool IsSymbol(params string[] vals) => vals.Contains(op);
            if (!isUnary)
            {
                if (IsSymbol("=")) return 20;
                if (IsSymbol("or")) return 30;
                if (IsSymbol("and")) return 40;
                if (IsSymbol("not")) return 50;
                if (IsSymbol("<", ">", "<=", ">=", "==", "!=")) return 60;
                if (IsSymbol("+", "-")) return 70;
                if (IsSymbol("*", "/", "*")) return 80;
            }
            else
            {
                if (IsSymbol("+", "-")) return 90;
            }            
            throw new ArgumentException("Unknown operator");
        }

        public static int PeriodPriority => 100;
        public static int FunctionPriority => 110;
    }
}
