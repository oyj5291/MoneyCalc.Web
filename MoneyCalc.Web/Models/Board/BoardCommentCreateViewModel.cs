using System.ComponentModel.DataAnnotations;

namespace MoneyCalc.Web.Models.Board;

public class BoardCommentCreateViewModel
{
    public int PostId { get; set; }

    [Required(ErrorMessage = "글쓴이를 입력해주세요.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "글쓴이는 2자 이상 20자 이하로 입력해주세요.")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "댓글 내용을 입력해주세요.")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "댓글은 2자 이상 1,000자 이하로 입력해주세요.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "수정·삭제용 비밀번호를 입력해주세요.")]
    [StringLength(40, MinimumLength = 4, ErrorMessage = "비밀번호는 4자 이상 40자 이하로 입력해주세요.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? Website { get; set; }
}
