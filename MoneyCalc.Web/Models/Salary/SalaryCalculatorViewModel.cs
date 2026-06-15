using System.ComponentModel.DataAnnotations;

namespace MoneyCalc.Web.Models.Salary;

public class SalaryCalculatorViewModel
{
    [Display(Name = "연봉")]
    [Range(1, 10_000_000_000, ErrorMessage = "연봉은 1원 이상 입력해 주세요.")]
    public decimal AnnualSalary { get; set; } = 50_000_000;

    [Display(Name = "월 비과세액")]
    [Range(0, 10_000_000, ErrorMessage = "월 비과세액은 0원 이상 입력해 주세요.")]
    public decimal MonthlyTaxFreeAmount { get; set; } = 200_000;

    [Display(Name = "부양가족 수")]
    [Range(1, 20, ErrorMessage = "부양가족 수는 본인을 포함해 1명 이상 입력해 주세요.")]
    public int Dependents { get; set; } = 1;

    public SalaryCalculationResult? Result { get; set; }
}

public class SalaryCalculationResult
{
    public decimal MonthlyGrossSalary { get; init; }
    public decimal NationalPension { get; init; }
    public decimal HealthInsurance { get; init; }
    public decimal LongTermCareInsurance { get; init; }
    public decimal EmploymentInsurance { get; init; }
    public decimal IncomeTax { get; init; }
    public decimal LocalIncomeTax { get; init; }
    public decimal TotalDeduction { get; init; }
    public decimal MonthlyNetSalary { get; init; }
}
