using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class StudentModule
{
    public Guid Id{ get; set; }

    public Guid StudentId{ get; set; }

    public Student Student{ get; set; }
        = null!;

    public int ModuleId{ get; set; }

    public Module Module{ get; set; }
        = null!;

    public bool IsUnlocked{ get; set; }

    public bool IsCompleted{ get; set; }

    public bool ExamPassed{ get; set; }

    public int ExamAttempts{ get; set; }

    public decimal ProgressPercentage{ get; set; }

    public ModuleStatus Status{ get; set; }

    public DateTime AssignedAt{ get; set; }

    public DateTime? CompletedAt{ get; set; }
}