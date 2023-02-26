namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    public enum InterpretedTokenType
    {
        Integer,
        Decimal,
        String,                

        Operator,        

        LeftParen,
        RightParen,

        Keyword,

        MethodCallPeriod,
        Semicolon,
        Comma,
        
        Symbol,        

        InstructionSeparator
    }
}