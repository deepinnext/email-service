using System;
using Deepin.Email.API.Configurations;
using Deepin.Email.API.Infrastructure;
using Deepin.Email.API.Infrastructure.Services;
using Deepin.EventBus.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Deepin.Email.API.Extensions;

public static class HostingExtensions
{
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddHealthChecks()
            .AddCheck("default", () => HealthCheckResult.Healthy());

        builder.Services.AddDbContext<EmailDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.Configure<SmtpConfiguration>(builder.Configuration.GetSection("Smtp"));

        builder.Services.AddSingleton<IEmailSender>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SmtpConfiguration>>();
            if (options.Value.IsEnabled)
            {
                return new SmtpEmailSender(sp.GetRequiredService<ILogger<SmtpEmailSender>>(), options);
            }
            return new FakeEmailSender(sp.GetRequiredService<ILogger<FakeEmailSender>>());
        });

        builder.Services
        .AddCustomDbContext(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection"))
        .AddCustomEventBus(builder.Configuration)
        .AddMigration<EmailDbContext>();
        return builder;
    }
    private static IServiceCollection AddCustomDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EmailDbContext>(options =>
        {
            options.UseNpgsql(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
            });
        }, ServiceLifetime.Scoped);
        return services;
    }
    private static IServiceCollection AddCustomEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqConfig = configuration.GetSection(RabbitMqConfiguration.ConfigurationKey).Get<RabbitMqConfiguration>();
        if (rabbitMqConfig is null)
        {
            throw new ArgumentNullException(nameof(rabbitMqConfig), "RabbitMQ configuration cannot be null");
        }
        services.AddEventBusRabbitMQ(rabbitMqConfig, typeof(HostingExtensions).Assembly);
        return services;
    }
    public static WebApplication UseApplicationService(this WebApplication app)
    {
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

        return app;
    }
}
