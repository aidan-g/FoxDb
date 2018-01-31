using FoxDb.Interfaces;

namespace FoxDb
{
    public class SqlCeQueryFragment : SqlQueryFragment
    {
        public SqlCeQueryFragment(IFragmentTarget target) : base(target)
        {
        }

        public SqlCeQueryFragment(string commandText, byte priority) : base(commandText, priority)
        {
        }
    }
}
