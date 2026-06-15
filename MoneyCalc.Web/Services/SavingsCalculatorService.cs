using MoneyCalc.Web.Models.Deposit;
using MoneyCalc.Web.Models.Savings;

namespace MoneyCalc.Web.Services;

public class SavingsCalculatorService : ISavingsCalculatorService
{
    public SavingsCalculationResult Calculate(
        decimal monthlyPayment,
        decimal annualInterestRate,
        int termMonths,
        SavingsInterestType interestType,
        TaxType taxType)
    {
        var monthlyRate = annualInterestRate / 100m / 12m;
        var grossInterest = interestType switch
        {
            SavingsInterestType.Simple =>
                CalculateSimpleInterest(monthlyPayment, monthlyRate, termMonths),
            SavingsInterestType.MonthlyCompound =>
                CalculateMonthlyCompoundInterest(monthlyPayment, monthlyRate, termMonths),
            _ => throw new ArgumentOutOfRangeException(nameof(interestType))
        };

        var taxRate = taxType switch
        {
            TaxType.General => 0.154m,
            TaxType.TaxPreferred => 0.095m,
            TaxType.TaxExempt => 0m,
            _ => throw new ArgumentOutOfRangeException(nameof(taxType))
        };
        var totalPrincipal = Round(monthlyPayment * termMonths);
        var roundedGrossInterest = Round(grossInterest);
        var tax = Round(roundedGrossInterest * taxRate);
        var netInterest = roundedGrossInterest - tax;

        return new SavingsCalculationResult
        {
            TotalPrincipal = totalPrincipal,
            GrossInterest = roundedGrossInterest,
            Tax = tax,
            NetInterest = netInterest,
            MaturityAmount = totalPrincipal + netInterest,
            TaxRate = taxRate * 100m,
            InterestType = interestType,
            TaxType = taxType
        };
    }

    private static decimal CalculateSimpleInterest(
        decimal monthlyPayment,
        decimal monthlyRate,
        int termMonths)
    {
        var totalInterestMonths = termMonths * (termMonths + 1m) / 2m;
        return monthlyPayment * monthlyRate * totalInterestMonths;
    }

    private static decimal CalculateMonthlyCompoundInterest(
        decimal monthlyPayment,
        decimal monthlyRate,
        int termMonths)
    {
        if (monthlyRate == 0)
        {
            return 0;
        }

        var maturityAmount = monthlyPayment *
            (1 + monthlyRate) *
            ((decimal)Math.Pow(1 + (double)monthlyRate, termMonths) - 1) /
            monthlyRate;

        return maturityAmount - monthlyPayment * termMonths;
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 0, MidpointRounding.AwayFromZero);
}
