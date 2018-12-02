using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServer2012QueryFragment : SqlServerQueryFragment
    {
        public const FragmentType Offset = (FragmentType)101;

        public const FragmentType Limit = (FragmentType)102;

        static SqlServer2012QueryFragment()
        {
            Priorities[Offset] = 100;
            Priorities[Limit] = 110;
        }

        public SqlServer2012QueryFragment(IFragmentTarget target)
            : base(target)
        {
        }

        public SqlServer2012QueryFragment(string commandText, byte priority)
            : base(commandText, priority)
        {
        }
    }
}
