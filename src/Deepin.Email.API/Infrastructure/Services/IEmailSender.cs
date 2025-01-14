using System;

namespace Deepin.Email.API.Infrastructure.Services;

public interface IEmailSender
{
    Task SendAsync(string from, string fromDisplayName, string[] to, string subject, string body, bool isBodyHtml = true, string[]? cc = null);
}
