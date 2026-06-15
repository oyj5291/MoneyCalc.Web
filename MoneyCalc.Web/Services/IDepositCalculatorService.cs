using MoneyCalc.Web.Models.Deposit;

namespace MoneyCalc.Web.Services;

public interface IDepositCalculatorService
{
    DepositCalculationResult Calculate(
        decimal principal,
        decimal annualInterestRate,
        int termMonths,
        InterestCalculationType interestCalculationType,
        TaxType taxType);
}
