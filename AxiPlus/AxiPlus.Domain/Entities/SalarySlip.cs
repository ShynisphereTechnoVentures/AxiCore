using AxiPlus.Domain.Common;

namespace AxiPlus.Domain.Entities;

public class SalarySlip : BaseEntity
{
    public Guid UserId{ get; set; }

    public User User{ get; set; } = null!;

    public int Month{ get; set; }

    public int Year{ get; set; }

    public decimal GrossAmount{ get; set; }

    public decimal NetAmount{ get; set; }

    public string FileUrl{ get; set; } = string.Empty;

    public Guid UploadedByUserId{ get; set; }

    public User UploadedByUser{ get; set; } = null!;
}
