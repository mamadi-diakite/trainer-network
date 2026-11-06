using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrainerNetwork.Application.Content;
using TrainerNetwork.Application.Security;
using TrainerNetwork.Application.Users;
using TrainerNetwork.Infrastructure.Persistence;
using TrainerNetwork.Infrastructure.Security;
using TrainerNetwork.Infrastructure.Seeding;

namespace TrainerNetwork.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<TrainerNetworkDbContext>(options => options.UseSqlite(connectionString));
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IContentQueries, ContentQueries>();
        services.AddScoped<DemoDataSeeder>();

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TrainerNetworkDbContext>();

        EnsureDataDirectoryExists(db.Database.GetConnectionString());
        await db.Database.MigrateAsync(cancellationToken);
    }

    public static async Task<bool> SeedDemoDataAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();

        return await scope.ServiceProvider.GetRequiredService<DemoDataSeeder>().SeedAsync(cancellationToken);
    }

    // SQLite creates the database file but not its parent folder.
    private static void EnsureDataDirectoryExists(string? connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        if (builder.Mode == SqliteOpenMode.Memory || builder.DataSource is "" or ":memory:")
        {
            return;
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(builder.DataSource));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
