using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:5000");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddCheck("default", () => HealthCheckResult.Healthy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UsePathBase("/email");
}
app.UseHttpsRedirection();

app.MapHealthChecks("health");

app.MapGet("/api/about", () => new
{
    Name = "Deepin.Email.API",
    Version = "1.0.0",
    DeepinEnv = app.Configuration["DEEPIN_ENV"],
    EnvironmentName = app.Environment.EnvironmentName
});

app.Run();
