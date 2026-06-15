using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MoneyCalc.Web.Controllers;

public class SeoController : Controller
{
    private static readonly string[] SitemapPaths =
    [
        "/",
        "/Loan",
        "/Deposit",
        "/Savings",
        "/Salary",
        "/Finance",
        "/Finance/LoanGuide",
        "/Finance/DepositSavingsGuide",
        "/Finance/InterestGuide",
        "/Finance/Salary5000Guide"
    ];

    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 3600)]
    public IActionResult Sitemap()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var xml = new StringBuilder()
            .AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""")
            .AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");

        foreach (var path in SitemapPaths)
        {
            xml.AppendLine("  <url>")
                .Append("    <loc>")
                .Append(baseUrl)
                .Append(path)
                .AppendLine("</loc>")
                .AppendLine("  </url>");
        }

        xml.AppendLine("</urlset>");
        return Content(xml.ToString(), "application/xml", Encoding.UTF8);
    }

    [HttpGet("/robots.txt")]
    [ResponseCache(Duration = 3600)]
    public IActionResult Robots()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var content = $"""
            User-agent: *
            Allow: /

            Sitemap: {baseUrl}/sitemap.xml
            """;

        return Content(content, "text/plain", Encoding.UTF8);
    }
}
