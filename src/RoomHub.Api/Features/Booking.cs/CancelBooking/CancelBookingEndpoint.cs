using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid Id) : ICommand;
public sealed record CancelBookingResponse(bool Cancelled);

public sealed class CancelBookingHandler(
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<Result<CancelBookingResponse>> HandleAsync(
        CancelBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var booking = await bookings.GetByIdAsync(command.Id, cancellationToken);
        if (booking is null)
            return Result<CancelBookingResponse>.Failure(
                Error.NotFound("BookingNotFound", "The booking does not exist."));

     var utcNow = timeProvider.GetUtcNow().UtcDateTime;
var result = booking.Cancel(utcNow);
        if (result.IsFailure)
            return Result<CancelBookingResponse>.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CancelBookingResponse>.Success(new CancelBookingResponse(true));
    }
}

public static class CancelBookingEndpoint
{
    public static void Map(RouteGroupBuilder bookings)
    {
        bookings.MapPost("/{id:guid}/cancel", async (
            Guid id,
            ICommandHandler<CancelBookingCommand, CancelBookingResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new CancelBookingCommand(id), cancellationToken);
            return result.IsFailure
                ? result.ToProblemDetails()
                : Results.NoContent();
        });
    }
}