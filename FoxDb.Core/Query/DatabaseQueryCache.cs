using FoxDb.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FoxDb
{
    public class DatabaseQueryCache : IDatabaseQueryCache
    {
        public const string EXISTS = "C0CE76F8-CFEA-4138-A5FA-B445291AB44E";

        public const string LOOKUP = "F8FC6257-7E0E-47E2-8CC5-99F0DC5CF04E";

        public const string LOOKUP_BY_RELATION = "8425EDBF-3F40-4AA1-B2BB-A70C2F4E687E";

        public const string FETCH = "8363888C-D616-419D-9402-0274BD290B5C";

        public const string ADD = "30CFF274-C469-46C1-A44D-ECDCE4459409";

        public const string UPDATE = "6DBF4BBC-ACAD-4E59-B644-298CD446FEE1";

        public const string DELETE = "BABD8AEC-7E5E-4910-A359-32DAC534340D";

        private DatabaseQueryCache()
        {
            this.Cache = new ConcurrentDictionary<IDatabaseQueryCacheKey, IDatabaseQuery>();
        }

        public DatabaseQueryCache(IDatabase database)
            : this()
        {
            this.Database = database;
        }

        public ConcurrentDictionary<IDatabaseQueryCacheKey, IDatabaseQuery> Cache { get; private set; }

        public IDatabase Database { get; private set; }

        public IDatabaseQuery Exists(ITableConfig table)
        {
            return this.GetOrAdd(new DatabaseQueryTableCacheKey(table, EXISTS), () => this.Database.QueryFactory.Exists(table).Build());
        }

        public IDatabaseQuery Lookup(ITableConfig table)
        {
            return this.GetOrAdd(new DatabaseQueryTableCacheKey(table, LOOKUP), () => this.Database.QueryFactory.Lookup(table).Build());
        }

        public IDatabaseQuery Lookup(IRelationConfig relation)
        {
            return this.GetOrAdd(new DatabaseQueryRelationCacheKey(relation, LOOKUP_BY_RELATION), () => this.Database.QueryFactory.Lookup(relation).Build());
        }

        public IDatabaseQuery Fetch(ITableConfig table)
        {
            return this.GetOrAdd(new DatabaseQueryTableCacheKey(table, FETCH), () => this.Database.QueryFactory.Fetch(table).Build());
        }

        public IDatabaseQuery Add(ITableConfig table)
        {
            return this.GetOrAdd(new DatabaseQueryTableCacheKey(table, ADD), () => this.Database.QueryFactory.Add(table).Build());
        }

        public IDatabaseQuery Update(ITableConfig table)
        {
            return this.GetOrAdd(new DatabaseQueryTableCacheKey(table, UPDATE), () => this.Database.QueryFactory.Update(table).Build());
        }

        public IDatabaseQuery Delete(ITableConfig table)
        {
            return this.GetOrAdd(new DatabaseQueryTableCacheKey(table, DELETE), () => this.Database.QueryFactory.Delete(table).Build());
        }

        public IDatabaseQuery GetOrAdd(IDatabaseQueryCacheKey key, Func<IDatabaseQuery> factory)
        {
            return this.Cache.GetOrAdd(key, _key => factory());
        }
    }

    public class DatabaseQueryCacheKey : IDatabaseQueryCacheKey
    {
        public DatabaseQueryCacheKey(string id)
        {
            this.Id = id;
        }

        public string Id { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += this.Id.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is IDatabaseQueryCacheKey)
            {
                return this.Equals(obj as IDatabaseQueryCacheKey);
            }
            return base.Equals(obj);
        }

        public virtual bool Equals(IDatabaseQueryCacheKey other)
        {
            if (other == null)
            {
                return false;
            }
            if (!string.Equals(this.Id, other.Id, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(DatabaseQueryCacheKey a, DatabaseQueryCacheKey b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }
            if ((object)a == null || (object)b == null)
            {
                return false;
            }
            if (object.ReferenceEquals((object)a, (object)b))
            {
                return true;
            }
            return a.Equals(b);
        }

        public static bool operator !=(DatabaseQueryCacheKey a, DatabaseQueryCacheKey b)
        {
            return !(a == b);
        }
    }

    public class DatabaseQueryTableCacheKey : DatabaseQueryCacheKey
    {
        public DatabaseQueryTableCacheKey(ITableConfig table, string id)
            : base(id)
        {
            this.Table = table;
        }

        public ITableConfig Table { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            unchecked
            {
                if (this.Table != null)
                {
                    hashCode += this.Table.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(IDatabaseQueryCacheKey other)
        {
            if (other is DatabaseQueryTableCacheKey)
            {
                return this.Equals(other as DatabaseQueryTableCacheKey);
            }
            return base.Equals(other);
        }

        public bool Equals(DatabaseQueryTableCacheKey other)
        {
            return base.Equals(other) && TableComparer.TableConfig.Equals(this.Table, other.Table);
        }
    }

    public class DatabaseQueryColumnsCacheKey : DatabaseQueryTableCacheKey
    {
        public DatabaseQueryColumnsCacheKey(ITableConfig table, IEnumerable<IColumnConfig> columns, string id)
            : base(table, id)
        {
            this.Columns = columns;
        }

        public IEnumerable<IColumnConfig> Columns { get; private set; }

        public override bool Equals(IDatabaseQueryCacheKey other)
        {
            if (other is DatabaseQueryColumnsCacheKey)
            {
                return this.Equals(other as DatabaseQueryColumnsCacheKey);
            }
            return base.Equals(other);
        }

        public bool Equals(DatabaseQueryColumnsCacheKey other)
        {
            return base.Equals(other) && this.Columns.SequenceEqual(other.Columns, true);
        }
    }

    public class DatabaseQueryRelationCacheKey : DatabaseQueryCacheKey
    {
        public DatabaseQueryRelationCacheKey(IRelationConfig relation, string id)
            : base(id)
        {
            this.Relation = relation;
        }

        public IRelationConfig Relation { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            unchecked
            {
                if (this.Relation != null)
                {
                    hashCode += this.Relation.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(IDatabaseQueryCacheKey other)
        {
            if (other is DatabaseQueryRelationCacheKey)
            {
                return this.Equals(other as DatabaseQueryRelationCacheKey);
            }
            return base.Equals(other);
        }

        public bool Equals(DatabaseQueryRelationCacheKey other)
        {
            return base.Equals(other) && RelationComparer.RelationConfig.Equals(this.Relation, other.Relation);
        }
    }
}
