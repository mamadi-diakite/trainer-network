using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TrainerNetwork.Infrastructure.Persistence;

namespace TrainerNetwork.Infrastructure.Tests;

/// <summary>In-memory SQLite database that lives as long as its connection stays open.</summary>
internal sealed class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public SqliteTestDatabase()
    {
        _connection.Open();
        using var db = CreateContext();
        db.Database.Migrate();
    }

    public TrainerNetworkDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<TrainerNetworkDbContext>().UseSqlite(_connection).Options);

    public void Dispose() => _connection.Dispose();
}
