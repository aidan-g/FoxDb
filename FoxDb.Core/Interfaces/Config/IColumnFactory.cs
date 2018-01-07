using System.Reflection;

namespace FoxDb.Interfaces
{
    public interface IColumnFactory
    {
        IColumnConfig Create(ITableConfig table, string name);

        IColumnConfig Create(ITableConfig table, PropertyInfo property);
    }
}
