using TrainerNetwork.Domain;

namespace TrainerNetwork.Application.Users;

public interface IUserRepository
{
    /// <param name="email">Normalized email: trimmed and lowercase.</param>
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
