using GatilDosResgatadosApi.Core.Abstractions;
using MercadoPago.Client.PreapprovalPlan;
using MercadoPago.Error;
using Microsoft.AspNetCore.Http.HttpResults;

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

    public async Task UpdatePreapprovalPlan(string planId, string planName, string backUrl, PreapprovalRecurring recurring)
    {
        var client = new PreapprovalPlanClient();

        await client.UpdateAsync(planId, new PreapprovalPlanUpdateRequest()
        {
            Reason = planName,
            AutoRecurring = new PreapprovalPlanAutoRecurringUpdateRequest()
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
    }
}
