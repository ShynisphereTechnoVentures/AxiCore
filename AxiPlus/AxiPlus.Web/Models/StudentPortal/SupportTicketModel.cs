using AxiPlus.Domain.Enums;

namespace AxiPlus.Web.Models.StudentPortal;

public class SupportTicketModel
{
    public Guid Id{get;set; }

    public string Subject{get;set; } = string.Empty;

    public string Message{get;set; } = string.Empty;

    public SupportTicketStatus Status{get;set; }

    public string MentorResponse{get;set; } = string.Empty;

    public DateTime CreatedAt{get;set; }

    public DateTime? UpdatedAt{get;set; }
}
