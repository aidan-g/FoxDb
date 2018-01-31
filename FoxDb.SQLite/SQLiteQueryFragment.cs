using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQueryFragment : SqlQueryFragment
    {
        public const FragmentType Limit = (FragmentType)101;

        public const FragmentType Offset = (FragmentType)102;

        static SQLiteQueryFragment()
        {
            Priorities[Limit] = 100;
            Priorities[Offset] = 110;
        }

        public SQLiteQueryFragment(IFragmentTarget target) : base(target)
        {

        }

        public SQLiteQueryFragment(string commandText, byte priority) : base(commandText, priority)
        {

        }
    }
}
