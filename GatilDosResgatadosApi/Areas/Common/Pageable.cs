namespace GatilDosResgatadosApi.Areas.Common;

public record Pageable : IPageable
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}