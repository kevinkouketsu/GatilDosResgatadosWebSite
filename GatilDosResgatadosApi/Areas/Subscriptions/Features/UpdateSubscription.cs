using FastEndpoints;
using FluentValidation;
using GatilDosResgatadosApi.Core.Abstractions;
using GatilDosResgatadosApi.Infrastructure;
using GatilDosResgatadosApi.Infrastructure.Data;
using MercadoPago.Error;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace GatilDosResgatadosApi.Areas.Subscriptions.Features;

public record UpdateSubscriptionRequest
{
    [FromRoute]
    public string SubscriptionId { get; set; } = default!;
    public string? Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public IFormFile? Image { get; set; }
}

class UpdateSubscriptionRequestValidator : Validator<UpdateSubscriptionRequest>
{
    public UpdateSubscriptionRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("O campo Nome tem um máximo de 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(4096).WithMessage("O campo Descrição tem um máximo de 4096 caracteres");

        RuleFor(x => x.Price)
            .Must(x => x > 0.0m).WithMessage("O preço deve ser maior que R$ 0.00");

        When(x => x.Image is not null, () =>
        {
            RuleFor(x => x.Image)
                .Must(x => x!.IsImage()).WithMessage("A imagem enviada não é válida");
        });
    }
}


public class UpdateSubscription(
    ApplicationDbContext dbContext, 
    IPaymentGateway paymentGateway, 
    ILogger<UpdateSubscription> logger
) : Endpoint<UpdateSubscriptionRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Patch("/api/subscriptions/{SubscriptionId}/");
        AllowFileUploads();
    }

    public async override Task<Results<NoContent, NotFound, ProblemHttpResult>> ExecuteAsync(UpdateSubscriptionRequest req, CancellationToken ct)
    {
        var subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == req.SubscriptionId, ct);
        if (subscription is null)
        {
            return TypedResults.NotFound();
        }

        try
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (!string.IsNullOrEmpty(req.Description))
                subscription.Description = req.Description;

            if (!string.IsNullOrEmpty(req.Name))
                subscription.Name = req.Name;

            if (req.Image?.Length > 0)
                subscription.Image = await req.Image.GetBytesAsync(ct);

            if (req.Price is not null)
                subscription.Price = req.Price.Value;

            await dbContext.SaveChangesAsync(ct);

            await paymentGateway.UpdatePreapprovalPlan(subscription.ExternalReference, 
                subscription.Name, 
                "https://google.com", 
                new PreapprovalRecurring() 
                {
                    BillingDay = 5,
                    Frequency = 1,
                    FrequencyType = FrequencyType.Months,
                    TransactionAmount = subscription.Price
                });

            transaction.Complete();
            return TypedResults.NoContent();
        }
        catch (MercadoPagoApiException ex) when (ex.StatusCode == StatusCodes.Status404NotFound)
        {
            return TypedResults.NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to update subscription {Id}: {Exception}", req.SubscriptionId, ex);

            return TypedResults.Problem();
        }
    }
}
