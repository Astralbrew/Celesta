namespace Astralbrew.Celesta.Compiler.AST
{
    public enum SyntaxTreeNodeType
    {
        Constant  = 1,
        Variable  =2,
        DataType = 3,
        Function = 4,
        Operator = 5,
        Assignment  =6,
        Block = 7,
        Conditional = 8,
        Loop = 9,
        RepeatN = 10
    }
}
