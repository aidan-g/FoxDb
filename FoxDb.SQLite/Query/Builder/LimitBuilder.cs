using FoxDb.Interfaces;

namespace FoxDb
{
    public class LimitBuilder : FragmentBuilder, ILimitBuilder
    {
        public LimitBuilder(int limit)
        {
            this.Limit = limit;
        }

        public override FragmentType FragmentType
        {
            get
            {
                return SQLiteQueryFragment.Limit;
            }
        }

        public int Limit { get; private set; }
    }
}
