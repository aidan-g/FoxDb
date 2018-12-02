using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlServerQueryFragment : SqlQueryFragment
    {
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
