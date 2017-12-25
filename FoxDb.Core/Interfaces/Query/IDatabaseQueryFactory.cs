namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IDatabaseQueryComposer Compose();

        IDatabaseQuery Select<T>();

        IDatabaseQuery Insert<T>();

        IDatabaseQuery Insert<T1, T2>();

        IDatabaseQuery Update<T>();

        IDatabaseQuery Delete<T>();

        IDatabaseQuery Delete<T1, T2>();

        IDatabaseQuery Count(IDatabaseQuery query);
    }
}
