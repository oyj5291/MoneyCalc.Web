using System.ComponentModel.DataAnnotations;

namespace MoneyCalc.Web.Models.Deposit;

public class DepositCalculatorViewModel
{
    [Display(Name = "예치금액")]
    [Range(1, 100_000_000_000, ErrorMessage = "예치금액은 1원 이상 입력해 주세요.")]
    public decimal Principal { get; set; } = 10_000_000;

    [Display(Name = "연 이자율")]
    [Range(0, 100, ErrorMessage = "연 이자율은 0% 이상 100% 이하로 입력해 주세요.")]
    public decimal AnnualInterestRate { get; set; } = 3.5m;

    [Display(Name = "예치기간")]
    [Range(1, 600, ErrorMessage = "예치기간은 1개월 이상 600개월 이하로 입력해 주세요.")]
    public int TermMonths { get; set; } = 12;

    [Display(Name = "이자 계산 방식")]
    public InterestCalculationType InterestCalculationType { get; set; } = InterestCalculationType.Simple;

    [Display(Name = "과세 구분")]
    public TaxType TaxType { get; set; } = TaxType.General;

    public DepositCalculationResult? Result { get; set; }
}

public class DepositCalculationResult
{
    public decimal Principal { get; init; }
    public decimal GrossInterest { get; init; }
    public decimal Tax { get; init; }
    public decimal NetInterest { get; init; }
    public decimal MaturityAmount { get; init; }
    public decimal TaxRate { get; init; }
    public InterestCalculationType InterestCalculationType { get; init; }
    public TaxType TaxType { get; init; }
}
