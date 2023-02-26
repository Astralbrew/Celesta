using Astralbrew.Celesta.Script.CodePieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.Compile
{
    internal class CompileTimeContext
    {
        internal HashSet<CompileTimeType> DataTypes = new HashSet<CompileTimeType>();
        internal Dictionary<string, CompileTimeType> Variables = new Dictionary<string, CompileTimeType>();        
        internal Dictionary<
            (string Operator, CompileTimeType FirstMemberType, CompileTimeType SecondMemberType) 
            , CompileTimeType> 
            BinaryOperators = new Dictionary<(string, CompileTimeType, CompileTimeType), CompileTimeType>();

        internal Dictionary<(string Operator, CompileTimeType MemberType), CompileTimeType>
            UnaryOperators = new Dictionary<(string, CompileTimeType), CompileTimeType>();

        internal List<CompileTimeFunction> Functions = new List<CompileTimeFunction>();

        internal List<(CompileTimeType FromType, CompileTimeType ToType)> 
            Converters = new List<(CompileTimeType, CompileTimeType)>();

        public CompileTimeContext()
        {
            RegisterDataTypes("Void", "Integer", "String", "Decimal");

            RegisterConverter("Integer", "Decimal");
            RegisterConverter("Decimal", "Integer");

            RegisterUnaryOperator("+", "Integer", "Integer");
            RegisterUnaryOperator("-", "Integer", "Integer");

            RegisterUnaryOperator("+", "Decimal", "Decimal");
            RegisterUnaryOperator("-", "Decimal", "Decimal");

            RegisterBinaryOperator("+", "Integer", "Integer", "Integer");
            RegisterBinaryOperator("-", "Integer", "Integer", "Integer");
            RegisterBinaryOperator("*", "Integer", "Integer", "Integer");
            RegisterBinaryOperator("/", "Integer", "Integer", "Integer");

            RegisterBinaryOperator("+", "Decimal", "Decimal", "Decimal");
            RegisterBinaryOperator("-", "Decimal", "Decimal", "Decimal");
            RegisterBinaryOperator("*", "Decimal", "Decimal", "Decimal");

            RegisterBinaryOperator("+", "Integer", "Decimal", "Decimal");
            RegisterBinaryOperator("-", "Integer", "Decimal", "Decimal");
            RegisterBinaryOperator("*", "Integer", "Decimal", "Decimal");

            RegisterBinaryOperator("+", "Decimal", "Integer", "Decimal");
            RegisterBinaryOperator("-", "Decimal", "Integer", "Decimal");
            RegisterBinaryOperator("*", "Decimal", "Integer", "Decimal");
        }

        public void RegisterDataType(string typeName)
        {
            DataTypes.Add(new CompileTimeType(typeName));
        }

        public void RegisterVariable(string varName, string typeName)
        {
            if (Variables.ContainsKey(varName))
                throw new ArgumentException($"Duplicate declaration of variable '{varName}'");
            ValidateType(typeName);

            Variables[varName] = new CompileTimeType(typeName);
        }

        public void RegisterUnaryOperator(string opName, string typeName, string returnTypeName)
        {
            ValidateType(typeName);
            ValidateType(returnTypeName);

            var mType = new CompileTimeType(typeName);
            var rType = new CompileTimeType(returnTypeName);

            if(UnaryOperators.ContainsKey((opName, mType)))
            {
                throw new ArgumentException($"Multiple definition of operator {opName}({typeName})");
            }

            UnaryOperators[(opName, mType)] = rType;
        }

        public void RegisterBinaryOperator(string opName, string t1Name, string t2Name, string returnTypeName)
        {
            ValidateType(t1Name);
            ValidateType(t2Name);
            ValidateType(returnTypeName);

            var m1Type = new CompileTimeType(t1Name);
            var m2Type = new CompileTimeType(t2Name);
            var rType = new CompileTimeType(returnTypeName);

            if (BinaryOperators.ContainsKey((opName, m1Type, m2Type)))
            {
                throw new ArgumentException($"Multiple definition of operator {opName}({t1Name}, {t2Name})");
            }

            BinaryOperators[(opName, m1Type, m2Type)] = rType;
        }

        public void RegisterDataTypes(params string[] typeNames)
        {
            typeNames.Select(t => new CompileTimeType(t)).ToList().ForEach(t => DataTypes.Add(t));            
        }

        public void RegisterConverter(string from, string to)
        {
            ValidateType(from);
            ValidateType(to);
            Converters.Add((new CompileTimeType(from), new CompileTimeType(to)));
        }

        public bool IsValidType(string type) => DataTypes.Contains(new CompileTimeType(type));

        private void ValidateType(string type)
        {
            if (!IsValidType(type))
                throw new ArgumentException($"Undefined type '{type}'");
        }

        public bool ExistsConverter(CompileTimeType from, CompileTimeType to)
        {
            return from == to || Converters.Any(c => c.FromType == from && c.ToType == to);
        }

        public CompileTimeType GetVariableType(string varName)
        {
            if (!Variables.ContainsKey(varName))
                throw new ArgumentException($"Undefined variable \"{varName}\"");
            return Variables[varName];
        }

        public CompileTimeType GetOperatorType(string opName, CompileTimeType firstType, CompileTimeType secondType)
        {
            if (!BinaryOperators.ContainsKey((opName, firstType, secondType)))
                throw new ArgumentException($"Undefined operator {opName} ({firstType},{secondType})");
            return BinaryOperators[(opName, firstType, secondType)];
        }

        public CompileTimeType GetOperatorType(string opName, CompileTimeType type)
        {
            if (!UnaryOperators.ContainsKey((opName, type)))
                throw new ArgumentException($"Undefined operator {opName} ({type})");
            return UnaryOperators[(opName, type)];
        }

        public CompileTimeType GetFunctionType(string funcName, CompileTimeType[] argTypes)
        {
            var type = Functions.Where(f => f.Name == funcName
                && (
                    EqualityComparer<CompileTimeType[]>.Default.Equals(f.ArgumentTypes, argTypes)
                    || argTypes.Zip(f.ArgumentTypes, (from, to) => (from, to)).All(t => ExistsConverter(t.from, t.to)))
                )
                .FirstOrDefault().ReturnType;
            if(type==null)
            {
                throw new ArgumentException($"Undefined function " +
                    $"{funcName}({string.Join(", ", argTypes.Select(a => a.Name))})");
            }
            return type;
        }

        public void RegisterFunction(string funcName, string[] argTypeNames, string retTypeName)
        {          
            argTypeNames.ToList().ForEach(ValidateType);
            ValidateType(retTypeName);

            CompileTimeFunction fun = new CompileTimeFunction(funcName
                , argTypeNames.Select(t => new CompileTimeType(t)).ToArray(), new CompileTimeType(retTypeName));
          
            if (Functions.Any(f=>f.Name==fun.Name && f.HasSameArgumentsWith(fun)))
            {
                throw new ArgumentException($"Multiple definition of function " +
                    $"{fun.Name}({string.Join(", ", fun.ArgumentTypes.Select(t => t.Name))})");
            }
            Functions.Add(fun);
        }

        public IEnumerable<string> DumpFunctions() => Functions.Select(f => f.ToString());

        public bool IsFunction(string name)
        {
            return Functions.Any(f => f.Name == name);
        }
        

    }
}
