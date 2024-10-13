using GatilDosResgatadosApi.Core.Abstractions;
using MercadoPago.Client.PreapprovalPlan;
using MercadoPago.Error;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GatilDosResgatadosApi.Core.Services.MercadoPago;

public class MercadoPagoGateway : IPaymentGateway
{
    public async Task<string> CreatePreapprovalPlanAsync(string planName, string backUrl, PreapprovalRecurring recurring, CancellationToken cancellationToken)
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
        }, cancellationToken: cancellationToken);

        return preapprovalPlan.Id;
    }

    public async Task UpdatePreapprovalPlanAsync(string planId, string planName, string backUrl, PreapprovalRecurring recurring, CancellationToken cancellationToken)
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
        }, cancellationToken: cancellationToken);
    }

    public async Task<PreapprovalPlan> GetPreapprovalPlanAsync(string planId, CancellationToken cancellationToken)
    {
        var client = new PreapprovalPlanClient();
        var subscriptionPlan = await client.GetAsync(planId, cancellationToken: cancellationToken);

        return new PreapprovalPlan()
        {
            DateCreated = subscriptionPlan.DateCreated,
            InitPoint = subscriptionPlan.InitPoint,
            LastModified = subscriptionPlan.LastModified,
            Recurring = new PreapprovalRecurring()
            {
                BillingDay = subscriptionPlan.AutoRecurring.BillingDay,
                Frequency = subscriptionPlan.AutoRecurring.Frequency,
                TransactionAmount = subscriptionPlan.AutoRecurring.TransactionAmount,
                FrequencyType = subscriptionPlan.AutoRecurring.FrequencyType == "days" ? FrequencyType.Days : FrequencyType.Months,
            },
            Status = subscriptionPlan.Status == "active" ? PreapprovalStatus.Active : PreapprovalStatus.Cancelled,
            Subscribed = subscriptionPlan.Subscribed
        };
    }
}
