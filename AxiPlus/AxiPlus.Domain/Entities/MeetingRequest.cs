using AxiPlus.Domain.Common;
using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class MeetingRequest : BaseEntity
{
    public Guid MentorUserId{ get; set; }

    public User MentorUser{ get; set; } = null!;

    public Guid StudentId{ get; set; }

    public Student Student{ get; set; } = null!;

    public Guid BatchId{ get; set; }

    public Batch Batch{ get; set; } = null!;

    public DateTime ScheduledAt{ get; set; }

    public string MeetingLink{ get; set; } = string.Empty;

    public string Reason{ get; set; } = string.Empty;

    public MeetingRequestStatus Status{ get; set; } = MeetingRequestStatus.Pending;

    public string StudentResponseNote{ get; set; } = string.Empty;

    public string MentorFollowUpNote{ get; set; } = string.Empty;
}
