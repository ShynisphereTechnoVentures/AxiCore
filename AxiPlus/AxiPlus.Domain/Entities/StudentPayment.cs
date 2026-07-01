using AxiPlus.Domain.Common;
using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class StudentPayment : BaseEntity
{
    public Guid StudentId{ get; set; }

    public Student Student{ get; set; } = null!;

    public decimal Amount{ get; set; }

    public string Currency{ get; set; } = "INR";

    public PaymentStatus Status{ get; set; } = PaymentStatus.Initiated;

    public string Method{ get; set; } = "UPI";

    public string UpiId{ get; set; } = string.Empty;

    public string ProviderReference{ get; set; } = string.Empty;

    public DateTime DueDate{ get; set; }

    public DateTime GraceEndsAt{ get; set; }

    public DateTime? PaidAt{ get; set; }
}
