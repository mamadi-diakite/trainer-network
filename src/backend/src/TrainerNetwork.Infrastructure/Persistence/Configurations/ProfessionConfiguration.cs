namespace TrainerNetwork.Infrastructure.Persistence.Configurations;

internal sealed class ProfessionConfiguration : IEntityTypeConfiguration<Profession>
{
    public void Configure(EntityTypeBuilder<Profession> builder)
    {
        builder.ToTable("Professions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(Profession.NameMaxLength).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(Profession.SlugMaxLength).IsRequired();
        builder.HasIndex(p => p.Slug).IsUnique();

        builder.HasMany(p => p.Topics)
            .WithOne(t => t.Profession)
            .HasForeignKey(t => t.ProfessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
