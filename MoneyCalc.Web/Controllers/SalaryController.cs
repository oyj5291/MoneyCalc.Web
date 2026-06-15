using Microsoft.AspNetCore.Mvc;
using MoneyCalc.Web.Models.Salary;
using MoneyCalc.Web.Services;

namespace MoneyCalc.Web.Controllers;

public class SalaryController(ISalaryCalculatorService calculatorService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new SalaryCalculatorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(SalaryCalculatorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Result = calculatorService.Calculate(
            model.AnnualSalary,
            model.MonthlyTaxFreeAmount,
            model.Dependents);

        return View(model);
    }
}
