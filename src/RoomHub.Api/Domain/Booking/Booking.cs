using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed class Booking
{
    private Booking() { }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public static Result<Booking> Create(
        Guid roomId,
        int roomCapacity,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime utcNow)
    {
        startUtc = startUtc.Kind == DateTimeKind.Utc
            ? startUtc
            : DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
        endUtc = endUtc.Kind == DateTimeKind.Utc
            ? endUtc
            : DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);

        if (string.IsNullOrWhiteSpace(bookedBy))
            return Result<Booking>.Failure(Error.Validation("InvalidBookedBy", "BookedBy is required."));

        if (endUtc <= startUtc)
            return Result<Booking>.Failure(Error.Validation("InvalidRange", "The end must be after the start."));

        if (attendees <= 0)
            return Result<Booking>.Failure(Error.Validation("InvalidAttendees", "Attendees must be positive."));

        if (attendees > roomCapacity)
            return Result<Booking>.Failure(Error.Validation("AttendeesExceedCapacity", "The room does not have enough seats."));

        if (startUtc < utcNow)
            return Result<Booking>.Failure(Error.Validation("BookingInPast", "A booking cannot start in the past."));

        var maximumDuration = roomCapacity > 10
    ? TimeSpan.FromHours(2)
    : TimeSpan.FromHours(4);

if (endUtc - startUtc > maximumDuration)
{
    var hours = roomCapacity > 10 ? 2 : 4;

    return Result<Booking>.Failure(
        Error.Validation(
            "BookingTooLong",
            $"A booking cannot exceed {hours} hours for this room."));
}
        return Result<Booking>.Success(new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            BookedBy = bookedBy.Trim(),
            StartUtc = startUtc,
            EndUtc = endUtc,
            Attendees = attendees,
            Status = BookingStatus.Active
        });
    }

    public bool Overlaps(Booking other)
    {
        if (Status != BookingStatus.Active || other.Status != BookingStatus.Active)
            return false;

        return StartUtc < other.EndUtc && other.StartUtc < EndUtc;
    }

   public Result<Booking> Cancel(DateTime utcNow)
{
    if (Status == BookingStatus.Cancelled)
    {
        return Result<Booking>.Failure(
            Error.Conflict(
                "BookingAlreadyCancelled",
                "The booking is already cancelled."));
    }

    if (StartUtc - utcNow < TimeSpan.FromMinutes(60))
    {
        return Result<Booking>.Failure(
            Error.Conflict(
                "CancellationTooLate",
                "A booking can only be cancelled at least 60 minutes before it starts."));
    }

    Status = BookingStatus.Cancelled;
    return Result<Booking>.Success(this);
}
}