using MoneyCalc.Web.Models.Loan;

namespace MoneyCalc.Web.Services;

public class LoanCalculatorService : ILoanCalculatorService
{
    public LoanCalculationResult Calculate(
        decimal principal,
        decimal annualInterestRate,
        int termMonths,
        RepaymentType repaymentType,
        int gracePeriodMonths)
    {
        var monthlyRate = annualInterestRate / 100m / 12m;
        var appliedGracePeriod = repaymentType == RepaymentType.Bullet ? 0 : gracePeriodMonths;
        var repaymentMonths = termMonths - appliedGracePeriod;
        var schedule = CreateGracePeriod(principal, monthlyRate, appliedGracePeriod);

        schedule.AddRange(repaymentType switch
        {
            RepaymentType.EqualPayment => CalculateEqualPayment(
                principal, monthlyRate, repaymentMonths, appliedGracePeriod),
            RepaymentType.EqualPrincipal => CalculateEqualPrincipal(
                principal, monthlyRate, repaymentMonths, appliedGracePeriod),
            RepaymentType.Bullet => CalculateBullet(principal, monthlyRate, termMonths),
            _ => throw new ArgumentOutOfRangeException(nameof(repaymentType))
        });

        var firstRepaymentIndex = Math.Min(appliedGracePeriod, schedule.Count - 1);

        return new LoanCalculationResult
        {
            RepaymentType = repaymentType,
            GracePeriodMonths = appliedGracePeriod,
            MonthlyPayment = schedule[firstRepaymentIndex].Payment,
            FirstPayment = schedule[firstRepaymentIndex].Payment,
            LastPayment = schedule[^1].Payment,
            TotalPayment = schedule.Sum(item => item.Payment),
            TotalInterest = schedule.Sum(item => item.Interest),
            Schedule = schedule
        };
    }

    private static List<LoanPaymentItem> CreateGracePeriod(
        decimal principal,
        decimal monthlyRate,
        int gracePeriodMonths)
    {
        var schedule = new List<LoanPaymentItem>(gracePeriodMonths);
        var interest = principal * monthlyRate;

        for (var month = 1; month <= gracePeriodMonths; month++)
        {
            schedule.Add(CreateItem(month, interest, 0, interest, principal, true));
        }

        return schedule;
    }

    private static List<LoanPaymentItem> CalculateEqualPayment(
        decimal principal,
        decimal monthlyRate,
        int repaymentMonths,
        int monthOffset)
    {
        var factor = (decimal)Math.Pow(1 + (double)monthlyRate, repaymentMonths);
        var payment = monthlyRate == 0
            ? principal / repaymentMonths
            : principal * monthlyRate * factor / (factor - 1);
        var balance = principal;
        var schedule = new List<LoanPaymentItem>(repaymentMonths);

        for (var index = 1; index <= repaymentMonths; index++)
        {
            var interest = balance * monthlyRate;
            var principalPayment = index == repaymentMonths ? balance : payment - interest;
            var actualPayment = principalPayment + interest;
            balance -= principalPayment;
            schedule.Add(CreateItem(
                monthOffset + index, actualPayment, principalPayment, interest, balance));
        }

        return schedule;
    }

    private static List<LoanPaymentItem> CalculateEqualPrincipal(
        decimal principal,
        decimal monthlyRate,
        int repaymentMonths,
        int monthOffset)
    {
        var basePrincipalPayment = principal / repaymentMonths;
        var balance = principal;
        var schedule = new List<LoanPaymentItem>(repaymentMonths);

        for (var index = 1; index <= repaymentMonths; index++)
        {
            var interest = balance * monthlyRate;
            var principalPayment = index == repaymentMonths ? balance : basePrincipalPayment;
            balance -= principalPayment;
            schedule.Add(CreateItem(
                monthOffset + index,
                principalPayment + interest,
                principalPayment,
                interest,
                balance));
        }

        return schedule;
    }

    private static List<LoanPaymentItem> CalculateBullet(
        decimal principal,
        decimal monthlyRate,
        int termMonths)
    {
        var schedule = new List<LoanPaymentItem>(termMonths);
        var monthlyInterest = principal * monthlyRate;

        for (var month = 1; month <= termMonths; month++)
        {
            var principalPayment = month == termMonths ? principal : 0;
            var balance = month == termMonths ? 0 : principal;
            schedule.Add(CreateItem(
                month,
                principalPayment + monthlyInterest,
                principalPayment,
                monthlyInterest,
                balance,
                month < termMonths));
        }

        return schedule;
    }

    private static LoanPaymentItem CreateItem(
        int month,
        decimal payment,
        decimal principal,
        decimal interest,
        decimal balance,
        bool isGracePeriod = false)
    {
        return new LoanPaymentItem
        {
            Month = month,
            Payment = Round(payment),
            Principal = Round(principal),
            Interest = Round(interest),
            Balance = Round(Math.Max(0, balance)),
            IsGracePeriod = isGracePeriod
        };
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 0, MidpointRounding.AwayFromZero);
}
