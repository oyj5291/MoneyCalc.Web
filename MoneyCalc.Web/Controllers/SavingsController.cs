using Microsoft.AspNetCore.Mvc;
using MoneyCalc.Web.Models.Savings;
using MoneyCalc.Web.Services;

namespace MoneyCalc.Web.Controllers;

public class SavingsController(ISavingsCalculatorService calculatorService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new SavingsCalculatorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(SavingsCalculatorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Result = calculatorService.Calculate(
            model.MonthlyPayment,
            model.AnnualInterestRate,
            model.TermMonths,
            model.InterestType,
            model.TaxType);

        return View(model);
    }
}
