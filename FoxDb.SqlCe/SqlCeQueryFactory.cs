using FoxDb.Interfaces;
using System.Data;

namespace FoxDb
{
    public class SqlCeQueryFactory : SqlQueryFactory
    {
        public SqlCeQueryFactory(IDatabase database)
            : base(database)
        {
        }

        protected override IQueryBuilder CreateBuilder(IDatabase database, IQueryGraphBuilder graph)
        {
            return new SqlCeQueryBuilder(database, graph);
        }

        public override IDatabaseQuery Create(string commandText, params IDatabaseQueryParameter[] parameters)
        {
            return new SqlCeQuery(commandText, parameters);
        }

        public override IDatabaseQueryDialect Dialect
        {
            get
            {
                return new SqlCeQueryDialect(this.Database);
            }
        }

        public override IQueryGraphBuilder Add(ITableConfig table)
        {
            var builder = default(IQueryGraphBuilder);
            foreach (var column in table.PrimaryKeys)
            {
                if (column.Flags.HasFlag(ColumnFlags.Generated) && column.ColumnType.IsNumeric)
                {
                    if (builder == null)
                    {
                        builder = this.Build();
                    }
                    builder.Output.AddParameter(
                        SqlCeQueryParameter.Identity,
                        DbType.Object,
                        0,
                        0,
                        0,
                        ParameterDirection.Input,
                        true,
                        null,
                        DatabaseQueryParameterFlags.None
                    );
                }
            }
            if (builder == null)
            {
                return base.Add(table);
            }
            return this.Combine(new[] { base.Add(table), builder });
        }

        public override IQueryGraphBuilder Count(ITableConfig table, IQueryGraphBuilder query)
        {
            var builder = this.Build();
            builder.Output.AddFunction(
                QueryFunction.Count,
                builder.Output.CreateOperator(QueryOperator.Star)
            );
            builder.Source.AddSubQuery(
                query.Clone().With(subQuery =>
                {
                    subQuery.Output.Expressions.Clear();
                    subQuery.Output.AddColumn(table.PrimaryKey);
                    subQuery.Aggregate.AddColumn(table.PrimaryKey);
                    subQuery.Sort.Expressions.Clear();
                })
            ).Alias = table.TableName;
            return builder;
        }
    }
}
