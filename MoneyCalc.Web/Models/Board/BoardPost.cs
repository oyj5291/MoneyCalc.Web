namespace MoneyCalc.Web.Models.Board;

public class BoardPost
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public int ViewCount { get; init; }
}
