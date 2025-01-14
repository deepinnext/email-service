using Deepin.Email.API.Configurations;
using Deepin.Email.API.Domain.Entities;
using Deepin.Email.API.Infrastructure;
using Deepin.Email.API.Infrastructure.Services;
using Deepin.EventBus;
using Deepin.EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Deepin.Email.API.Application.IntegrationEvents.EventHandling;

public class SendEmailIntegrationEventHandler(ILogger<SendEmailIntegrationEventHandler> logger, IOptions<SmtpConfiguration> options, IEmailSender emailSender, EmailDbContext db) : IIntegrationEventHandler<SendEmailIntegrationEvent>
{
    private readonly ILogger<SendEmailIntegrationEventHandler> _logger = logger;
    private readonly SmtpConfiguration _smtpConfiguration = options.Value;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly EmailDbContext _db = db;

    public async Task Consume(ConsumeContext<SendEmailIntegrationEvent> context)
    {
        try
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", context.MessageId, context.Message);

            var @event = context.Message;
            await _emailSender.SendAsync(_smtpConfiguration.FromAddress, _smtpConfiguration.FromDisplayName, @event.To, @event.Subject, @event.Body, true, @event.CC);
            var email = new MailObject(from: _smtpConfiguration.FromAddress, to: string.Join(";", @event.To), subject: @event.Subject, body: @event.Body, isBodyHtml: true, cc: @event.CC == null ? null : string.Join(";", @event.CC));
            _db.MailObjects.Add(email);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Processing message: {Message}", ex.Message);
        }
    }
}
