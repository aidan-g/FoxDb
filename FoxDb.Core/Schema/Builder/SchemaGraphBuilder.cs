using FoxDb.Interfaces;
using System.Diagnostics;

namespace FoxDb
{
    public class SchemaGraphBuilder : QueryGraphBuilder, ISchemaGraphBuilder
    {
        public SchemaGraphBuilder(IDatabase database)
            : base(database)
        {

        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ICreateBuilder Create
        {
            get
            {
                return this.Fragment<ICreateBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IAlterBuilder Alter
        {
            get
            {
                return this.Fragment<IAlterBuilder>();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDropBuilder Drop
        {
            get
            {
                return this.Fragment<IDropBuilder>();
            }
        }
    }
}
