using FastEndpoints;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GatilDosResgatadosApi.Areas.Pets.Features.Media;

public record DeleteMediaPetRequest(string PetId, string MediaId);

public class DeleteMediaPet(ApplicationDbContext dbContext, ILogger<DeleteMediaPet> logger) : Endpoint<DeleteMediaPetRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete("/api/pets/{@pet}/medias/{@mediaId}", x => new { x.PetId, x.MediaId });
    }

    public async override Task<Results<NoContent, NotFound, ProblemHttpResult>> ExecuteAsync(DeleteMediaPetRequest req, CancellationToken ct)
    {
        var pet = await dbContext.Pets.FirstOrDefaultAsync(x => x.Id == req.PetId, ct);
        if (pet is null)
        {
            return TypedResults.NotFound();
        }

        try
        {
            var media = pet.Medias.Find(x => x.Id == req.MediaId);
            if (media is null)
            {
                return TypedResults.NotFound();
            }

            pet.Medias.Remove(media);
            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError("Could not delete media {media} for pet {pet}. Error: {error}", req.MediaId, req.PetId, ex);
            return TypedResults.Problem("Could not save to database");
        }
        return TypedResults.NoContent();
    }
}
