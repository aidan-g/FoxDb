namespace FoxDb.Interfaces
{
    public interface IOperatorBuilder : IExpressionBuilder
    {
        QueryOperator Operator { get; set; }
    }

    public enum QueryOperator : byte
    {
        None,
        //Logical
        Not,
        Is,
        In,
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        Match,
        And,
        AndAlso,
        Or,
        OrElse,
        OpenParentheses,
        CloseParentheses,
        Between,
        //Mathmatical
        Plus,
        Minus,
        //Other
        Null,
        Star,
        Concat
    }
}
