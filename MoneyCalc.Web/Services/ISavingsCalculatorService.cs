using MoneyCalc.Web.Models.Deposit;
using MoneyCalc.Web.Models.Savings;

namespace MoneyCalc.Web.Services;

public interface ISavingsCalculatorService
{
    SavingsCalculationResult Calculate(
        decimal monthlyPayment,
        decimal annualInterestRate,
        int termMonths,
        SavingsInterestType interestType,
        TaxType taxType);
}
