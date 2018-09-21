using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQueryFragment : SqlQueryFragment
    {
        public const FragmentType Offset = (FragmentType)101;

        public const FragmentType Limit = (FragmentType)102;

        static SqlServerQueryFragment()
        {
            Priorities[Offset] = 100;
            Priorities[Limit] = 110;
        }

        public SqlServerQueryFragment(IFragmentTarget target)
            : base(target)
        {
        }

        public SqlServerQueryFragment(string commandText, byte priority)
            : base(commandText, priority)
        {
        }
    }
}
