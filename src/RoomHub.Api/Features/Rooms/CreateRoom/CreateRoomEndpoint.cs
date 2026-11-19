using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomRequest(string? Name, int Capacity);
public sealed record CreateRoomResponse(Guid Id, string Name, int Capacity);

public sealed record CreateRoomCommand(string? Name, int Capacity) : ICommand;

public sealed class CreateRoomHandler(
    IRoomRepository rooms,
    RoomHub.Api.Domain.Common.IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoomCommand, CreateRoomResponse>
{
    public async Task<RoomHub.Api.Domain.Common.Result<CreateRoomResponse>> HandleAsync(
        CreateRoomCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = Room.Create(command.Name, command.Capacity);
        if (result.IsFailure)
            return RoomHub.Api.Domain.Common.Result<CreateRoomResponse>.Failure(result.Error);

        await rooms.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var room = result.Value;
        return RoomHub.Api.Domain.Common.Result<CreateRoomResponse>.Success(
            new CreateRoomResponse(room.Id, room.Name, room.Capacity));
    }
}

public static class CreateRoomEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateRoomRequest request,
            ICommandHandler<CreateRoomCommand, CreateRoomResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(
                new CreateRoomCommand(request.Name, request.Capacity),
                cancellationToken);

            if (result.IsFailure)
                return result.ToProblemDetails();

            var response = result.Value;
            return Results.Created($"/api/rooms/{response.Id}", response);
        });
    }
}