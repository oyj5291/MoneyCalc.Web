namespace MoneyCalc.Web.Models.Board;

public class BoardComment
{
    public int Id { get; init; }

    public int PostId { get; init; }

    public string Author { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }
}
