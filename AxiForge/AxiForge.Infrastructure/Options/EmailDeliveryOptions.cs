namespace AxiForge.Infrastructure.Options;

public sealed class EmailDeliveryOptions
{
    public string Mode { get; set; } = "Console";

    public string FromEmail { get; set; } = "no-reply@axionora.local";

    public string AppBaseUrl { get; set; } = "http://localhost:5242";

    public SmtpDeliveryOptions Smtp { get; set; } = new();
}

public sealed class SmtpDeliveryOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool EnableSsl { get; set; } = true;
}
