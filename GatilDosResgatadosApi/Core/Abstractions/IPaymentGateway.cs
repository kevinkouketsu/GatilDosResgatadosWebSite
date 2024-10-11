namespace GatilDosResgatadosApi.Core.Abstractions;

public enum FrequencyType
{
    Days,
    Months
}

public class PreapprovalRecurring
{
    public int Frequency { get; set; } = 1;
    public FrequencyType FrequencyType { get; set; } = FrequencyType.Months;
    public int BillingDay { get; set; }
    public decimal TransactionAmount { get; set; }
}

public interface IPaymentGateway
{
    Task<string> CreatePreapprovalPlan(string planName, string backUrl, PreapprovalRecurring recurring);
}
