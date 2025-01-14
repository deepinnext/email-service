using System;
using System.Net.Mail;
using Deepin.Email.API.Configurations;
using Microsoft.Extensions.Options;

namespace Deepin.Email.API.Infrastructure.Services;

public class SmtpEmailSender(ILogger<SmtpEmailSender> logger, IOptions<SmtpConfiguration> options) : IEmailSender
{
    private readonly ILogger<SmtpEmailSender> _logger = logger;
    private readonly SmtpConfiguration _smtpConfiguration = options.Value;
    public async Task SendAsync(string from, string fromDisplayName, string[] to, string subject, string body, bool isBodyHtml = true, string[]? cc = null)
    {
        MailMessage mailMessage = new MailMessage();
        foreach (var destination in to)
        {
            mailMessage.To.Add(new MailAddress(destination));
        }
        mailMessage.From = new MailAddress(from, fromDisplayName);
        if (cc != null)
        {
            foreach (var destination in cc)
            {
                mailMessage.CC.Add(new MailAddress(destination));
            }
        }
        if (!string.IsNullOrEmpty(_smtpConfiguration.ReplyTo))
        {
            mailMessage.ReplyToList.Add(new MailAddress(_smtpConfiguration.ReplyTo));
        }
        mailMessage.Subject = subject;

        mailMessage.Body = body;
        mailMessage.IsBodyHtml = isBodyHtml;

        // add attachments
        // string file = "D:\\1.txt";
        // Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
        // mailMessage.Attachments.Add(data); 

        SmtpClient smtpClient = new SmtpClient(_smtpConfiguration.Host, _smtpConfiguration.Port);
        smtpClient.EnableSsl = true;

        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(_smtpConfiguration.FromAddress, _smtpConfiguration.Password);
        smtpClient.Credentials = credentials;
        await smtpClient.SendMailAsync(mailMessage);
    }
}
