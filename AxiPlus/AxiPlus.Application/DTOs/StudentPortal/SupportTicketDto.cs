using AxiPlus.Domain.Enums;

namespace AxiPlus.Application.DTOs.StudentPortal;

public class SupportTicketDto
{
    public Guid Id{ get; set; }

    public string Subject{ get; set; } = string.Empty;

    public string Message{ get; set; } = string.Empty;

    public SupportTicketStatus Status{ get; set; }

    public string MentorResponse{ get; set; } = string.Empty;

    public DateTime CreatedAt{ get; set; }

    public DateTime? UpdatedAt{ get; set; }
}
