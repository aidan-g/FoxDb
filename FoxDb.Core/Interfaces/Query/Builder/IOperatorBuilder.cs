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
        And,
        AndAlso,
        Or,
        OrElse,
        OpenParentheses,
        CloseParentheses,
        //Mathmatical
        Add,
        //Other
        Null,
        Star
    }
}
