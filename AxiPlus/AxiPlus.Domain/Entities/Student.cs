using AxiPlus.Domain.Common;

namespace AxiPlus.Domain.Entities;

public class Student : BaseEntity
{
    public Guid UserId{ get; set; }

    public Guid BatchId{ get; set; }

    public string CollegeName{ get; set; } = string.Empty;
    public int TrackId{ get; set; }

    public Track Track{ get; set; } = null!;

    public DateTime JoinedDate{ get; set; }

    public Batch Batch{ get; set; } = null!;

    public StudentBillingAccount? BillingAccount{ get; set; }


    public string FullName{ get; set; }
    = string.Empty;

public string Email{ get; set; }
    = string.Empty;

public string PhoneNumber{ get; set; }
    = string.Empty;

public string Gender{ get; set; }
    = string.Empty;

public string Address{ get; set; }
    = string.Empty;

    public ICollection<StudentLessonProgress>
    LessonProgresses
   { get; set; }
    = new List<StudentLessonProgress>();
}
