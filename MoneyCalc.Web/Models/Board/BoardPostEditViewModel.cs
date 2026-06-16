using System.ComponentModel.DataAnnotations;

namespace MoneyCalc.Web.Models.Board;

public class BoardPostEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "제목을 입력해주세요.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "제목은 2자 이상 80자 이하로 입력해주세요.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "본문을 입력해주세요.")]
    [StringLength(4000, MinimumLength = 10, ErrorMessage = "본문은 10자 이상 4,000자 이하로 입력해주세요.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "작성 시 입력한 비밀번호를 입력해주세요.")]
    [StringLength(40, MinimumLength = 4, ErrorMessage = "비밀번호는 4자 이상 40자 이하로 입력해주세요.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
