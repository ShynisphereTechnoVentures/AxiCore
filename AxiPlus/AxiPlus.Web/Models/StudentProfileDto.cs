namespace AxiPlus.Web.Models;

using AxiPlus.Domain.Enums;

public class StudentProfileDto
{
    public string FullName{get;set; }
        = string.Empty;

    public string Email{get;set; }
        = string.Empty;

    public string Batch{get;set; }
        = string.Empty;

    public string Track{get;set; }
        = string.Empty;

    public DateTime JoinedDate{get;set; }

    public string PhoneNumber{get;set; }
        = string.Empty;

    public StudentBillingModel? Billing{get;set; }
}

public class StudentBillingModel
{
    public string CurrentPlanCode{get;set; } = "foundation";

    public string CurrentPlanName{get;set; } = "Foundation";

    public decimal MonthlyFee{get;set; }

    public string Currency{get;set; } = "INR";

    public DateTime NextDueDate{get;set; }

    public DateTime GraceEndsAt{get;set; }

    public BillingStatus Status{get;set; }

    public bool AutoPayEnabled{get;set; }

    public string UpiId{get;set; } = string.Empty;

    public int DaysRemaining{get;set; }

    public bool IsLocked{get;set; }
}

public class UpdateStudentBillingModel
{
    public bool AutoPayEnabled{get;set; }

    public string UpiId{get;set; } = string.Empty;
}

public class StudentPaymentModel
{
    public Guid PaymentId{get;set; }

    public decimal Amount{get;set; }

    public string Currency{get;set; } = "INR";

    public PaymentStatus Status{get;set; }

    public string Method{get;set; } = "UPI";

    public string UpiId{get;set; } = string.Empty;

    public string ProviderReference{get;set; } = string.Empty;

    public DateTime DueDate{get;set; }

    public DateTime GraceEndsAt{get;set; }

    public DateTime CreatedAt{get;set; }

    public DateTime? PaidAt{get;set; }
}

public class StudentPlanModel
{
    public string Code{get;set; } = string.Empty;

    public string Name{get;set; } = string.Empty;

    public string Description{get;set; } = string.Empty;

    public decimal MonthlyFee{get;set; }

    public string Currency{get;set; } = "INR";

    public bool IsCurrent{get;set; }

    public List<string> Features{get;set; } = new();
}
