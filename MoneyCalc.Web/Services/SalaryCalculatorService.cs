using MoneyCalc.Web.Models.Salary;

namespace MoneyCalc.Web.Services;

public class SalaryCalculatorService : ISalaryCalculatorService
{
    private const decimal NationalPensionEmployeeRate = 0.0475m;
    private const decimal NationalPensionMonthlyCap = 6_370_000m;
    private const decimal HealthInsuranceEmployeeRate = 0.03595m;
    private const decimal LongTermCareRate = 0.1314m;
    private const decimal EmploymentInsuranceEmployeeRate = 0.009m;

    public SalaryCalculationResult Calculate(
        decimal annualSalary,
        decimal monthlyTaxFreeAmount,
        int dependents)
    {
        var monthlyGrossSalary = annualSalary / 12m;
        var monthlyTaxableSalary = Math.Max(0, monthlyGrossSalary - monthlyTaxFreeAmount);

        var nationalPension = RoundDown(
            Math.Min(monthlyTaxableSalary, NationalPensionMonthlyCap) *
            NationalPensionEmployeeRate);
        var healthInsurance = RoundDown(monthlyTaxableSalary * HealthInsuranceEmployeeRate);
        var longTermCareInsurance = RoundDown(healthInsurance * LongTermCareRate);
        var employmentInsurance = RoundDown(monthlyTaxableSalary * EmploymentInsuranceEmployeeRate);

        var annualSocialInsurance =
            (nationalPension + healthInsurance + longTermCareInsurance + employmentInsurance) * 12m;
        var annualIncomeTax = CalculateAnnualIncomeTax(
            annualSalary,
            monthlyTaxFreeAmount * 12m,
            annualSocialInsurance,
            dependents);
        var incomeTax = RoundDown(annualIncomeTax / 12m);
        var localIncomeTax = RoundDown(incomeTax * 0.1m);
        var totalDeduction =
            nationalPension +
            healthInsurance +
            longTermCareInsurance +
            employmentInsurance +
            incomeTax +
            localIncomeTax;

        return new SalaryCalculationResult
        {
            MonthlyGrossSalary = Round(monthlyGrossSalary),
            NationalPension = nationalPension,
            HealthInsurance = healthInsurance,
            LongTermCareInsurance = longTermCareInsurance,
            EmploymentInsurance = employmentInsurance,
            IncomeTax = incomeTax,
            LocalIncomeTax = localIncomeTax,
            TotalDeduction = totalDeduction,
            MonthlyNetSalary = Round(monthlyGrossSalary - totalDeduction)
        };
    }

    private static decimal CalculateAnnualIncomeTax(
        decimal annualSalary,
        decimal annualTaxFreeAmount,
        decimal annualSocialInsurance,
        int dependents)
    {
        var taxableSalary = Math.Max(0, annualSalary - annualTaxFreeAmount);
        var earnedIncomeDeduction = CalculateEarnedIncomeDeduction(taxableSalary);
        var personalDeduction = dependents * 1_500_000m;
        var taxBase = Math.Max(
            0,
            taxableSalary - earnedIncomeDeduction - personalDeduction - annualSocialInsurance);
        var calculatedTax = CalculateProgressiveTax(taxBase);
        var earnedIncomeTaxCredit = Math.Min(
            740_000m,
            calculatedTax <= 1_300_000m
                ? calculatedTax * 0.55m
                : 715_000m + (calculatedTax - 1_300_000m) * 0.30m);

        return Round(Math.Max(0, calculatedTax - earnedIncomeTaxCredit));
    }

    private static decimal CalculateEarnedIncomeDeduction(decimal salary)
    {
        var deduction = salary switch
        {
            <= 5_000_000m => salary * 0.70m,
            <= 15_000_000m => 3_500_000m + (salary - 5_000_000m) * 0.40m,
            <= 45_000_000m => 7_500_000m + (salary - 15_000_000m) * 0.15m,
            <= 100_000_000m => 12_000_000m + (salary - 45_000_000m) * 0.05m,
            _ => 14_750_000m + (salary - 100_000_000m) * 0.02m
        };

        return Math.Min(deduction, 20_000_000m);
    }

    private static decimal CalculateProgressiveTax(decimal taxBase)
    {
        return taxBase switch
        {
            <= 14_000_000m => taxBase * 0.06m,
            <= 50_000_000m => taxBase * 0.15m - 1_260_000m,
            <= 88_000_000m => taxBase * 0.24m - 5_760_000m,
            <= 150_000_000m => taxBase * 0.35m - 15_440_000m,
            <= 300_000_000m => taxBase * 0.38m - 19_940_000m,
            <= 500_000_000m => taxBase * 0.40m - 25_940_000m,
            <= 1_000_000_000m => taxBase * 0.42m - 35_940_000m,
            _ => taxBase * 0.45m - 65_940_000m
        };
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 0, MidpointRounding.AwayFromZero);

    private static decimal RoundDown(decimal value) =>
        Math.Floor(value / 10m) * 10m;
}
