using FoxDb.Interfaces;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace FoxDb
{
    public partial class DatabaseCommand : Disposable, IDatabaseCommand
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

    public partial class DatabaseCommand
    {
        public Task<int> ExecuteNonQueryAsync()
        {
            var command = this.Command as DbCommand;
            if (command == null)
            {
                throw new NotImplementedException();
            }
            return command.ExecuteNonQueryAsync();
        }

        public Task<object> ExecuteScalarAsync()
        {
            var command = this.Command as DbCommand;
            if (command == null)
            {
                throw new NotImplementedException();
            }
            return command.ExecuteScalarAsync();
        }

        public async Task<IDataReader> ExecuteReaderAsync()
        {
            var command = this.Command as DbCommand;
            if (command == null)
            {
                throw new NotImplementedException();
            }
            return await command.ExecuteReaderAsync();
        }
    }
}
