using AxiPlus.Domain.Enums;

namespace AxiPlus.Application.DTOs.Students;

public class StudentBillingDto
{
    public string CurrentPlanCode{ get; set; } = "foundation";

    public string CurrentPlanName{ get; set; } = "Foundation";

    public decimal MonthlyFee{ get; set; }

    public string Currency{ get; set; } = "INR";

    public DateTime NextDueDate{ get; set; }

    public DateTime GraceEndsAt{ get; set; }

    public BillingStatus Status{ get; set; }

    public bool AutoPayEnabled{ get; set; }

    public string UpiId{ get; set; } = string.Empty;

    public int DaysRemaining{ get; set; }

    public bool IsLocked{ get; set; }
}

public class UpdateStudentBillingDto
{
    public bool AutoPayEnabled{ get; set; }

    public string UpiId{ get; set; } = string.Empty;
}

public class StudentPaymentDto
{
    public Guid PaymentId{ get; set; }

    public decimal Amount{ get; set; }

    public string Currency{ get; set; } = "INR";

    public PaymentStatus Status{ get; set; }

    public string Method{ get; set; } = "UPI";

    public string UpiId{ get; set; } = string.Empty;

    public string ProviderReference{ get; set; } = string.Empty;

    public DateTime DueDate{ get; set; }

    public DateTime GraceEndsAt{ get; set; }

    public DateTime CreatedAt{ get; set; }

    public DateTime? PaidAt{ get; set; }
}

public class StudentPlanDto
{
    public string Code{ get; set; } = string.Empty;

    public string Name{ get; set; } = string.Empty;

    public string Description{ get; set; } = string.Empty;

    public decimal MonthlyFee{ get; set; }

    public string Currency{ get; set; } = "INR";

    public bool IsCurrent{ get; set; }

    public List<string> Features{ get; set; } = new();
}
