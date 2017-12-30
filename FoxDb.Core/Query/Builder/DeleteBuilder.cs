using FoxDb.Interfaces;

namespace FoxDb
{
    public class DeleteBuilder : FragmentBuilder, IDeleteBuilder
    {
        public override FragmentType FragmentType
        {
            get
            {
                return FragmentType.Delete;
            }
        }
    }
}
