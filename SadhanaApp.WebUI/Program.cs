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




    // Add Key Vault to the configuration pipeline get it from App settings
    //  var keyVaultEndpoint = new Uri(builder.Configuration["KeyVaultEndpoint"]);
    // Create a new secret client using the default credential from Azure.Identity using environment variables previously set
    // var secretClient = new SecretClient(vaultUri: keyVaultEndpoint, credential: new DefaultAzureCredential());

    // Get the secret we created previously
    // KeyVaultSecret secret = secretClient.GetSecret("SadhanaSqlConnection");  // Azure SQL

    // KeyVaultSecret secret = secretClient.GetSecret("DevConnection");  // Local SQL

    // Access the configuration from the builder.
    //var configuration = builder.Configuration;
    //var check = configuration.GetConnectionString("DefaultConnection");

    /*builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(secret.Value));
    */

    // Determine the environment
    var environment = builder.Environment;

    // Get the connection string based on the environment
    string connectionString;

    if (environment.IsDevelopment())
    {
        // For local development, use Secret Manager
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    }
    else
    {
        // For production, use Azure Key Vault
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

    // Configure the HTTP request pipeline.
    //Exception hanlding. Create a middleware and include that here
    // Enable Serilog exception logging
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
