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

    Task<bool> DeletePostAsync(int id, string password, CancellationToken cancellationToken = default);
}
