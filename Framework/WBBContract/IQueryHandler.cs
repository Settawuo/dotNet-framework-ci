using System.Threading.Tasks;

namespace WBBContract
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }

    public interface IQueryHandlerAsync<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}