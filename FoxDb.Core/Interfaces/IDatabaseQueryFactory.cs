namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IDatabaseQuery Select<T>(params string[] filters);

        IDatabaseQuery First<T>(params string[] filters);

        IDatabaseQuery Count<T>(params string[] filters);

        IDatabaseQuery Count<T>(IDatabaseQuery query);

        IDatabaseQuery Insert<T>();

        IDatabaseQuery Update<T>();

        IDatabaseQuery Delete<T>();
    }
}
