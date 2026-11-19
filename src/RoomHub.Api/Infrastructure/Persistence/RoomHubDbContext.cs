using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using Microsoft.EntityFrameworkCore;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class RoomHubDbContext(DbContextOptions<RoomHubDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var room = modelBuilder.Entity<Room>();
        room.HasKey(x => x.Id);
        room.Property(x => x.Name).IsRequired();
        room.Property(x => x.Capacity).IsRequired();

        var booking = modelBuilder.Entity<Booking>();
        booking.HasKey(x => x.Id);
        booking.Property(x => x.RoomId).IsRequired();
        booking.Property(x => x.BookedBy).IsRequired();
        booking.Property(x => x.StartUtc).IsRequired();
        booking.Property(x => x.EndUtc).IsRequired();
        booking.Property(x => x.Attendees).IsRequired();
        booking.Property(x => x.Status).IsRequired();
        booking.Property(x => x.StartUtc)
            .HasConversion(
                value => value.ToUniversalTime(),
                value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
        booking.Property(x => x.EndUtc)
            .HasConversion(
                value => value.ToUniversalTime(),
                value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

        booking.HasOne<Room>()
            .WithMany()
            .HasForeignKey(x => x.RoomId);
    }
}