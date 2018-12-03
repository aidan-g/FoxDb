using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQueryFragment : SqlQueryFragment
    {
        public const FragmentType TableHint = (FragmentType)103;

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
