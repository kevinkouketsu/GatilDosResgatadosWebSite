using FastEndpoints;
using GatilDosResgatadosApi.Areas.Common;
using GatilDosResgatadosApi.Areas.Pets.Common;
using GatilDosResgatadosApi.Areas.Pets.Entities;
using GatilDosResgatadosApi.Infrastructure;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GatilDosResgatadosApi.Areas.Pets.Features;

public record ListPetsRequest : Pageable
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Gender? Gender { get; set; }
    public double? MinimumWeight { get; set; }
    public double? MaximumWeight { get; set; }
    public bool IncludeAvatar { get; set; } = false;
}

public class ListPets(ApplicationDbContext dbContext) : Endpoint<ListPetsRequest, Results<Ok<PaginatedList<PetResponse>>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Get("/api/pets/");
    }

    public async override Task<Results<Ok<PaginatedList<PetResponse>>, ProblemHttpResult>> ExecuteAsync(ListPetsRequest req, CancellationToken ct)
    {
        var results = await dbContext.Pets
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(req.Name), x => EF.Functions.Like(x.Name, $"%{req.Name}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(req.Description), x => EF.Functions.Like(x.Description, $"%{req.Description}%"))
            .WhereIf(req.Gender.HasValue, x => x.Gender == req.Gender!.Value)
            .WhereIf(req.MinimumWeight.HasValue, x => x.Weight >= req.MinimumWeight!.Value)
            .WhereIf(req.MaximumWeight.HasValue, x => x.Weight <= req.MaximumWeight!.Value)
            .OrderBy(x => x.CreatedAt)
            .ToPaginatedListAsync(req, ct);

        return TypedResults.Ok(results.Map(PetResponse.Map));
    }
}
