using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingRequest(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);
public sealed record CreateBookingResponse(Guid Id, Guid RoomId, string Status);
public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand;

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, CreateBookingResponse>
{
    public async Task<Result<CreateBookingResponse>> HandleAsync(
        CreateBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return Result<CreateBookingResponse>.Failure(
                Error.NotFound("RoomNotFound", "The room does not exist."));

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        var bookingResult = Booking.Create(
            command.RoomId,
            room.Capacity,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            utcNow);

        if (bookingResult.IsFailure)
            return Result<CreateBookingResponse>.Failure(bookingResult.Error);

        var existingBookings = await bookings.ListActiveForRoomAsync(command.RoomId, cancellationToken);
        if (existingBookings.Any(bookingResult.Value.Overlaps))
            return Result<CreateBookingResponse>.Failure(
                Error.Conflict("RoomOccupied", "The room is occupied in this interval."));

        var utcStartDay = DateTime.SpecifyKind(
            bookingResult.Value.StartUtc,
            DateTimeKind.Utc);
        var participantBookings = await bookings.ListActiveForParticipantOnUtcDayAsync(
            bookingResult.Value.BookedBy,
            utcStartDay,
            cancellationToken);

        if (participantBookings.Count >= 2)
            return Result<CreateBookingResponse>.Failure(
                Error.Conflict(
                    "ParticipantBookingLimit",
                    "A participant cannot have more than 2 active bookings per day."));

        await bookings.AddAsync(bookingResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var booking = bookingResult.Value;
        return Result<CreateBookingResponse>.Success(
            new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString()));
    }
}

public static class CreateBookingEndpoint
{
    public static void Map(RouteGroupBuilder rooms)
    {
        rooms.MapPost("/{roomId:guid}/bookings", async (
            Guid roomId,
            CreateBookingRequest request,
            ICommandHandler<CreateBookingCommand, CreateBookingResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(
                new CreateBookingCommand(
                    roomId,
                    request.BookedBy,
                    request.StartUtc,
                    request.EndUtc,
                    request.Attendees),
                cancellationToken);

            if (result.IsFailure)
                return result.ToProblemDetails();

            var response = result.Value;
            return Results.Created($"/api/bookings/{response.Id}", response);
        });
    }
}