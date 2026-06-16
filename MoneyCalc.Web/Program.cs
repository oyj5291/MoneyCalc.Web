using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<MoneyCalc.Web.Services.ILoanCalculatorService, MoneyCalc.Web.Services.LoanCalculatorService>();
builder.Services.AddScoped<MoneyCalc.Web.Services.IDepositCalculatorService, MoneyCalc.Web.Services.DepositCalculatorService>();
builder.Services.AddScoped<MoneyCalc.Web.Services.ISavingsCalculatorService, MoneyCalc.Web.Services.SavingsCalculatorService>();
builder.Services.AddScoped<MoneyCalc.Web.Services.ISalaryCalculatorService, MoneyCalc.Web.Services.SalaryCalculatorService>();
builder.Services.AddSingleton<MoneyCalc.Web.Services.IBoardPasswordHasher, MoneyCalc.Web.Services.BoardPasswordHasher>();
builder.Services.AddSingleton<MoneyCalc.Web.Services.IBoardRepository, MoneyCalc.Web.Services.PostgresBoardRepository>();

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(app.Configuration.GetConnectionString("BoardDatabase")))
{
    using var scope = app.Services.CreateScope();
    var boardRepository = scope.ServiceProvider.GetRequiredService<MoneyCalc.Web.Services.IBoardRepository>();
    await boardRepository.EnsureCreatedAsync();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapGet("/healthz", () => Results.Ok(new
{
    status = "healthy",
    service = "MoneyCalc.Web"
}));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
