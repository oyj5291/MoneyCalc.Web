using MoneyCalc.Web.Models.Loan;

namespace MoneyCalc.Web.Services;

public interface ILoanCalculatorService
{
    LoanCalculationResult Calculate(
        decimal principal,
        decimal annualInterestRate,
        int termMonths,
        RepaymentType repaymentType,
        int gracePeriodMonths);
}
