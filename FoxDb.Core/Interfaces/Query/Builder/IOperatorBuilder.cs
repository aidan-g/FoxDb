namespace FoxDb.Interfaces
{
    public interface IOperatorBuilder : IExpressionBuilder
    {
        QueryOperator Operator { get; set; }
    }

    public enum QueryOperator : byte
    {
        None,
        Not,
        Equal,
        NotEqual,
        Greater,
        Less,
        And,
        AndAlso,
        Or,
        OrElse,
        OpenParentheses,
        CloseParentheses,
        Null,
        Star
    }
}
