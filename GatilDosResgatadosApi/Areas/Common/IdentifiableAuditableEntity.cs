namespace GatilDosResgatadosApi.Areas.Common;

public abstract class IdentifiableAuditableEntity : AuditableEntity
{
    public required string Id { get; set; }
}
