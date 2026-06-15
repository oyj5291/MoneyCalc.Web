using Microsoft.AspNetCore.Mvc;

namespace MoneyCalc.Web.Controllers;

[Route("Finance")]
public class FinanceController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("LoanGuide")]
    public IActionResult LoanGuide() => View();

    [HttpGet("DepositSavingsGuide")]
    public IActionResult DepositSavingsGuide() => View();

    [HttpGet("InterestGuide")]
    public IActionResult InterestGuide() => View();

    [HttpGet("Salary5000Guide")]
    public IActionResult Salary5000Guide() => View();
}
