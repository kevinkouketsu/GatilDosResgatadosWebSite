using GatilDosResgatadosApi.Areas.Common;

namespace GatilDosResgatadosApi.Areas.Subscriptions.Entities;

public class SubscriptionPlan : IdentifiableAuditableEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; } = default!;
    public string ExternalReference { get; set; } = default!;
    public byte[]? Image { get; set; }
}
