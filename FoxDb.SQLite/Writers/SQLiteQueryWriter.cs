using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public abstract class SQLiteQueryWriter : FragmentBuilder, IFragmentTarget
    {
        protected IDictionary<FragmentType, QueryGraphVisitorHandler> Handlers { get; private set; }

        protected StringBuilder Builder { get; private set; }

        protected virtual IDictionary<FragmentType, QueryGraphVisitorHandler> GetHandlers()
        {
            return new Dictionary<FragmentType, QueryGraphVisitorHandler>()
            {
                { FragmentType.Binary, fragment => this.VisitBinary(fragment as IBinaryExpressionBuilder) },
                { FragmentType.Table, fragment => this.VisitTable(fragment as ITableBuilder) },
                { FragmentType.Relation, fragment => this.VisitRelation(fragment as IRelationBuilder) },
                { FragmentType.Column, fragment => this.VisitColumn(fragment as IColumnBuilder) },
                { FragmentType.Parameter, fragment => this.VisitParameter(fragment as IParameterBuilder) },
                { FragmentType.Function, fragment => this.VisitFunction(fragment as IFunctionBuilder) },
                { FragmentType.Operator, fragment => this.VisitOperator(fragment as IOperatorBuilder) },
                { FragmentType.Constant, fragment => this.VisitConstant(fragment as IConstantBuilder) },
                { FragmentType.SubQuery, fragment => this.VisitSubQuery(fragment as ISubQueryBuilder) }
            };
        }

        protected readonly IDictionary<QueryOperator, string> Operators = new Dictionary<QueryOperator, string>()
        {
            { QueryOperator.Equal, SQLiteSyntax.EQUAL },
            { QueryOperator.NotEqual, SQLiteSyntax.NOT_EQUAL },
            { QueryOperator.Greater, SQLiteSyntax.GREATER },
            { QueryOperator.Less, SQLiteSyntax.LESS },
            { QueryOperator.And, SQLiteSyntax.AND },
            { QueryOperator.AndAlso, SQLiteSyntax.AND_ALSO },
            { QueryOperator.Or, SQLiteSyntax.OR },
            { QueryOperator.OrElse, SQLiteSyntax.OR_ELSE },
            { QueryOperator.OpenParentheses, SQLiteSyntax.OPEN_PARENTHESES },
            { QueryOperator.CloseParentheses, SQLiteSyntax.CLOSE_PARENTHESES },
            { QueryOperator.Null, SQLiteSyntax.NULL },
            { QueryOperator.Star, SQLiteSyntax.STAR }
        };

        protected readonly IDictionary<QueryFunction, string> Functions = new Dictionary<QueryFunction, string>()
        {
            { QueryFunction.Identity, SQLiteSyntax.IDENTITY },
            { QueryFunction.Count, SQLiteSyntax.COUNT },
            { QueryFunction.Exists, SQLiteSyntax.EXISTS }
        };

        protected SQLiteQueryWriter() : base(QueryGraphBuilder.Null)
        {
            this.Handlers = this.GetHandlers();
            this.Builder = new StringBuilder();
        }

        public SQLiteQueryWriter(IDatabase database, IQueryGraphVisitor visitor, ICollection<string> parameterNames) : this()
        {
            this.Database = database;
            this.Visitor = visitor;
            this.ParameterNames = parameterNames;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraphVisitor Visitor { get; private set; }

        public ICollection<string> ParameterNames { get; private set; }

        public override FragmentType FragmentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string CommandText
        {
            get
            {
                return this.Builder.ToString();
            }
        }

        public IDictionary<string, object> Constants
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public abstract T Write<T>(T fragment) where T : IFragmentBuilder;

        protected virtual void Visit(IExpressionBuilder expression)
        {
            var handler = default(QueryGraphVisitorHandler);
            if (!this.Handlers.TryGetValue(expression.FragmentType, out handler))
            {
                throw new NotImplementedException();
            }
            handler(expression);
        }

        protected virtual void Visit(IEnumerable<IExpressionBuilder> expressions)
        {
            foreach (var expression in expressions)
            {
                this.Visit(expression);
            }
        }

        protected virtual void VisitTable(ITableBuilder expression)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Table.TableName));
        }

        protected virtual void VisitRelation(IRelationBuilder expression)
        {
            switch (expression.Relation.Flags.GetMultiplicity())
            {
                case RelationFlags.OneToOne:
                case RelationFlags.OneToMany:
                    this.VisitRelation(expression, expression.Relation.RightTable, expression.Relation.LeftColumn, expression.Relation.RightColumn);
                    break;
                case RelationFlags.ManyToMany:
                    this.VisitRelation(expression, expression.Relation.MappingTable, expression.Relation.LeftColumn, expression.Relation.LeftTable.PrimaryKey);
                    this.VisitRelation(expression, expression.Relation.RightTable, expression.Relation.RightColumn, expression.Relation.RightTable.PrimaryKey);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void VisitRelation(IExpressionBuilder expression, ITableConfig table, IColumnConfig leftColumn, IColumnConfig rightColumn)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.JOIN);
            this.VisitTable(expression.CreateTable(table));
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.ON);
            this.VisitBinary(expression.CreateFragment<IBinaryExpressionBuilder>().With(criteria =>
            {
                criteria.Left = expression.CreateColumn(leftColumn);
                criteria.Operator = expression.CreateOperator(QueryOperator.Equal);
                criteria.Right = expression.CreateColumn(rightColumn);
            }));
        }

        protected virtual void VisitBinary(IBinaryExpressionBuilder expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Operator);
            this.Visit(expression.Right);
        }

        protected virtual void VisitColumn(IColumnBuilder expression)
        {
            var identifier = default(string);
            if (expression.Flags.HasFlag(ColumnBuilderFlags.Identifier))
            {
                identifier = expression.Column.Identifier;
            }
            else
            {
                identifier = expression.Column.ColumnName;
            }
            if (expression.Flags.HasFlag(ColumnBuilderFlags.Distinct))
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.DISTINCT);
            }
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Column.Table.TableName, identifier));
        }

        protected virtual void VisitParameter(IParameterBuilder expression)
        {
            this.Builder.AppendFormat("{0}{1} ", SQLiteSyntax.PARAMETER, expression.Name);
            this.ParameterNames.Add(expression.Name);
        }

        protected virtual void VisitFunction(IFunctionBuilder expression)
        {
            var function = default(string);
            if (!Functions.TryGetValue(expression.Function, out function))
            {
                throw new NotImplementedException();
            }
            this.Builder.AppendFormat("{0} ", function);
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            if (expression.Expressions.Any())
            {
                this.Visit(expression.Expressions);
            }
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
        }

        protected virtual void VisitOperator(IOperatorBuilder expression)
        {
            var @operator = default(string);
            if (!Operators.TryGetValue(expression.Operator, out @operator))
            {
                throw new NotImplementedException();
            }
            this.Builder.AppendFormat("{0} ", @operator);
        }

        protected virtual void VisitConstant(IConstantBuilder expression)
        {
            if (expression.Value == null)
            {
                this.Visit(expression.CreateOperator(QueryOperator.Null));
            }
            else
            {
                switch (Type.GetTypeCode(expression.Value.GetType()))
                {
                    case TypeCode.Int32:
                        this.Builder.AppendFormat("{0} ", expression.Value);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected virtual void VisitSubQuery(ISubQueryBuilder expression)
        {
            var query = this.Database.QueryFactory.Create(expression.Query);
            this.Builder.AppendFormat("{0} ", query.CommandText);
            foreach (var parameterName in query.ParameterNames)
            {
                if (this.ParameterNames.Contains(parameterName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }
                this.ParameterNames.Add(parameterName);
            }
        }

        protected virtual void VisitAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return;
            }
            this.Builder.AppendFormat("{0} {1} ", SQLiteSyntax.AS, SQLiteSyntax.Identifier(alias));
        }
    }
}
