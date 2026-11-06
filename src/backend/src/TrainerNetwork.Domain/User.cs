namespace TrainerNetwork.Domain;

public sealed class User
{
    public const int EmailMaxLength = 254;
    public const int DisplayNameMaxLength = 100;
    public const int PasswordHashMaxLength = 500;

    private User()
    {
    }

    public User(string email, string displayName, string passwordHash, UserRole role, DateTime createdAtUtc)
    {
        Id = Guid.CreateVersion7();
        Email = NormalizeEmail(email);
        DisplayName = Guard.Required(displayName, nameof(displayName), DisplayNameMaxLength);
        PasswordHash = Guard.Required(passwordHash, nameof(passwordHash), PasswordHashMaxLength);
        Role = Guard.Defined(role, nameof(role));
        CreatedAtUtc = Guard.Utc(createdAtUtc, nameof(createdAtUtc));
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public void ChangePassword(string passwordHash) =>
        PasswordHash = Guard.Required(passwordHash, nameof(passwordHash), PasswordHashMaxLength);

    private static string NormalizeEmail(string? email)
    {
        var value = Guard.Required(email, nameof(email), EmailMaxLength).ToLowerInvariant();
        var at = value.IndexOf('@');
        var domain = at < 0 ? string.Empty : value[(at + 1)..];

        var isValid = at > 0
            && at == value.LastIndexOf('@')
            && !value.Any(char.IsWhiteSpace)
            && domain.Contains('.')
            && !domain.StartsWith('.')
            && !domain.EndsWith('.');

        return isValid ? value : throw new ArgumentException("Email is not valid.", nameof(email));
    }
}
