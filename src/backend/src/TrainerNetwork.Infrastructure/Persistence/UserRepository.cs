namespace TrainerNetwork.Infrastructure.Persistence;

public sealed class UserRepository(TrainerNetworkDbContext db) : IUserRepository
{
    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
