using GatilDosResgatadosApi.Areas.Pets.Entities;

namespace GatilDosResgatadosApi.Areas.Pets.Common;

public record PetMediaInfoResponse(string Id);

public class PetResponse
{
    public string Id { get; set; }
    public string Name { get; set; } = default!; 
    public string? Description { get; set; }
    public double? Weight { get; set; }
    public Gender? Gender { get; set; }
    public byte[]? Avatar { get; set; }
    public IList<PetMediaInfoResponse> Medias { get; set; } = default!;

    public static PetResponse Map(Pet pet) => new()
    {
        Id = pet.Id,
        Avatar = pet.Avatar,
        Gender = pet.Gender,
        Description = pet.Description,
        Name = pet.Name!,
        Weight = pet.Weight,
        Medias = pet.Medias.Select(x => new PetMediaInfoResponse(x.Id)).ToList()
    };
}
