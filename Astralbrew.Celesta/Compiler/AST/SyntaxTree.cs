namespace Astralbrew.Celesta.Compiler.AST
{
    internal class SyntaxTree
    {
        public ISyntaxTreeNode Root { get; }
        public SyntaxTree(ISyntaxTreeNode root)
        {
            Root = root;
        }
    }
}
