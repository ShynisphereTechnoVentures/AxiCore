using AxiPlus.Domain.Common;
using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class StudentBillingAccount : BaseEntity
{
    public Guid StudentId{ get; set; }

    public Student Student{ get; set; } = null!;

    public decimal MonthlyFee{ get; set; }

    public string Currency{ get; set; } = "INR";

    public DateTime NextDueDate{ get; set; }

    public DateTime GraceEndsAt{ get; set; }

    public BillingStatus Status{ get; set; } = BillingStatus.Trial;

    public bool AutoPayEnabled{ get; set; }

    public string UpiId{ get; set; } = string.Empty;

    public DateTime? LastReminderAt{ get; set; }
}
