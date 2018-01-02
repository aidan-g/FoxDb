using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public class SQLiteQueryBuilder : IQueryBuilder
    {
        public SQLiteQueryBuilder(IDatabase database, IQueryGraph graph)
        {
            this.Database = database;
            this.Graph = graph;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraph Graph { get; private set; }

        public IDatabaseQuery Query
        {
            get
            {
                var visitor = new SQLiteQueryBuilderVisitor(this.Database);
                visitor.Visit(this.Graph);
                return visitor.Query;
            }
        }

        protected class SQLiteQueryBuilderVisitor : QueryGraphVisitor
        {
            protected readonly IDictionary<Type, byte> FragmentPriorities = new Dictionary<Type, byte>()
            {
                { typeof(IInsertBuilder), 10 },
                { typeof(IUpdateBuilder), 20 },
                { typeof(IDeleteBuilder), 30 },
                { typeof(ISelectBuilder), 40 },
                { typeof(IFromBuilder), 50 },
                { typeof(IWhereBuilder), 60 },
                { typeof(IOrderByBuilder), 70 }
            };

            protected readonly IDictionary<QueryOperator, string> Operators = new Dictionary<QueryOperator, string>()
            {
                { QueryOperator.Equal, SQLiteSyntax.EQUAL },
                { QueryOperator.NotEqual, SQLiteSyntax.NOT_EQUAL },
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

            private SQLiteQueryBuilderVisitor()
            {
                this.Members = new DynamicMethod(this.GetType());
                this.Builder = new StringBuilder();
                this.ParameterNames = new List<string>();
            }

            public SQLiteQueryBuilderVisitor(IDatabase database) : this()
            {
                this.Database = database;
            }

            protected DynamicMethod Members { get; private set; }

            public StringBuilder Builder { get; private set; }

            public ICollection<string> ParameterNames { get; private set; }

            public IDatabase Database { get; private set; }

            public IDatabaseQuery Query
            {
                get
                {
                    return new DatabaseQuery(this.Builder.ToString(), this.ParameterNames.ToArray());
                }
            }

            protected override IEnumerable<IFragmentBuilder> GetFragments(IQueryGraph graph)
            {
                return base.GetFragments(graph).OrderBy(fragment =>
                {
                    var type = fragment.GetType();
                    foreach (var @interface in type.GetInterfaces())
                    {
                        var priority = default(byte);
                        if (FragmentPriorities.TryGetValue(@interface, out priority))
                        {
                            return priority;
                        }
                    }
                    throw new NotImplementedException();
                });
            }

            protected override void VisitDelete(IDeleteBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.DELETE);
            }

            protected override void VisitFrom(IFromBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.FROM);
                this.VisitSourceList(expression.Expressions);
            }

            protected override void VisitInsert(IInsertBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.INSERT);
                this.VisitTable(expression.Table);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
                this.VisitColumnList(expression.Columns, false);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
            }

            protected override void VisitOrderBy(IOrderByBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.ORDER_BY);
                this.VisitColumnList(expression.Columns, true);
            }

            protected override void VisitSelect(ISelectBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.SELECT);
                this.VisitExpressionList(expression.Expressions, SQLiteSyntax.LIST_DELIMITER, true);
            }

            protected override void VisitUpdate(IUpdateBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.UPDATE);
                this.VisitTable(expression.Table);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.SET);
                this.VisitAssignmentList(expression.Expressions);
            }

            protected override void VisitWhere(IWhereBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.WHERE);
                this.VisitCriteriaList(expression.Expressions);
            }

            protected virtual void VisitTable(ITableBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Table.TableName));
            }

            protected virtual void VisitRelation(IRelationBuilder expression)
            {
                switch (expression.Relation.Multiplicity)
                {
                    case RelationMultiplicity.OneToOne:
                    case RelationMultiplicity.OneToMany:
                        if (expression.Relation.Inverted)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            this.VisitRelation(expression, expression.Relation.Table, expression.Relation.Parent.PrimaryKey, expression.Relation.Table.ForeignKey);
                        }
                        break;
                    case RelationMultiplicity.ManyToMany:
                        var table = this.Database.Config.Table(expression.Relation.Parent.TableType, expression.Relation.RelationType);
                        if (expression.Relation.Inverted)
                        {
                            this.VisitRelation(expression, table, table.RightForeignKey, table.RightTable.PrimaryKey);
                        }
                        else
                        {
                            this.VisitRelation(expression, table, table.LeftForeignKey, table.LeftTable.PrimaryKey);
                            this.VisitRelation(expression, table.RightTable, table.RightForeignKey, table.RightTable.PrimaryKey);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            protected virtual void VisitRelation(IExpressionBuilder expression, ITableConfig table, IColumnConfig leftColumn, IColumnConfig rightColumn)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.JOIN);
                this.VisitTable(expression.GetTable(table));
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.ON);
                this.VisitCriteria(expression.GetFragment<IBinaryExpressionBuilder>().With(criteria =>
                {
                    criteria.Left = expression.GetColumn(leftColumn);
                    criteria.Operator = expression.GetOperator(QueryOperator.Equal);
                    criteria.Right = expression.GetColumn(rightColumn);
                }));
            }

            protected virtual void VisitSubQuery(ISubQueryBuilder expression, bool parentheses)
            {
                if (parentheses)
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
                }
                var query = this.Database.QueryFactory.Create(expression.Query);
                this.Builder.AppendFormat("{0} ", query.CommandText);
                if (parentheses)
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
                }
                this.VisitAlias(expression.Alias);
                foreach (var parameterName in query.ParameterNames)
                {
                    if (this.ParameterNames.Contains(parameterName, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    this.ParameterNames.Add(parameterName);
                }
            }

            protected virtual void VisitColumnList(IEnumerable<IColumnBuilder> expressions, bool prefix)
            {
                var first = true;
                foreach (var expression in expressions)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.Builder.AppendFormat("{0} ", SQLiteSyntax.LIST_DELIMITER);
                    }
                    this.VisitColumn(expression, prefix);
                }
            }

            protected virtual void VisitBinary(IBinaryExpressionBuilder expression, bool prefix)
            {
                this.VisitSource(expression);
            }

            protected virtual void VisitColumn(IColumnBuilder expression, bool prefix)
            {
                if (prefix)
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Column.Table.TableName, expression.Column.ColumnName));
                }
                else
                {
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(expression.Column.ColumnName));
                }
                switch (expression.Direction)
                {
                    case OrderByDirection.Ascending:
                        this.Builder.AppendFormat("{0} ", SQLiteSyntax.ASC);
                        break;
                    case OrderByDirection.Descending:
                        this.Builder.AppendFormat("{0} ", SQLiteSyntax.DESC);
                        break;
                }
                this.VisitAlias(expression.Alias);
            }

            protected virtual void VisitParameter(IParameterBuilder expression)
            {
                this.Builder.AppendFormat("{0}{1} ", SQLiteSyntax.PARAMETER, expression.Name);
                this.VisitAlias(expression.Alias);
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
                this.VisitExpressionList(expression.Arguments, SQLiteSyntax.LIST_DELIMITER, true);
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
                this.VisitAlias(expression.Alias);
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
                    this.Visit(expression.GetOperator(QueryOperator.Null));
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

            protected virtual void VisitAlias(string alias)
            {
                if (string.IsNullOrEmpty(alias))
                {
                    return;
                }
                this.Builder.AppendFormat("{0} {1} ", SQLiteSyntax.AS, SQLiteSyntax.Identifier(alias));
            }

            protected virtual void VisitSourceList(IEnumerable<IExpressionBuilder> expressions)
            {
                foreach (var expression in expressions)
                {
                    this.VisitSource(expression);
                }
            }

            protected virtual void VisitSource(IExpressionBuilder expression)
            {
                if (expression is ITableBuilder)
                {
                    this.VisitTable(expression as ITableBuilder);
                }
                else if (expression is IRelationBuilder)
                {
                    this.VisitRelation(expression as IRelationBuilder);
                }
                else if (expression is ISubQueryBuilder)
                {
                    this.VisitSubQuery(expression as ISubQueryBuilder, true);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            protected virtual void VisitExpressionList(IEnumerable<IExpressionBuilder> expressions, string delimiter, bool prefix)
            {
                var first = true;
                foreach (var expression in expressions)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.Builder.AppendFormat("{0} ", delimiter);
                    }
                    this.VisitExpression(expression, prefix);
                }
            }

            protected virtual void VisitExpression(IExpressionBuilder expression, bool prefix)
            {
                if (expression is IBinaryExpressionBuilder)
                {
                    this.VisitCriteria(expression as IBinaryExpressionBuilder);
                }
                else if (expression is IColumnBuilder)
                {
                    this.VisitColumn(expression as IColumnBuilder, prefix);
                }
                else if (expression is IParameterBuilder)
                {
                    this.VisitParameter(expression as IParameterBuilder);
                }
                else if (expression is IFunctionBuilder)
                {
                    this.VisitFunction(expression as IFunctionBuilder);
                }
                else if (expression is IOperatorBuilder)
                {
                    this.VisitOperator(expression as IOperatorBuilder);
                }
                else if (expression is IConstantBuilder)
                {
                    this.VisitConstant(expression as IConstantBuilder);
                }
                else if (expression is ISubQueryBuilder)
                {
                    this.VisitSubQuery(expression as ISubQueryBuilder, false);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            protected virtual void VisitAssignmentList(IEnumerable<IBinaryExpressionBuilder> expressions)
            {
                var first = true;
                foreach (var expression in expressions)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.Builder.AppendFormat("{0} ", SQLiteSyntax.LIST_DELIMITER);
                    }
                    this.VisitAssignment(expression);
                }
            }

            protected virtual void VisitAssignment(IBinaryExpressionBuilder expression)
            {
                this.VisitExpression(expression.Left, false);
                this.VisitOperator(expression.Operator);
                this.VisitExpression(expression.Right, false);
            }

            protected virtual void VisitCriteriaList(IEnumerable<IExpressionBuilder> expressions)
            {
                var first = true;
                foreach (var expression in expressions)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.Builder.AppendFormat("{0} ", SQLiteSyntax.AND_ALSO);
                    }
                    this.VisitExpression(expression, true);
                }
            }

            protected virtual void VisitCriteria(IBinaryExpressionBuilder expression)
            {
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
                if (expression.Left != null)
                {
                    this.VisitExpression(expression.Left, true);
                }
                if (expression.Operator != null)
                {
                    this.VisitOperator(expression.Operator);
                }
                if (expression.Right != null)
                {
                    this.VisitExpression(expression.Right, true);
                }
                this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
            }
        }
    }
}
