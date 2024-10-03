namespace GatilDosResgatadosApi.Areas.Pets.Entities;

public class PetMedia
{
    public static int MaxMediaSize = Pet.MaxAvatarSize;
    public required string Id { get; set; }
    public byte[]? Data { get; set; }
    public string? Description { get; set; }
}
