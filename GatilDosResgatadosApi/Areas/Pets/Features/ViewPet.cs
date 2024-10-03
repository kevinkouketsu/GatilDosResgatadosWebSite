using FastEndpoints;
using GatilDosResgatadosApi.Areas.Pets.Common;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GatilDosResgatadosApi.Areas.Pets.Features;

public record ViewPetRequest([FromRoute] string Id);

public class ViewPet(ApplicationDbContext dbContext) : Endpoint<ViewPetRequest, Results<Ok<PetResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/pets/{id}");
    }

    public async override Task<Results<Ok<PetResponse>, NotFound>> ExecuteAsync(ViewPetRequest req, CancellationToken ct)
    {
        var pet = await dbContext.Pets.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (pet is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(PetResponse.Map(pet));
    }
}
