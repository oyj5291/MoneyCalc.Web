using Microsoft.AspNetCore.Mvc;
using MoneyCalc.Web.Models.Deposit;
using MoneyCalc.Web.Services;

namespace MoneyCalc.Web.Controllers;

public class DepositController(IDepositCalculatorService calculatorService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new DepositCalculatorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(DepositCalculatorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Result = calculatorService.Calculate(
            model.Principal,
            model.AnnualInterestRate,
            model.TermMonths,
            model.InterestCalculationType,
            model.TaxType);

        return View(model);
    }
}
