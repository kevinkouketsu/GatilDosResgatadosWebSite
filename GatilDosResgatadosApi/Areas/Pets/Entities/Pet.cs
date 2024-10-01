using GatilDosResgatadosApi.Areas.Common;
using GatilDosResgatadosApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace GatilDosResgatadosApi.Areas.Pets.Entities;

public class Pet : AuditableEntity
{
    public string Id { get; set; } = default!;
    public required string Name { get; set; }
    public string Description { get; set; } = default!;
    public double? Weight { get; set; }
    public Gender? Gender { get; set; }
    public string? CreatedById { get; set; }
    public virtual ApplicationUser CreatedBy { get; set; } = default!;
    public override string ToString()
    {
        return $"Pet [ID: {Id}, Name: {Name}, Description: {Description}, " +
               $"Weight: {(Weight.HasValue ? $"{Weight.Value} kg" : "N/A")}, " +
               $"Gender: {(Gender.HasValue ? Gender.Value.ToString() : "N/A")}]";
    }
}
