using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomResponse(Guid Id, string Name, int Capacity);
public sealed record GetRoomQuery(Guid Id) : IQuery;

public sealed class GetRoomHandler(IRoomRepository rooms)
    : IQueryHandler<GetRoomQuery, GetRoomResponse>
{
    public async Task<Result<GetRoomResponse>> HandleAsync(
        GetRoomQuery query,
        CancellationToken cancellationToken = default)
    {
        var room = await rooms.GetByIdAsync(query.Id, cancellationToken);
        if (room is null)
            return Result<GetRoomResponse>.Failure(
                Error.NotFound("RoomNotFound", "The room does not exist."));

        return Result<GetRoomResponse>.Success(
            new GetRoomResponse(room.Id, room.Name, room.Capacity));
    }
}

public static class GetRoomEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRoomQuery, GetRoomResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetRoomQuery(id), cancellationToken);
            return result.IsFailure
                ? result.ToProblemDetails()
                : Results.Ok(result.Value);
        });
    }
}