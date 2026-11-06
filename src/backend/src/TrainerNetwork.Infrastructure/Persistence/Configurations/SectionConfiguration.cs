using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainerNetwork.Domain;

namespace TrainerNetwork.Infrastructure.Persistence.Configurations;

internal sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title).HasMaxLength(Section.TitleMaxLength).IsRequired();
        builder.Property(s => s.Content).IsRequired();

        // Not unique: reordering rewrites positions one row at a time.
        builder.HasIndex(s => new { s.PageId, s.Position });
    }
}
