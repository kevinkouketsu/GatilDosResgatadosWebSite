using FastEndpoints;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GatilDosResgatadosApi.Areas.Pets.Features.Media;

public record GetPetMediaRequest(string PetId, string MediaId);

public record GetPetMediaResponse(string Id, string Description, byte[] Data);

public class GetPetMedia(ApplicationDbContext dbContext) : Endpoint<GetPetMediaRequest, Results<Ok<GetPetMediaResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/pet/{@pet}/medias/{@mediaId}", x => new { x.PetId, x.MediaId });
    }

    public async override Task<Results<Ok<GetPetMediaResponse>, NotFound>> ExecuteAsync(GetPetMediaRequest req, CancellationToken ct)
    {
        var media = await dbContext.Pets
            .AsNoTracking()
            .Where(x => x.Id == req.PetId)
            .SelectMany(pet => pet.Medias)
            .FirstOrDefaultAsync(media => media.Id == req.MediaId, ct);

        if (media is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new GetPetMediaResponse(media.Id, media.Description ?? "", media.Data));
    }
}
