using Astralbrew.Celesta.Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.CelestaSyntaxTreeCompiler
{
    internal class MemoryPool
    {
        public readonly Dictionary<string,MemoryPoolRecord> Records = new();

        private void AddRecord(string name, string scope, int size, bool isReadOnly, ISyntaxTreeNode node)
        {
            Records[name] = new MemoryPoolRecord(name, scope, size, isReadOnly, node);
        }

        public void AddRecord(VariableNode variable)
        {
            var name = variable.ToString();
            int size = 4;
            //Console.WriteLine($"Var {name} => {variable.GetScope()}");
            AddRecord(name, variable.GetScope(), size, false, variable);             
        }

        public void AddRecord(ConstantNode constant)
        {
            var name = constant.ToString();
            int size = 4;
            var scope = constant.GetScope();
            if (scope == "")
            {
                scope = "@main";
                name += scope;
            }

            var aliases = Records.Where(r => r.Key.StartsWith(name)).ToList();

            if(aliases.Count>0)
            {
                aliases.ForEach(a => Records.Remove(a.Key));
                AddRecord(name, scope, size, true, constant);
                return;
            }

            aliases = Records.Where(r => name.StartsWith(r.Key)).ToList();
            if(aliases.Count>0)
            {
                return; // constant already exists in the scope
            }
            AddRecord(name, scope, size, true, constant);
        }

        private ScopeTree _ScopeTree = null;
        public int HeapStartOffset { get; private set; } = 0;

        public MemoryPoolRecord this[string name]
        {
            get
            {
                if (name.StartsWith("#"))
                {
                    var key = Records.Keys.Where(k => name.StartsWith(k)).FirstOrDefault();
                    if (key == null)
                        throw new ArgumentException($"No record with name '{name}'");
                    return Records[key];
                }
                return Records[name];
            }
        }
        public MemoryPoolRecord this[ConstantNode constant] => this[constant.ToString()];
        public MemoryPoolRecord this[VariableNode variable] => this[variable.ToString()];

        public void ResolveOffsets()
        {
            _ScopeTree = new ScopeTree();
            Records.Values.ToList().ForEach(_ScopeTree.Insert);
            HeapStartOffset = _ScopeTree.AssignOffsets();
            //Console.WriteLine(_ScopeTree);
        }

        public List<MemoryPoolRecord> this[BlockNode block, bool inDepth=false]
        {
            get
            {
                if (inDepth)
                    return Records.Values.Where(r => r.Scope.StartsWith(block.ScopeName)).ToList();
                return Records.Values.Where(r => r.Scope == block.ScopeName).ToList();
            }
        }

        class ScopeTree
        {
            public string ScopeName { get; }
            public List<MemoryPoolRecord> Records { get; } = new List<MemoryPoolRecord>();
            public List<ScopeTree> Children { get; } = new List<ScopeTree>();

            private int Offset = 0;

            public ScopeTree(string name = "")
            {
                ScopeName = name;                
            }            

            private ScopeTree GetScopedChild(string scope)
            {
                var result = Children.Where(c => c.ScopeName == scope).FirstOrDefault();
                if (result != null)
                    return result;
                result = new ScopeTree(scope);
                Children.Add(result);
                return result;
            }

            private void _Insert(MemoryPoolRecord record, string scope)
            {
                if(scope=="")
                {
                    Records.Add(record);
                    return;
                }
                var scopes = scope.Split('@').Where(s => s != "").ToArray();
                var firstScope = scopes[0];
                var nextScopes = string.Join("", scopes.Skip(1).Select(s => "@" + s));
                GetScopedChild(firstScope)._Insert(record, nextScopes);
            }

            public void Insert(MemoryPoolRecord record) => _Insert(record, record.Scope);

            public override string ToString()
            {
                var result = ScopeName + "\n";

                foreach (var r in Records)
                    result += $"  {r.Name} : {r.Offset}\n";

                foreach(var c in Children)
                {
                    result += string.Join("\n", c.ToString().Split('\n').Select(s => "  " + s)) + "\n";
                }
                return result;
            }

            public int AssignOffsets()
            {
                foreach(var r in Records)
                {
                    r.Offset = Offset;
                    Offset += r.Size;
                }

                int maxOffset = Offset;

                foreach (var c in Children) 
                {
                    c.Offset = Offset;
                    maxOffset = Math.Max(maxOffset, c.AssignOffsets());
                }

                return maxOffset;
            }
        }
    }

    internal class MemoryPoolRecord
    {
        public string Name { get; }

        public ISyntaxTreeNode Node { get; }

        public string Scope { get; }

        public int Size { get; }
        public bool IsReadOnly { get; }        
        public int Offset { get; set; }

        public MemoryPoolRecord(string name, string scope, int size, bool isReadOnly, ISyntaxTreeNode node)
        {
            Name = name;
            Scope = scope;
            Size = size;
            IsReadOnly = isReadOnly;
            Node = node;
        }

        public override string ToString() => $"({Name}:{Scope}:{Size}:{IsReadOnly})";
    }
}
