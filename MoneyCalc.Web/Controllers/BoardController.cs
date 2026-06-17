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
            ViewData["Title"] = "자유게시판 | 금융 계산 질문과 정보 공유";
            ViewData["Description"] = "머니계산연구소 자유게시판에서 대출, 예금, 적금, 연봉 계산 관련 질문과 생활 금융 정보를 자유롭게 나누세요.";
            return View(model);
        }
        catch (InvalidOperationException)
        {
            ViewData["Title"] = "자유게시판 | 금융 계산 질문과 정보 공유";
            ViewData["Description"] = "머니계산연구소 자유게시판에서 금융 계산 관련 질문과 정보를 나누세요.";
            return View(new BoardIndexViewModel { IsConfigured = false });
        }
        catch (Npgsql.NpgsqlException)
        {
            ViewData["Title"] = "자유게시판 | 금융 계산 질문과 정보 공유";
            ViewData["Description"] = "머니계산연구소 자유게시판에서 금융 계산 관련 질문과 정보를 나누세요.";
            return View(new BoardIndexViewModel { IsConfigured = false });
        }
    }

    [HttpGet("Write")]
    public IActionResult Write()
    {
        ViewData["Title"] = "자유게시판 글쓰기";
        ViewData["Description"] = "대출, 예금, 적금, 연봉 계산과 생활 금융 관련 질문이나 정보를 자유게시판에 작성하세요.";
        return View(new BoardPostCreateViewModel());
    }

    [HttpPost("Write")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Write(BoardPostCreateViewModel model, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "자유게시판 글쓰기";
        ViewData["Description"] = "대출, 예금, 적금, 연봉 계산과 생활 금융 관련 질문이나 정보를 자유게시판에 작성하세요.";

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
        ViewData["Description"] = $"머니계산연구소 자유게시판 게시글: {post.Title}";
        var comments = await boardRepository.GetCommentsAsync(id, cancellationToken);
        return View(new BoardPostDetailsViewModel
        {
            Post = post,
            Comments = comments,
            NewComment = new BoardCommentCreateViewModel { PostId = post.Id },
            Delete = new BoardPostDeleteViewModel { Id = post.Id }
        });
    }

    [HttpGet("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var post = await boardRepository.GetPostAsync(id, incrementViewCount: false, cancellationToken);
        if (post is null)
        {
            return NotFound();
        }

        ViewData["Title"] = "자유게시판 게시글 수정";
        ViewData["Description"] = "머니계산연구소 자유게시판 게시글의 제목과 본문을 수정합니다.";
        return View(new BoardPostEditViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content
        });
    }

    [HttpPost("{id:int}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BoardPostEditViewModel model, CancellationToken cancellationToken = default)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        ViewData["Title"] = "자유게시판 게시글 수정";
        ViewData["Description"] = "머니계산연구소 자유게시판 게시글의 제목과 본문을 수정합니다.";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await boardRepository.UpdatePostAsync(model, cancellationToken);
        if (!updated)
        {
            ModelState.AddModelError("Password", "비밀번호가 일치하지 않습니다.");
            return View(model);
        }

        TempData["BoardMessage"] = "게시글이 수정되었습니다.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
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
            return View("Details", new BoardPostDetailsViewModel
            {
                Post = post,
                Comments = await boardRepository.GetCommentsAsync(id, cancellationToken),
                NewComment = new BoardCommentCreateViewModel { PostId = post.Id },
                Delete = model
            });
        }

        var deleted = await boardRepository.DeletePostAsync(id, model.Password, cancellationToken);
        if (!deleted)
        {
            ModelState.AddModelError("Password", "비밀번호가 일치하지 않습니다.");
            ViewData["Title"] = post.Title;
            return View("Details", new BoardPostDetailsViewModel
            {
                Post = post,
                Comments = await boardRepository.GetCommentsAsync(id, cancellationToken),
                NewComment = new BoardCommentCreateViewModel { PostId = post.Id },
                Delete = model
            });
        }

        TempData["BoardMessage"] = "게시글이 삭제되었습니다.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{postId:int}/Comments")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateComment(
        int postId,
        BoardCommentCreateViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (postId != model.PostId)
        {
            return BadRequest();
        }

        if (!string.IsNullOrWhiteSpace(model.Website))
        {
            ModelState.AddModelError(string.Empty, "댓글을 등록할 수 없습니다.");
        }

        if (!ModelState.IsValid)
        {
            var post = await boardRepository.GetPostAsync(postId, incrementViewCount: false, cancellationToken);
            if (post is null)
            {
                return NotFound();
            }

            ViewData["Title"] = post.Title;
            return View("Details", new BoardPostDetailsViewModel
            {
                Post = post,
                Comments = await boardRepository.GetCommentsAsync(postId, cancellationToken),
                NewComment = model,
                Delete = new BoardPostDeleteViewModel { Id = post.Id }
            });
        }

        var commentId = await boardRepository.CreateCommentAsync(
            model,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers["User-Agent"].ToString(),
            cancellationToken);

        if (commentId == 0)
        {
            return NotFound();
        }

        TempData["BoardMessage"] = "댓글이 등록되었습니다.";
        return RedirectToComment(postId, commentId);
    }

    [HttpPost("{postId:int}/Comments/{commentId:int}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComment(
        int postId,
        int commentId,
        BoardCommentEditViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (postId != model.PostId || commentId != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            TempData["BoardError"] = "댓글 수정 내용을 확인해주세요.";
            return RedirectToComment(postId, commentId);
        }

        var updated = await boardRepository.UpdateCommentAsync(model, cancellationToken);
        TempData[updated ? "BoardMessage" : "BoardError"] = updated ? "댓글이 수정되었습니다." : "댓글 비밀번호가 일치하지 않습니다.";
        return RedirectToComment(postId, commentId);
    }

    [HttpPost("{postId:int}/Comments/{commentId:int}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(
        int postId,
        int commentId,
        BoardCommentDeleteViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (postId != model.PostId || commentId != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            TempData["BoardError"] = "댓글 삭제 비밀번호를 입력해주세요.";
            return RedirectToComment(postId, commentId);
        }

        var deleted = await boardRepository.DeleteCommentAsync(model, cancellationToken);
        TempData[deleted ? "BoardMessage" : "BoardError"] = deleted ? "댓글이 삭제되었습니다." : "댓글 비밀번호가 일치하지 않습니다.";
        return RedirectToAction(nameof(Details), new { id = postId });
    }

    private IActionResult RedirectToComment(int postId, int commentId)
    {
        var url = Url.Action(nameof(Details), new { id = postId }) ?? $"/Board/{postId}";
        return Redirect($"{url}#comment-{commentId}");
    }
}
