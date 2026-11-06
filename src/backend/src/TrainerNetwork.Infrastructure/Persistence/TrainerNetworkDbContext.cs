namespace TrainerNetwork.Infrastructure.Persistence;

public sealed class TrainerNetworkDbContext(DbContextOptions<TrainerNetworkDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Profession> Professions => Set<Profession>();

    public DbSet<Topic> Topics => Set<Topic>();

    public DbSet<Page> Pages => Set<Page>();

    public DbSet<Section> Sections => Set<Section>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainerNetworkDbContext).Assembly);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
}
