using System.ComponentModel.DataAnnotations;
using MoneyCalc.Web.Models.Deposit;

namespace MoneyCalc.Web.Models.Savings;

public class SavingsCalculatorViewModel
{
    [Display(Name = "월 납입액")]
    [Range(1, 10_000_000_000, ErrorMessage = "월 납입액은 1원 이상 입력해 주세요.")]
    public decimal MonthlyPayment { get; set; } = 1_000_000;

    [Display(Name = "연 이자율")]
    [Range(0, 100, ErrorMessage = "연 이자율은 0% 이상 100% 이하로 입력해 주세요.")]
    public decimal AnnualInterestRate { get; set; } = 3.5m;

    [Display(Name = "적금 기간")]
    [Range(1, 600, ErrorMessage = "적금 기간은 1개월 이상 600개월 이하로 입력해 주세요.")]
    public int TermMonths { get; set; } = 12;

    [Display(Name = "이자 계산 방식")]
    public SavingsInterestType InterestType { get; set; } = SavingsInterestType.Simple;

    [Display(Name = "과세 구분")]
    public TaxType TaxType { get; set; } = TaxType.General;

    public SavingsCalculationResult? Result { get; set; }
}

public class SavingsCalculationResult
{
    public decimal TotalPrincipal { get; init; }
    public decimal GrossInterest { get; init; }
    public decimal Tax { get; init; }
    public decimal NetInterest { get; init; }
    public decimal MaturityAmount { get; init; }
    public decimal TaxRate { get; init; }
    public SavingsInterestType InterestType { get; init; }
    public TaxType TaxType { get; init; }
}
