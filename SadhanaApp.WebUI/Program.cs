using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });


// Add Key Vault to the configuration pipeline get it from App settings
var keyVaultEndpoint = new Uri(builder.Configuration["KeyVaultEndpoint"]);
// Create a new secret client using the default credential from Azure.Identity using environment variables previously set
var secretClient = new SecretClient(vaultUri: keyVaultEndpoint, credential: new DefaultAzureCredential());

// Get the secret we created previously
KeyVaultSecret secret = secretClient.GetSecret("SadhanaSqlConnection");  // Azure SQL

//KeyVaultSecret secret = secretClient.GetSecret("DevConnection");  // Local SQL




builder.Services.AddApplicationInsightsTelemetry();
// Access the configuration from the builder.
//var configuration = builder.Configuration;
//var check = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(secret.Value));

builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            // Log the exception to Application Insights using RequestServices
            var telemetryClient = context.RequestServices.GetService<TelemetryClient>();
            telemetryClient?.TrackException(exception);

            // Redirect to a generic error page
            context.Response.Redirect("/Home/Error");
        });
    });
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
