namespace FoxDb.Interfaces
{
    public interface IDatabaseQueryFactory
    {
        IQueryGraphBuilder Build();

        IDatabaseQuery Create(params IQueryGraph[] graphs);

        IDatabaseQuery Create(string commandText, params string[] parameterNames);

        IDatabaseQuery Select<T>();

        IDatabaseQuery Insert<T>();

        IDatabaseQuery Insert<T1, T2>();

        IDatabaseQuery Update<T>();

        IDatabaseQuery Delete<T>();

        IDatabaseQuery Delete<T1, T2>();

        IDatabaseQuery Count(IDatabaseQuery query);
    }
}
