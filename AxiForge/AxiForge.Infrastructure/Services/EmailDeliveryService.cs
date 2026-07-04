using System.Net;
using System.Net.Mail;
using AxiCore.Diagnostics;
using AxiForge.Application.Interfaces;
using AxiForge.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AxiForge.Infrastructure.Services;

public sealed class EmailDeliveryService : IEmailDeliveryService
{
    private readonly EmailDeliveryOptions _options;
    private readonly ILogger<EmailDeliveryService> _logger;

    public EmailDeliveryService(
        IOptions<EmailDeliveryOptions> options,
        ILogger<EmailDeliveryService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends an email-confirmation message through SMTP when configured, otherwise writes the message to logs.
    /// Returns no value because delivery is a side effect of account provisioning.
    /// </summary>
    public Task SendEmailConfirmationAsync(
        string email,
        string fullName,
        string confirmationLink,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(EmailDeliveryService), nameof(SendEmailConfirmationAsync));
        try
        {
            return SendAsync(
                email,
                "Confirm your AxiForge email",
                $"Hi {fullName}, confirm your AxiForge email here: {confirmationLink}",
                cancellationToken);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Sends a password reset message through SMTP when configured, otherwise writes the message to logs.
    /// Returns no value because delivery is handled by the configured provider.
    /// </summary>
    public Task SendPasswordResetAsync(
        string email,
        string fullName,
        string resetLink,
        CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(EmailDeliveryService), nameof(SendPasswordResetAsync));
        try
        {
            return SendAsync(
                email,
                "Reset your AxiForge password",
                $"Hi {fullName}, reset your AxiForge password here: {resetLink}",
                cancellationToken);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    private async Task SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        if (!IsSmtpEnabled())
        {
            _logger.LogWarning(
                "EMAIL -> AxiForge -> {Subject} -> To:{Email} -> Body:{Body}",
                subject,
                toEmail,
                body);
            return;
        }

        using var message = new MailMessage(_options.FromEmail, toEmail, subject, body);
        using var client = new SmtpClient(_options.Smtp.Host, _options.Smtp.Port)
        {
            EnableSsl = _options.Smtp.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_options.Smtp.Username))
        {
            client.Credentials = new NetworkCredential(
                _options.Smtp.Username,
                _options.Smtp.Password);
        }

        await client.SendMailAsync(message, cancellationToken);
    }

    private bool IsSmtpEnabled() =>
        _options.Mode.Equals("Smtp", StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(_options.Smtp.Host);
}
