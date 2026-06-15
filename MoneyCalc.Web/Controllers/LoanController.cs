using Microsoft.AspNetCore.Mvc;
using MoneyCalc.Web.Models.Loan;
using MoneyCalc.Web.Services;

namespace MoneyCalc.Web.Controllers;

public class LoanController(ILoanCalculatorService calculatorService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new LoanCalculatorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(LoanCalculatorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Result = calculatorService.Calculate(
            model.Principal,
            model.AnnualInterestRate,
            model.TermMonths,
            model.RepaymentType,
            model.GracePeriodMonths);

        return View(model);
    }
}
