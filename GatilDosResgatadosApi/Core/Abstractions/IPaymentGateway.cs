namespace GatilDosResgatadosApi.Core.Abstractions;

public enum FrequencyType
{
    Days,
    Months
}

public class PreapprovalRecurring
{
    public int? Frequency { get; set; } = 1;
    public FrequencyType FrequencyType { get; set; } = FrequencyType.Months;
    public int BillingDay { get; set; }
    public decimal? TransactionAmount { get; set; }
}

public enum PreapprovalStatus
{
    Cancelled,
    Active
}

public class PreapprovalPlan
{
    public PreapprovalRecurring Recurring { get; set; } = default!;
    public PreapprovalStatus Status { get; set; }
    public int Subscribed {  get; set; }
    public string InitPoint { get; set; } = default!;
    public DateTime? DateCreated { get; set; }
    public DateTime? LastModified { get; set; }
}


public interface IPaymentGateway
{
    Task<string> CreatePreapprovalPlanAsync(string planName, string backUrl, PreapprovalRecurring recurring, CancellationToken cancellationToken);
    Task UpdatePreapprovalPlanAsync(string planId, string planName, string backUrl, PreapprovalRecurring recurring, CancellationToken cancellationToken);
    Task<PreapprovalPlan> GetPreapprovalPlanAsync(string planId, CancellationToken cancellationToken);
}
