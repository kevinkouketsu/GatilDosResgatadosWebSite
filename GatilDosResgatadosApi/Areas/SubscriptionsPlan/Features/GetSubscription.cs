using FastEndpoints;
using GatilDosResgatadosApi.Core.Abstractions;
using GatilDosResgatadosApi.Infrastructure.Data;
using MercadoPago.Error;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GatilDosResgatadosApi.Areas.Subscriptions.Features;

public record GetSubscriptionRequest(string SubscriptionId);

public class GetSubscription(
    ApplicationDbContext dbContext, 
    ILogger<GetSubscription> logger, 
    IPaymentGateway paymentGateway
) : Endpoint<GetSubscriptionRequest, Results<Ok<SubscriptionPlanResponse>, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Get("/api/subscriptions/{@SubscriptionId}", x => new { x.SubscriptionId });
    }

    public async override Task<Results<Ok<SubscriptionPlanResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(GetSubscriptionRequest req, CancellationToken ct)
    {
        var subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == req.SubscriptionId, ct);
        if (subscription is null) 
        {
            return TypedResults.NotFound();
        }

        try
        {
            var gatewaySubscription = await paymentGateway.GetPreapprovalPlanAsync(subscription.ExternalReference, ct);
            var response = new SubscriptionPlanResponse()
            {
                Description = subscription.Description,
                Image = subscription.Image,
                Name = subscription.Name,
                Price = subscription.Price,
                Subscribed = gatewaySubscription.Subscribed,
                InitPoint = gatewaySubscription.InitPoint,
                Status = gatewaySubscription.Status == PreapprovalStatus.Active ? SubscriptionStatus.Active : SubscriptionStatus.Cancelled
            };

            return TypedResults.Ok(response);
        }
        catch (MercadoPagoApiException ex) when (ex.StatusCode == StatusCodes.Status404NotFound)
        {
            logger.LogError("Fail to find subscription plan {id} on payment gateway. Error: {ex}", req.SubscriptionId, ex);
            return TypedResults.Problem($"Fail to find subscription plan on payment gateway.");
        }
        catch (Exception ex)
        {
            logger.LogError("Could not fetch id {id}. Error: {ex}", req.SubscriptionId, ex);
            return TypedResults.Problem($"Fail to fetch subscription");
        }
    }
}
