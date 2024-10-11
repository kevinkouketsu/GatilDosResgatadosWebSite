using FastEndpoints;
using FluentValidation;
using GatilDosResgatadosApi.Areas.Subscriptions.Entities;
using GatilDosResgatadosApi.Core.Abstractions;
using GatilDosResgatadosApi.Infrastructure;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GatilDosResgatadosApi.Areas.Subscriptions.Features;

public record CreateSubscriptionRequest
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile? Image { get; set; }
}

class CreateSubscriptionRequestValidator : Validator<CreateSubscriptionRequest>
{
    public CreateSubscriptionRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O campo Nome é obrigatório.")
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

public class CreateSubscription(
    ApplicationDbContext dbContext, 
    IPaymentGateway paymentGateway, 
    ILogger<CreateSubscription> logger
) : Endpoint<CreateSubscriptionRequest, Results<Created, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/subscriptions/");
        AllowFileUploads();
    }

    public async override Task<Results<Created, ProblemHttpResult>> ExecuteAsync(CreateSubscriptionRequest req, CancellationToken ct)
    {
        try
        {
            var preapprovalPlanId = await paymentGateway.CreatePreapprovalPlan(req.Name, "https://google.com", new PreapprovalRecurring()
            {
                BillingDay = 5,
                Frequency = 1,
                FrequencyType = FrequencyType.Months,
                TransactionAmount = req.Price
            });

            var preapprovalPlan = new SubscriptionPlan() 
            {
                Id = Guid.NewGuid().ToString(),
                Name = req.Name,
                Price = req.Price,
                ExternalReference = preapprovalPlanId,
                Description = req.Description ?? ""
            };

            await dbContext.Subscriptions.AddAsync(preapprovalPlan, ct);
            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to create subscription: {}", ex);

            return TypedResults.Problem();
        }

        return TypedResults.Created();
    }
}
