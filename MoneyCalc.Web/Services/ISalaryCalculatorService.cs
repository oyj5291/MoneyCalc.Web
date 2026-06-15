using MoneyCalc.Web.Models.Salary;

namespace MoneyCalc.Web.Services;

public interface ISalaryCalculatorService
{
    SalaryCalculationResult Calculate(
        decimal annualSalary,
        decimal monthlyTaxFreeAmount,
        int dependents);
}
