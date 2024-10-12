using FastEndpoints;
using FluentValidation;
using GatilDosResgatadosApi.Areas.Pets.Entities;
using GatilDosResgatadosApi.Infrastructure;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GatilDosResgatadosApi.Areas.Pets.Features;

public record UpdatePetRequest
{
    [FromRoute]
    public string PetId { get; set; } = default!;
    public string? Name { get; set; }
    public double? Weight { get; set; }
    public string? Description { get; set; }
    public Gender? Gender { get; set; }
    public IFormFile? Avatar { get; set; }
}

public class UpdatePet(ApplicationDbContext dbContext, ILogger<UpdatePetRequest> logger) : Endpoint<UpdatePetRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Patch("api/pets/{PetId}");
        AllowFileUploads();
    }

    public async override Task<Results<NoContent, NotFound, ProblemHttpResult>> ExecuteAsync(UpdatePetRequest req, CancellationToken ct)
    {
        var pet = await dbContext.Pets.FirstOrDefaultAsync(x => x.Id == req.PetId, ct);
        if (pet is null)
        {
            return TypedResults.NotFound();
        }

        if (!string.IsNullOrWhiteSpace(req.Name))
            pet.Name = req.Name;
        if (req.Weight is not null)
            pet.Weight = req.Weight;
        if (!string.IsNullOrWhiteSpace(req.Description))
            pet.Description = req.Description;
        if (req.Gender is not null)
            pet.Gender = req.Gender;

        if (req.Avatar?.Length > 0)
            pet.Avatar = await req.Avatar.GetBytesAsync(ct);

        try
        {
            dbContext.Update(pet);
            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError("Could not update pet {pet}. Error: {error}", pet, ex);
            return TypedResults.Problem("Could not save to database");
        }
        return TypedResults.NoContent();
    }
}
