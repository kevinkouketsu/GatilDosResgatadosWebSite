using FastEndpoints;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GatilDosResgatadosApi.Areas.Pets.Features;

public record DeletePetRequest([FromRoute] string Id);

public class DeletePet(ApplicationDbContext dbContext, ILogger<DeletePet> logger) : Endpoint<DeletePetRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete("/api/pet/{id}");
    }

    public async override Task<Results<NoContent, NotFound, ProblemHttpResult>> ExecuteAsync(DeletePetRequest req, CancellationToken ct)
    {
        var pet = await dbContext.Pets.FindAsync([req.Id], cancellationToken: ct);
        if (pet is null)
        {
            return TypedResults.NotFound();
        }

        try
        {
            dbContext.Pets.Remove(pet);
            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError("Could not delete pet {pet}. Error: {ex}", pet.ToString(), ex);
            return TypedResults.Problem("Could not save to database");
        }

        return TypedResults.NoContent();
    }
}
