using AxiPlus.Domain.Common;
using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class SupportTicket : BaseEntity
{
    public Guid StudentId{ get; set; }

    public Student Student{ get; set; } = null!;

    public string Subject{ get; set; } = string.Empty;

    public string Message{ get; set; } = string.Empty;

    public SupportTicketStatus Status{ get; set; } = SupportTicketStatus.Open;

    public string MentorResponse{ get; set; } = string.Empty;
}
