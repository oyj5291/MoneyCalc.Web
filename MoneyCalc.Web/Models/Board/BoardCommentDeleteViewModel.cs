using System.ComponentModel.DataAnnotations;

namespace MoneyCalc.Web.Models.Board;

public class BoardCommentDeleteViewModel
{
    public int Id { get; set; }

    public int PostId { get; set; }

    [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
    [StringLength(40, MinimumLength = 4, ErrorMessage = "비밀번호는 4자 이상 40자 이하로 입력해주세요.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
