using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace FoxDb
{
    public partial class DatabaseReader : Disposable, IDatabaseReader
    {
        public DatabaseReader(IDbCommand command, bool ownsCommand)
        {
            this.Command = command;
            this.OwnsCommand = ownsCommand;
            this.Reader = command.ExecuteReader();
        }

        public IDbCommand Command { get; private set; }

        public bool OwnsCommand { get; private set; }

        public IDataReader Reader { get; private set; }

        public IEnumerator<IDatabaseReaderRecord> GetEnumerator()
        {
            while (this.Reader.Read())
            {
                yield return new DatabaseReaderRecord(this.Reader);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected override void OnDisposing()
        {
            this.Reader.Dispose();
            if (this.OwnsCommand)
            {
                this.Command.Dispose();
            }
            base.OnDisposing();
        }

        private class DatabaseReaderRecord : IDatabaseReaderRecord
        {
            public DatabaseReaderRecord(IDataReader reader)
            {
                this.Reader = reader;
                this.Refresh();
            }

            public IDataReader Reader { get; private set; }

            protected virtual IDictionary<string, object> Data { get; private set; }

            public object this[string name]
            {
                get
                {
                    var value = default(object);
                    if (!this.TryGetValue(name, out value))
                    {
                        return null;
                    }
                    return value;
                }
            }

            public object this[IColumnConfig column]
            {
                get
                {
                    return this[column.Identifier];
                }
            }

            public IEnumerable<string> Names
            {
                get
                {
                    return this.Data.Keys;
                }
            }

            public IEnumerable<object> Values
            {
                get
                {
                    return this.Data.Values;
                }
            }

            public int Count
            {
                get
                {
                    return this.Data.Count;
                }
            }

            public bool Contains(string name)
            {
                return this.Data.ContainsKey(name);
            }

            public bool Contains(IColumnConfig column)
            {
                return this.Contains(column.Identifier);
            }

            public T Get<T>(string name)
            {
                var value = this[name];
                return Converter.ChangeType<T>(value);
            }

            public T Get<T>(IColumnConfig column)
            {
                return this.Get<T>(column.Identifier);
            }

            public bool TryGetValue(string name, out object value)
            {
                return this.Data.TryGetValue(name, out value);
            }

            public bool TryGetValue(IColumnConfig column, out object value)
            {
                return this.TryGetValue(column.Identifier, out value) || this.TryGetValue(column.ColumnName, out value);
            }

            public bool IsNull(string name)
            {
                var value = default(object);
                if (!this.TryGetValue(name, out value))
                {
                    return true;
                }
                return DBNull.Value.Equals(value);
            }

            public bool IsNull(IColumnConfig column)
            {
                return this.IsNull(column.Identifier);
            }

            public void Refresh()
            {
                this.Data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                for (var a = 0; a < this.Reader.FieldCount; a++)
                {
                    var name = this.Reader.GetName(a);
                    var value = this.Reader.GetValue(a);
                    this.Data.Add(name, value);
                }
            }
        }
    }

    public partial class DatabaseReader
    {
        public IAsyncEnumerator<IDatabaseReaderRecord> GetAsyncEnumerator()
        {
            return new AsyncEnumerator(this);
        }

        private class AsyncEnumerator : Disposable, IAsyncEnumerator<IDatabaseReaderRecord>
        {
            public AsyncEnumerator(DatabaseReader reader)
            {
                this.Reader = reader;
            }

            public DatabaseReader Reader { get; private set; }

            public IDatabaseReaderRecord Current { get; private set; }

            object IAsyncEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            IDatabaseReaderRecord IAsyncEnumerator<IDatabaseReaderRecord>.Current
            {
                get
                {
                    return this.Current;
                }
            }

#if NET40
            public Task<bool> MoveNextAsync()
#else
            public async Task<bool> MoveNextAsync()
#endif
            {
                var reader = this.Reader.Reader as DbDataReader;
                if (reader == null)
                {
                    throw new NotImplementedException();
                }
#if NET40
                if (reader.Read())
#else
                if (await reader.ReadAsync().ConfigureAwait(false))
#endif
                {
                    this.Current = new DatabaseReaderRecord(this.Reader.Reader);
#if NET40
                    return TaskEx.FromResult(true);
#else
                    return true;
#endif
                }
                this.Current = null;
#if NET40
                return TaskEx.FromResult(false);
#else
                return false;
#endif
            }

            protected override void OnDisposing()
            {
                this.Reader.Dispose();
                base.OnDisposing();
            }
        }
    }
}
