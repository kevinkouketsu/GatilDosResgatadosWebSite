using GatilDosResgatadosApi.Areas.Pets.Entities;

namespace GatilDosResgatadosApi.Areas.Pets.Common;

public class PetResponse
{
    public string Name { get; set; } = default!; 
    public string? Description { get; set; }
    public double? Weight { get; set; }
    public Gender? Gender { get; set; }
    public byte[]? Avatar { get; set; }

    public static PetResponse Map(Pet? pet) => new()
    {
        Avatar = pet?.Avatar,
        Gender = pet?.Gender,
        Description = pet?.Description,
        Name = pet?.Name!,
        Weight = pet?.Weight
    };
}
