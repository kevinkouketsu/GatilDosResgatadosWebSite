namespace GatilDosResgatadosApi.Areas.Common;

public interface IPageable
{
    int PageNumber { get; }
    int PageSize { get; }
}