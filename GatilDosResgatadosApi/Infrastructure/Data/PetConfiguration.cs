using GatilDosResgatadosApi.Areas.Pets.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GatilDosResgatadosApi.Infrastructure.Data;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(64);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.Weight);
        builder.Property(x => x.Gender);
        builder.Property(x => x.Avatar).HasMaxLength(Pet.MaxAvatarSize);

        builder
            .HasOne(x => x.CreatedBy)
            .WithMany(x => x.Pets)
            .HasForeignKey(x => x.CreatedById);

        builder.OwnsMany(p => p.Medias, a =>
        {
            a.ToTable("PetMedias");
            a.HasKey(x => x.Id);

            a.Property<string>("OwnerId").IsRequired();
            a.WithOwner().HasForeignKey("OwnerId");

            a.Property(x => x.Description).HasMaxLength(4096);
            a.Property(x => x.Data).HasMaxLength(PetMedia.MaxMediaSize).IsRequired();
        });
    }
}
