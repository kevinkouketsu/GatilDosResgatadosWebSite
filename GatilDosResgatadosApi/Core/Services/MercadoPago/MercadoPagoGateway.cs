using GatilDosResgatadosApi.Core.Abstractions;
using MercadoPago.Client.PreapprovalPlan;

namespace GatilDosResgatadosApi.Core.Services.MercadoPago;

public class MercadoPagoGateway : IPaymentGateway
{
    public async Task<string> CreatePreapprovalPlan(string planName, string backUrl, PreapprovalRecurring recurring)
    {
        var client = new PreapprovalPlanClient();
        var preapprovalPlan = await client.CreateAsync(new PreapprovalPlanCreateRequest()
        {
            Reason = planName,
            AutoRecurring = new PreapprovalPlanAutoRecurringCreateRequest()
            {
                BillingDay = recurring.BillingDay,
                BillingDayProportional = true,
                CurrencyId = "BRL",
                Frequency = recurring.Frequency,
                FrequencyType = recurring.FrequencyType == FrequencyType.Days ? "days" : "months",
                TransactionAmount = recurring.TransactionAmount,
            },
            BackUrl = backUrl,
        });

        return preapprovalPlan.Id;
    }
}
