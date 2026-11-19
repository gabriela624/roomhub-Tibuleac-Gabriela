using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common.CQRS;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}