namespace MoneyCalc.Web.Models.Board;

public class BoardPostDetailsViewModel
{
    public BoardPost Post { get; init; } = new();

    public BoardPostDeleteViewModel Delete { get; init; } = new();
}
