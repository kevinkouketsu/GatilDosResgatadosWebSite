namespace GatilDosResgatadosApi.Areas.Common;

public record PaginatedList<T>
{
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int TotalItems { get; init; }
    public IList<T> Result { get; init; } = null!;

    public PaginatedList<U> Map<U>(Func<T, U> action)
    {
        return new()
        {
            Result = Result.Select(action).ToList(),
            CurrentPage = CurrentPage,
            TotalItems = TotalItems,
            TotalPages = TotalPages
        };
    }
}
