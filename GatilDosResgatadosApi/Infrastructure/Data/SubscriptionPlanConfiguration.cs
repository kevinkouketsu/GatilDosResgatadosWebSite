using GatilDosResgatadosApi.Areas.Subscriptions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GatilDosResgatadosApi.Infrastructure.Data;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(8192);
        builder.Property(x => x.Price).IsRequired();
        builder.Property(x => x.Image).HasMaxLength(10 * 1024 * 1024);
    }
}
