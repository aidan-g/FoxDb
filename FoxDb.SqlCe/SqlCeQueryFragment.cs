using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQueryFragment : SqlQueryFragment
    {
        public const FragmentType Offset = (FragmentType)101;

        public const FragmentType Limit = (FragmentType)102;

        public const FragmentType TableHint = (FragmentType)103;

        static SqlCeQueryFragment()
        {
            Priorities[Offset] = 100;
            Priorities[Limit] = 110;
        }

        public SqlCeQueryFragment(IFragmentTarget target)
            : base(target)
        {
        }

        public SqlCeQueryFragment(string commandText, byte priority)
            : base(commandText, priority)
        {
        }
    }
}
