using System;
using System.Collections.Generic;

namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryComposer
    {
        IDatabaseQuery Query { get; }

        IDatabaseQueryComposer Select();

        IDatabaseQueryComposer Insert();

        IDatabaseQueryComposer Update();

        IDatabaseQueryComposer Set();

        IDatabaseQueryComposer Delete();

        IDatabaseQueryComposer From();

        IDatabaseQueryComposer Join();

        IDatabaseQueryComposer On();

        IDatabaseQueryComposer Where();

        IDatabaseQueryComposer And();

        IDatabaseQueryComposer Count();

        IDatabaseQueryComposer SubQuery(IDatabaseQuery query);

        IDatabaseQueryComposer Table(ITableConfig table);

        IDatabaseQueryComposer Column(IColumnConfig column);

        IDatabaseQueryComposer Columns(IEnumerable<IColumnConfig> columns);

        IDatabaseQueryComposer Identifier(string identifier);

        IDatabaseQueryComposer Identifiers(IEnumerable<string> identifiers);

        IDatabaseQueryComposer Operator(QueryOperator @operator);

        IDatabaseQueryComposer Parameter(string parameter);

        IDatabaseQueryComposer Parameters(IEnumerable<string> parameters);

        IDatabaseQueryComposer IdentifierDelimiter();

        IDatabaseQueryComposer ListDelimiter();

        IDatabaseQueryComposer OpenParentheses();

        IDatabaseQueryComposer CloseParentheses();

        IDatabaseQueryComposer Statement();

        IDatabaseQueryComposer Identity();

        IDatabaseQueryComposer Star();

        IDatabaseQueryComposer AssignParameterToColumn(IColumnConfig column);

        IDatabaseQueryComposer AssignParametersToColumns(IEnumerable<IColumnConfig> columns);

        IDatabaseQueryComposer AssignParameterToIdentifier(string identifier);

        IDatabaseQueryComposer AssignParametersToIdentifiers(IEnumerable<string> identifiers);
    }

    public enum QueryOperator : byte
    {
        None = 0,
        Equals = 1
    }
}
