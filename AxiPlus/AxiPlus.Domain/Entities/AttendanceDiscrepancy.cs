using AxiPlus.Domain.Common;
using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class AttendanceDiscrepancy : BaseEntity
{
    public Guid StudentId{ get; set; }

    public Student Student{ get; set; } = null!;

    public Guid SessionId{ get; set; }

    public Session Session{ get; set; } = null!;

    public AttendanceStatus? CurrentStatus{ get; set; }

    public AttendanceStatus RequestedStatus{ get; set; }

    public string Reason{ get; set; } = string.Empty;

    public AttendanceDiscrepancyStatus Status{ get; set; } =
        AttendanceDiscrepancyStatus.Open;

    public Guid? ReviewedByUserId{ get; set; }

    public User? ReviewedByUser{ get; set; }

    public string ResolutionNote{ get; set; } = string.Empty;
}
