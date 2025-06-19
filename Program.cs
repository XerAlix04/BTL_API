using BTL.Data;
using BTL.Helpers;
using BTL.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Maintain PascalCase
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Handle circular references
});

builder.Services.AddDbContext<Hshop2023Context>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"))
                .UseLazyLoadingProxies(false);
    }
    );

// Register your ApiService here:
builder.Services.AddScoped<ApiService>();

// Configure HttpClient (if you haven't already)
builder.Services.AddHttpClient<ApiService>(client =>
{
    var configuration = builder.Configuration;
    var baseApiUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7021/api"; // Configure in appsettings.json
    client.BaseAddress = new Uri(baseApiUrl);
    client.Timeout = TimeSpan.FromMinutes(5); // Example: Increase timeout to 5 minutes
    // You can add other default headers or configurations here if needed
});

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly); // Specify the assembly containing your profile

// Add API controllers
builder.Services.AddControllers();

builder.Services.AddScoped<IPasswordHasher<KhachHang>, PasswordHasher<KhachHang>>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/KhachHang/DangNhap";
    options.LogoutPath = "/KhachHang/DangXuat";
    options.AccessDeniedPath = "/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax; // Or Strict depending on your needs
    options.Cookie.Name = ".AspNetCore.Cookies"; // Default cookie name, can be customized
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// HttpContext Accessor
builder.Services.AddHttpContextAccessor();

// API Behavior Configuration
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

//app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Custom middleware for API 404 handling
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 &&
        context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(
            "{\"success\":false,\"message\":\"Resource not found\"}");
    }
});

app.Run();
