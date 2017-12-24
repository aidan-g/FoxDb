using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public class SQLiteQueryComposer : IDatabaseQueryComposer
    {
        public SQLiteQueryComposer()
        {
            this.Builder = new StringBuilder();
            this.ParameterNames = new List<string>();
        }

        public StringBuilder Builder { get; private set; }

        public IList<string> ParameterNames { get; private set; }

        public IDatabaseQuery Query
        {
            get
            {
                return new DatabaseQuery(this.Builder.ToString(), this.ParameterNames);
            }
        }

        public IDatabaseQueryComposer Select()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.SELECT);
            return this;
        }

        public IDatabaseQueryComposer Insert()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.INSERT);
            return this;
        }

        public IDatabaseQueryComposer Update()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.UPDATE);
            return this;
        }

        public IDatabaseQueryComposer Set()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.SET);
            return this;
        }

        public IDatabaseQueryComposer Delete()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.DELETE);
            return this;
        }

        public IDatabaseQueryComposer From()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.FROM);
            return this;
        }

        public IDatabaseQueryComposer Join()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.JOIN);
            return this;
        }

        public IDatabaseQueryComposer On()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.ON);
            return this;
        }

        public IDatabaseQueryComposer Where()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.WHERE);
            return this;
        }

        public IDatabaseQueryComposer And()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.AND);
            return this;
        }

        public IDatabaseQueryComposer Count()
        {
            this.Builder.Append(SQLiteSyntax.COUNT);
            this.OpenParentheses();
            this.Star();
            this.CloseParentheses();
            this.Builder.Append(" ");
            return this;
        }

        public IDatabaseQueryComposer SubQuery(IDatabaseQuery query)
        {
            this.Builder.AppendFormat("{0}{1}{2}", SQLiteSyntax.OPEN_PARENTHESES, query.CommandText, SQLiteSyntax.CLOSE_PARENTHESES);
            foreach (var parameterName in query.ParameterNames)
            {
                this.ParameterNames.Add(parameterName);
            }
            return this;
        }

        public IDatabaseQueryComposer Table(ITableConfig table)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(table.TableName));
            return this;
        }

        public IDatabaseQueryComposer Column(IColumnConfig column)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(column.Table.TableName, column.ColumnName));
            return this;
        }

        public IDatabaseQueryComposer Columns(IEnumerable<IColumnConfig> columns)
        {
            var first = true;
            foreach (var column in columns)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.ListDelimiter();
                }
                this.Column(column);
            }
            return this;
        }

        public IDatabaseQueryComposer Identifier(string identifier)
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.Identifier(identifier));
            return this;
        }

        public IDatabaseQueryComposer Identifiers(IEnumerable<string> identifiers)
        {
            var first = true;
            foreach (var identifier in identifiers)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.ListDelimiter();
                }
                this.Identifier(identifier);
            }
            return this;
        }

        public IDatabaseQueryComposer Operator(QueryOperator @operator)
        {
            switch (@operator)
            {
                case QueryOperator.Equals:
                    this.Builder.AppendFormat("{0} ", SQLiteSyntax.EQUALS);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return this;
        }

        public IDatabaseQueryComposer Parameter(string parameter)
        {
            this.Builder.AppendFormat("{0}{1} ", SQLiteSyntax.PARAMETER, parameter);
            this.ParameterNames.Add(parameter);
            return this;
        }

        public IDatabaseQueryComposer Parameters(IEnumerable<string> parameters)
        {
            var first = true;
            foreach (var parameter in parameters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.ListDelimiter();
                }
                this.Parameter(parameter);
            }
            return this;
        }

        public IDatabaseQueryComposer IdentifierDelimiter()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.IDENTIFIER_DELIMITER);
            return this;
        }

        public IDatabaseQueryComposer ListDelimiter()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.LIST_DELIMITER);
            return this;
        }

        public IDatabaseQueryComposer OpenParentheses()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.OPEN_PARENTHESES);
            return this;
        }

        public IDatabaseQueryComposer CloseParentheses()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.CLOSE_PARENTHESES);
            return this;
        }

        public IDatabaseQueryComposer Statement()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.STATEMENT);
            return this;
        }

        public IDatabaseQueryComposer Identity()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.IDENTITY);
            return this;
        }

        public IDatabaseQueryComposer Star()
        {
            this.Builder.AppendFormat("{0} ", SQLiteSyntax.STAR);
            return this;
        }

        public IDatabaseQueryComposer AssignParameterToColumn(IColumnConfig column)
        {
            return this.AssignParametersToColumns(new[] { column });
        }

        public IDatabaseQueryComposer AssignParametersToColumns(IEnumerable<IColumnConfig> columns)
        {
            var first = true;
            foreach (var column in columns)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.And();
                }
                this.Column(column);
                this.Operator(QueryOperator.Equals);
                this.Parameter(column.ColumnName);
            }
            return this;
        }

        public IDatabaseQueryComposer AssignParameterToIdentifier(string identifier)
        {
            return this.AssignParametersToIdentifiers(new[] { identifier });
        }

        public IDatabaseQueryComposer AssignParametersToIdentifiers(IEnumerable<string> identifiers)
        {
            var first = true;
            foreach (var identifier in identifiers)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.ListDelimiter();
                }
                this.Identifier(identifier);
                this.Operator(QueryOperator.Equals);
                this.Parameter(identifier);
            }
            return this;
        }
    }
}
