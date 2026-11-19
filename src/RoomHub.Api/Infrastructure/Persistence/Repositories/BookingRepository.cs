using RoomHub.Api.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository(RoomHubDbContext dbContext) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Bookings.FirstOrDefaultAsync(booking => booking.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(
        Guid roomId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Bookings
            .Where(booking => booking.RoomId == roomId &&
                              booking.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> ListActiveForParticipantOnUtcDayAsync(
        string bookedBy,
        DateTime utcDay,
        CancellationToken cancellationToken = default)
    {
        var dayStart = new DateTime(
            utcDay.Year,
            utcDay.Month,
            utcDay.Day,
            0, 0, 0,
            DateTimeKind.Utc);
        var nextDay = dayStart.AddDays(1);

        return await dbContext.Bookings
            .Where(booking =>
                booking.BookedBy == bookedBy &&
                booking.Status == BookingStatus.Active &&
                booking.StartUtc >= dayStart &&
                booking.StartUtc < nextDay)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await dbContext.Bookings.AddAsync(booking, cancellationToken);
    }
}