using FastEndpoints;
using GatilDosResgatadosApi.Areas.Common;
using GatilDosResgatadosApi.Areas.Pets.Entities;
using GatilDosResgatadosApi.Infrastructure;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GatilDosResgatadosApi.Areas.Pets.Features.Media;

public record UploadMediaPetRequest
{
    public required string PetId { get; set; }

    [FromForm]
    public IEnumerable<string>? Descriptions { get; set; }
}

class UploadMediaPetRequestValidator : Validator<UploadMediaPetRequest>
{
    public UploadMediaPetRequestValidator()
    {
    }
}

public class UploadMediaPet(ApplicationDbContext dbContext, ILogger<UploadMediaPet> logger) : Endpoint<UploadMediaPetRequest, Results<NoContent, NotFound, BadRequest, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/pet/{PetId}/medias/");
        AllowFileUploads();
    }

    public async override Task<Results<NoContent, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(UploadMediaPetRequest req, CancellationToken ct)
    {
        // Fast-endpoints does not support binding forms to complex DTO
        // So for now, we are going to have a simple DTO and will handle it manually
        // The expected here is to have Descriptions with the same size as file
        // and Description[0] will be used with File[0] and so on.
        if (req.Descriptions is null || req.Descriptions.Count() != Files.Count)
        {
            return TypedResults.BadRequest();
        }

        var pet = await dbContext.Pets.FirstOrDefaultAsync(x => x.Id == req.PetId, ct);
        if (pet is null)
        {
            return TypedResults.NotFound();
        }

        try
        {
            foreach (var (file, desc) in Files.Zip(req.Descriptions))
            {
                var media = await file.GetBytesAsync(ct);
                var petMedia = new PetMedia()
                {
                    Id = Guid.NewGuid().ToString(),
                    Data = media,
                    Description = desc
                };

                pet.Medias.Add(petMedia);
            }

            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError("Could not update media for pet {pet}. Error: {error}", req.PetId, ex);
            return TypedResults.Problem("Could not save to database");
        }
        return TypedResults.NoContent();
    }
}
