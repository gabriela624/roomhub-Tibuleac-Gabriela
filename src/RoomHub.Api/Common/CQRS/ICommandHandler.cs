using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common.CQRS;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand
{
    Task<Result<TResponse>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}