namespace MoneyCalc.Web.Models.Board;

public class BoardPostDetailsViewModel
{
    public BoardPost Post { get; init; } = new();

    public IReadOnlyList<BoardComment> Comments { get; init; } = [];

    public BoardCommentCreateViewModel NewComment { get; init; } = new();

    public BoardPostDeleteViewModel Delete { get; init; } = new();

    public string? CommentError { get; init; }

    public string? DeleteError { get; init; }
}
