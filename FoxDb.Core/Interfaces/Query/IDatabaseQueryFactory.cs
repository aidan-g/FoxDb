namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IDatabaseQueryComposer Compose();

        IDatabaseQuery Select<T>() where T : IPersistable;

        IDatabaseQuery Insert<T>() where T : IPersistable;

        IDatabaseQuery Insert<T1, T2>() where T1 : IPersistable where T2 : IPersistable;

        IDatabaseQuery Update<T>() where T : IPersistable;

        IDatabaseQuery Delete<T>() where T : IPersistable;

        IDatabaseQuery Delete<T1, T2>() where T1 : IPersistable where T2 : IPersistable;

        IDatabaseQuery Count(IDatabaseQuery query);
    }
}
