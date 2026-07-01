namespace AxiPlus.Application.DTOs.Students;

public class StudentProfileDto
{
    public string FullName{ get; set; }
        = string.Empty;

    public string Email{ get; set; }
        = string.Empty;

    public string Batch{ get; set; }
        = string.Empty;

    public string Track{ get; set; }
        = string.Empty;

    public DateTime JoinedDate{ get; set; }

    public string PhoneNumber{ get; set; }
        = string.Empty;

    public StudentBillingDto? Billing{ get; set; }
}
