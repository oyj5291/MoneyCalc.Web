namespace MoneyCalc.Web.Models.Board;

public class BoardIndexViewModel
{
    public IReadOnlyList<BoardPost> Posts { get; init; } = [];

    public int CurrentPage { get; init; } = 1;

    public int TotalPages { get; init; } = 1;

    public int TotalCount { get; init; }

    public bool IsConfigured { get; init; } = true;
}
