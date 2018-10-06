using FoxDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace FoxDb
{
    public class DatabaseCommandCache : IDatabaseCommandCache
    {
        private DatabaseCommandCache()
        {
            this.Cache = new ConcurrentDictionary<IDatabaseQuery, IDatabaseCommand>();
        }

        public DatabaseCommandCache(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        public ConcurrentDictionary<IDatabaseQuery, IDatabaseCommand> Cache { get; private set; }

        public IDatabase Database { get; private set; }

        public IDatabaseCommand GetOrAdd(IDatabaseQuery key, Func<IDatabaseCommand> factory)
        {
            return this.Cache.GetOrAdd(key, _key => factory());
        }

        public void Clear()
        {
            this.Cache.Values.ForEach(command => command.Dispose());
            this.Cache.Clear();
        }
    }
}