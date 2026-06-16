using MoneyCalc.Web.Models.Board;

namespace MoneyCalc.Web.Services;

public interface IBoardRepository
{
    Task EnsureCreatedAsync(CancellationToken cancellationToken = default);

    Task<BoardIndexViewModel> GetPostsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<BoardPost?> GetPostAsync(int id, bool incrementViewCount, CancellationToken cancellationToken = default);

    Task<int> CreatePostAsync(
        BoardPostCreateViewModel post,
        string? clientIp,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<bool> UpdatePostAsync(BoardPostEditViewModel post, CancellationToken cancellationToken = default);

    Task<bool> DeletePostAsync(int id, string password, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BoardComment>> GetCommentsAsync(int postId, CancellationToken cancellationToken = default);

    Task<int> CreateCommentAsync(
        BoardCommentCreateViewModel comment,
        string? clientIp,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateCommentAsync(BoardCommentEditViewModel comment, CancellationToken cancellationToken = default);

    Task<bool> DeleteCommentAsync(BoardCommentDeleteViewModel comment, CancellationToken cancellationToken = default);
}
