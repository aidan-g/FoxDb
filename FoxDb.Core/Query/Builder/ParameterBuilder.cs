using FoxDb.Interfaces;

namespace FoxDb
{
    public class ParameterBuilder : ExpressionBuilder, IParameterBuilder
    {
        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Parameter;
            }
        }

        public string Name { get; set; }
    }
}
