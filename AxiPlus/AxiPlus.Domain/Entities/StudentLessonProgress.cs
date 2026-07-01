using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class StudentLessonProgress
{
    public Guid Id{ get; set; }

    public Guid StudentId{ get; set; }

    public Student Student{ get; set; } = null!;

    public Guid LessonId{ get; set; }

    public Lesson Lesson{ get; set; }
        = null!;

    public DateTime CreatedAt{ get; set; }
        = DateTime.UtcNow;

    public LessonStatus Status{ get; set; }
= LessonStatus.Pending;

    public DateTime? ReviewedAt{ get; set; }

    public Guid? ReviewedById{ get; set; }
    public User? ReviewedBy{ get; set; }

    public string MentorRemarks{ get; set; }
        = string.Empty;

    public bool IsCompleted{ get; set; }
}