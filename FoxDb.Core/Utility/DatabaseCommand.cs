using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class DatabaseCommand : Disposable, IDatabaseCommand
    {
        public DatabaseCommand(IDbCommand command, IDatabaseParameters parameters, DatabaseCommandFlags flags)
        {
            this.Command = command;
            this.Parameters = parameters;
            this.Flags = flags;
        }

        public IDbCommand Command { get; private set; }

        public IDatabaseParameters Parameters { get; private set; }

        public DatabaseCommandFlags Flags { get; private set; }

        public int ExecuteNonQuery()
        {
            return this.Command.ExecuteNonQuery();
        }

        public object ExecuteScalar()
        {
            return this.Command.ExecuteScalar();
        }

        public IDataReader ExecuteReader()
        {
            return this.Command.ExecuteReader();
        }

        protected override void OnDisposing()
        {
            this.Command.Dispose();
            base.OnDisposing();
        }
    }
}
