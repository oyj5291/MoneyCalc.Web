using System.ComponentModel.DataAnnotations;

namespace MoneyCalc.Web.Models.Loan;

public class LoanCalculatorViewModel : IValidatableObject
{
    [Display(Name = "대출 원금")]
    [Range(1, 100_000_000_000, ErrorMessage = "대출 원금은 1원 이상 입력해 주세요.")]
    public decimal Principal { get; set; } = 100_000_000;

    [Display(Name = "연 이자율")]
    [Range(0, 100, ErrorMessage = "연 이자율은 0% 이상 100% 이하로 입력해 주세요.")]
    public decimal AnnualInterestRate { get; set; } = 4.5m;

    [Display(Name = "대출 기간")]
    [Range(1, 600, ErrorMessage = "대출 기간은 1개월 이상 600개월 이하로 입력해 주세요.")]
    public int TermMonths { get; set; } = 360;

    [Display(Name = "거치기간")]
    [Range(0, 599, ErrorMessage = "거치기간은 0개월 이상 599개월 이하로 입력해 주세요.")]
    public int GracePeriodMonths { get; set; }

    [Display(Name = "상환 방식")]
    public RepaymentType RepaymentType { get; set; } = RepaymentType.EqualPayment;

    public LoanCalculationResult? Result { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RepaymentType != RepaymentType.Bullet && GracePeriodMonths >= TermMonths)
        {
            yield return new ValidationResult(
                "거치기간은 대출 기간보다 짧아야 합니다.",
                [nameof(GracePeriodMonths)]);
        }
    }
}

public class LoanCalculationResult
{
    public RepaymentType RepaymentType { get; init; }
    public int GracePeriodMonths { get; init; }
    public decimal MonthlyPayment { get; init; }
    public decimal FirstPayment { get; init; }
    public decimal LastPayment { get; init; }
    public decimal TotalPayment { get; init; }
    public decimal TotalInterest { get; init; }
    public IReadOnlyList<LoanPaymentItem> Schedule { get; init; } = [];
}

public class LoanPaymentItem
{
    public int Month { get; init; }
    public decimal Payment { get; init; }
    public decimal Principal { get; init; }
    public decimal Interest { get; init; }
    public decimal Balance { get; init; }
    public bool IsGracePeriod { get; init; }
}
