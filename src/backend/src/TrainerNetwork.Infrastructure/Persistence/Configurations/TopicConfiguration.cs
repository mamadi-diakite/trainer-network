using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainerNetwork.Domain;

namespace TrainerNetwork.Infrastructure.Persistence.Configurations;

internal sealed class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable("Topics");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(Topic.NameMaxLength).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(Topic.SlugMaxLength).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(Topic.DescriptionMaxLength);
        builder.HasIndex(t => new { t.ProfessionId, t.Slug }).IsUnique();

        builder.HasMany(t => t.Pages)
            .WithOne(p => p.Topic)
            .HasForeignKey(p => p.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
