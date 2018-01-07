﻿using FoxDb.Interfaces;
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

        protected override void OnDisposing()
        {
            this.Reader.Dispose();
            base.OnDisposing();
        }

        private class DatabaseReaderRecord : IDatabaseReaderRecord
        {
            public DatabaseReaderRecord(IDataReader reader)
            {
                this.Data = reader.ToDictionary();
            }

            public IDictionary<string, object> Data { get; private set; }

            public object this[string name]
            {
                get
                {
                    return this.Data[name];
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

            public T Get<T>(string name)
            {
                var value = this[name];
                return Converter.ChangeType<T>(value);
            }
        }
    }
}
