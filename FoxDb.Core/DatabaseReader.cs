using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace FoxDb
{
    public class DatabaseReader : Disposable, IDatabaseReader
    {
        public DatabaseReader(IDataReader reader)
        {
            this.Reader = reader;
        }

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

        private class DatabaseReaderRecord : IDatabaseReaderRecord
        {
            public DatabaseReaderRecord(IDataReader reader)
            {
                this.Reader = reader;
            }

            public IDataReader Reader { get; private set; }

            public object this[int index]
            {
                get
                {
                    return this.Reader.GetValue(index);
                }
            }

            public object this[string name]
            {
                get
                {
                    var index = this.Reader.GetOrdinal(name);
                    return this.Reader.GetValue(index);
                }
            }

            public IEnumerable<string> Names
            {
                get
                {
                    for (var a = 0; a < this.Count; a++)
                    {
                        yield return this.Reader.GetName(a);
                    }
                }
            }

            public IEnumerable<object> Values
            {
                get
                {
                    for (var a = 0; a < this.Count; a++)
                    {
                        yield return this.Reader.GetValue(a);
                    }
                }
            }

            public int Count
            {
                get
                {
                    return this.Reader.FieldCount;
                }
            }

            public bool Contains(string name)
            {
                return this.Reader.GetOrdinal(name) != -1;
            }

            public T Get<T>(int index)
            {
                var value = this[index];
                return Converter.ChangeType<T>(value);
            }

            public T Get<T>(string name)
            {
                var value = this[name];
                return Converter.ChangeType<T>(value);
            }

            public IDictionary<string, object> ToDictionary()
            {
                var data = new Dictionary<string, object>();
                for (var a = 0; a < this.Count; a++)
                {
                    var name = this.Reader.GetName(a);
                    var value = this.Reader.GetValue(a);
                    data[name] = value;
                }
                return data;
            }
        }
    }
}
