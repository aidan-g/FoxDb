﻿using FoxDb.Interfaces;

namespace FoxDb
{
    public class SQLiteQueryFactory : SqlQueryFactory
    {
        public SQLiteQueryFactory(IDatabase database) : base(database)
        {
        }

        protected override IQueryBuilder CreateBuilder(IDatabase database, IQueryGraphBuilder graph)
        {
            return new SQLiteQueryBuilder(database, graph);
        }
    }
}
