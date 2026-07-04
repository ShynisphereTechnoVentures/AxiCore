namespace AxiForge.Application.Interfaces;

public interface IEmailDeliveryService
{
    Task SendEmailConfirmationAsync(
        string email,
        string fullName,
        string confirmationLink,
        CancellationToken cancellationToken);

    Task SendPasswordResetAsync(
        string email,
        string fullName,
        string resetLink,
        CancellationToken cancellationToken);
}
