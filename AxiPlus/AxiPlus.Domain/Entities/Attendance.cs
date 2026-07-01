using AxiPlus.Domain.Common;
using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class Attendance : BaseEntity
{
    public Guid SessionId{ get; set; }

    public Guid StudentId{ get; set; }

    public AttendanceStatus Status{ get; set; }
}