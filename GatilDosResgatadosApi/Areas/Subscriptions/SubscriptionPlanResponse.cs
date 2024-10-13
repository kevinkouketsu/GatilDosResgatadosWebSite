namespace GatilDosResgatadosApi.Areas.Subscriptions;

public enum SubscriptionStatus
{
    Active,
    Cancelled
}

public class SubscriptionPlanResponse
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; } = default!;
    public int Subscribed { get; set; }
    public SubscriptionStatus Status { get; set; }
    public string InitPoint { get; set; } = default!;
    public byte[]? Image { get; set; }
}
