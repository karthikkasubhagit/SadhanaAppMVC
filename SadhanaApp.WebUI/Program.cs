using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SadhanaApp.WebUI.Middleware;
using Serilog;
using System;
using System.Net;
using Microsoft.AspNetCore.Authentication.Google;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Persistence.Repository;



// Configure Serilog with the settings
Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.WriteTo.Debug()
.MinimumLevel.Information()
.Enrich.FromLogContext()
.CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
            options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
        });

    builder.Services.AddApplicationInsightsTelemetry();

    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
               .WriteTo.ApplicationInsights(
                 services.GetRequiredService<TelemetryConfiguration>(),
                 TelemetryConverter.Events));

    Log.Information("Starting the application...");
    // Add services to the container.




    //var instrumentationKey = "d0f0d672-8b27-46e3-b686-cc2317ecc440";



    builder.Services.AddControllersWithViews();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
        });

  

    // Determine the environment
    var environment = builder.Environment;

    // Get the connection string based on the environment
    string connectionString;

    if (environment.IsDevelopment())
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    }
    else
    {
        var keyVaultEndpoint = new Uri(builder.Configuration["KeyVaultEndpoint"]);
        var secretClient = new SecretClient(vaultUri: keyVaultEndpoint, credential: new DefaultAzureCredential());
        KeyVaultSecret secret = secretClient.GetSecret("SadhanaSqlConnection");
        connectionString = secret.Value;
    }

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    var app = builder.Build();

    
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            Log.Error(exception, "Unhandled exception occurred. {ExceptionDetails}", exception?.ToString());
            Console.WriteLine(exception?.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
        });
    });

    app.UseMiddleware<RequestResponseLoggingMiddleware>();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
