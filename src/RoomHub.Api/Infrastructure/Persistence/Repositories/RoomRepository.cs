using RoomHub.Api.Domain.Rooms;
using Microsoft.EntityFrameworkCore;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public sealed class RoomRepository(RoomHubDbContext dbContext) : IRoomRepository
{
    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Rooms.FirstOrDefaultAsync(room => room.Id == id, cancellationToken);

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await dbContext.Rooms.AddAsync(room, cancellationToken);
    }
}