using AxiPlus.Domain.Common;

namespace AxiPlus.Domain.Entities;

public class MentorProfile : BaseEntity
{
    public Guid UserId{ get; set; }

    public User User{ get; set; } = null!;

    public string PhoneNumber{ get; set; } = string.Empty;

    public string Address{ get; set; } = string.Empty;

    public string EmergencyContact{ get; set; } = string.Empty;

    public string Designation{ get; set; } = string.Empty;

    public DateTime JoinedDate{ get; set; } = DateTime.UtcNow;
}
