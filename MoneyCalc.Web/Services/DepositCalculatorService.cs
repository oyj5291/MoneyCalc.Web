using MoneyCalc.Web.Models.Deposit;

namespace MoneyCalc.Web.Services;

public class DepositCalculatorService : IDepositCalculatorService
{
    public DepositCalculationResult Calculate(
        decimal principal,
        decimal annualInterestRate,
        int termMonths,
        InterestCalculationType interestCalculationType,
        TaxType taxType)
    {
        var annualRate = annualInterestRate / 100m;
        var grossInterest = interestCalculationType switch
        {
            InterestCalculationType.Simple =>
                principal * annualRate * termMonths / 12m,
            InterestCalculationType.MonthlyCompound =>
                principal *
                ((decimal)Math.Pow(1 + (double)(annualRate / 12m), termMonths) - 1),
            InterestCalculationType.AnnualCompound =>
                CalculateAnnualCompoundInterest(principal, annualRate, termMonths),
            _ => throw new ArgumentOutOfRangeException(nameof(interestCalculationType))
        };

        var taxRate = taxType switch
        {
            TaxType.General => 0.154m,
            TaxType.TaxPreferred => 0.095m,
            TaxType.TaxExempt => 0m,
            _ => throw new ArgumentOutOfRangeException(nameof(taxType))
        };
        var roundedGrossInterest = Round(grossInterest);
        var tax = Round(roundedGrossInterest * taxRate);
        var netInterest = roundedGrossInterest - tax;

        return new DepositCalculationResult
        {
            Principal = Round(principal),
            GrossInterest = roundedGrossInterest,
            Tax = tax,
            NetInterest = netInterest,
            MaturityAmount = Round(principal) + netInterest,
            TaxRate = taxRate * 100m,
            InterestCalculationType = interestCalculationType,
            TaxType = taxType
        };
    }

    private static decimal CalculateAnnualCompoundInterest(
        decimal principal,
        decimal annualRate,
        int termMonths)
    {
        var fullYears = termMonths / 12;
        var remainingMonths = termMonths % 12;
        var balance = principal * (decimal)Math.Pow(1 + (double)annualRate, fullYears);
        balance *= 1 + annualRate * remainingMonths / 12m;
        return balance - principal;
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 0, MidpointRounding.AwayFromZero);
}
