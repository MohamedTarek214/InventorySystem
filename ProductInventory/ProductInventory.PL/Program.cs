using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using ProductInventory.BLL.Services;
using ProductInventory.DAL.Data;
using ProductInventory.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Only properties marked with [Required] are required (non-nullable strings are not implicitly required).
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// ---------- Dependency Injection (composition root) ----------
// Controllers only know IProductService. The wiring below is the only place that
// mentions the DbContext and the repository.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Use "." as the decimal separator regardless of the Windows language (prices like 10.50 bind correctly).
var culture = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new List<CultureInfo> { culture },
    SupportedUICultures = new List<CultureInfo> { culture }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 404 and other status codes get a friendly page.
app.UseStatusCodePagesWithReExecute("/Home/HttpStatus", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
