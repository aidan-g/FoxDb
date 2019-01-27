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
#if NET40
            return TaskEx.FromResult<int>(command.ExecuteNonQuery());
#else
            return command.ExecuteNonQueryAsync();
#endif
        }

        public Task<object> ExecuteScalarAsync()
        {
            var command = this.Command as DbCommand;
            if (command == null)
            {
                throw new NotImplementedException();
            }
#if NET40
            return TaskEx.FromResult<object>(command.ExecuteScalar());
#else
            return command.ExecuteScalarAsync();
#endif
        }

#if NET40
        public Task<IDataReader> ExecuteReaderAsync()
#else
        public async Task<IDataReader> ExecuteReaderAsync()
#endif
        {
            var command = this.Command as DbCommand;
            if (command == null)
            {
                throw new NotImplementedException();
            }
#if NET40
            return TaskEx.FromResult<IDataReader>(command.ExecuteReader());
#else
            return await command.ExecuteReaderAsync();
#endif
        }
    }
}
