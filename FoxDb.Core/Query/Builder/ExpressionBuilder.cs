using FoxDb.Interfaces;

namespace FoxDb
{
    public abstract class ExpressionBuilder : FragmentBuilder, IExpressionBuilder
    {
        public string Alias { get; set; }
    }
}
