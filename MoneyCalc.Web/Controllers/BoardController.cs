using MoneyCalc.Web.Models.Board;
using MoneyCalc.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MoneyCalc.Web.Controllers;

[Route("Board")]
public class BoardController(IBoardRepository boardRepository) : Controller
{
    private const int PageSize = 20;

    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = await boardRepository.GetPostsAsync(page, PageSize, cancellationToken);
            ViewData["Title"] = "자유게시판";
            ViewData["Description"] = "머니계산연구소 자유게시판에서 금융 계산과 생활 금융 이야기를 나누세요.";
            return View(model);
        }
        catch (InvalidOperationException)
        {
            ViewData["Title"] = "자유게시판";
            return View(new BoardIndexViewModel { IsConfigured = false });
        }
        catch (Npgsql.NpgsqlException)
        {
            ViewData["Title"] = "자유게시판";
            return View(new BoardIndexViewModel { IsConfigured = false });
        }
    }

    [HttpGet("Write")]
    public IActionResult Write()
    {
        ViewData["Title"] = "자유게시판 글쓰기";
        ViewData["Description"] = "머니계산연구소 자유게시판에 글을 작성합니다.";
        return View(new BoardPostCreateViewModel());
    }

    [HttpPost("Write")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Write(BoardPostCreateViewModel model, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "자유게시판 글쓰기";

        if (!string.IsNullOrWhiteSpace(model.Website))
        {
            ModelState.AddModelError(string.Empty, "게시글을 등록할 수 없습니다.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var postId = await boardRepository.CreatePostAsync(
                model,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers["User-Agent"].ToString(),
                cancellationToken);

            TempData["BoardMessage"] = "게시글이 등록되었습니다.";
            return RedirectToAction(nameof(Details), new { id = postId });
        }
        catch (InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, "게시판 데이터베이스 연결이 아직 설정되지 않았습니다.");
            return View(model);
        }
        catch (Npgsql.NpgsqlException)
        {
            ModelState.AddModelError(string.Empty, "게시판 데이터베이스에 연결할 수 없습니다. 잠시 후 다시 시도해주세요.");
            return View(model);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        var post = await boardRepository.GetPostAsync(id, incrementViewCount: true, cancellationToken);
        if (post is null)
        {
            return NotFound();
        }

        ViewData["Title"] = post.Title;
        ViewData["Description"] = "머니계산연구소 자유게시판 게시글입니다.";
        return View(new BoardPostDetailsViewModel
        {
            Post = post,
            Delete = new BoardPostDeleteViewModel { Id = post.Id }
        });
    }

    [HttpPost("{id:int}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, BoardPostDeleteViewModel model, CancellationToken cancellationToken = default)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var post = await boardRepository.GetPostAsync(id, incrementViewCount: false, cancellationToken);
        if (post is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = post.Title;
            return View("Details", new BoardPostDetailsViewModel { Post = post, Delete = model });
        }

        var deleted = await boardRepository.DeletePostAsync(id, model.Password, cancellationToken);
        if (!deleted)
        {
            ModelState.AddModelError("Password", "비밀번호가 일치하지 않습니다.");
            ViewData["Title"] = post.Title;
            return View("Details", new BoardPostDetailsViewModel { Post = post, Delete = model });
        }

        TempData["BoardMessage"] = "게시글이 삭제되었습니다.";
        return RedirectToAction(nameof(Index));
    }
}
