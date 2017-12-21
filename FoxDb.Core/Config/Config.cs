using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class Config : IConfig
    {
        public Config()
        {
            this.Tables = new Dictionary<Type, ITableConfig>();
        }

        private Dictionary<Type, ITableConfig> Tables { get; set; }

        public ITableConfig<T> Table<T>()
        {
            if (!this.Tables.ContainsKey(typeof(T)))
            {
                var config = new TableConfig<T>();
                this.Tables.Add(typeof(T), config);
            }
            return this.Tables[typeof(T)] as ITableConfig<T>;
        }
    }
}
