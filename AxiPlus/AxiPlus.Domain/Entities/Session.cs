using AxiPlus.Domain.Common;

namespace AxiPlus.Domain.Entities;

public class Session : BaseEntity
{
    public Guid BatchId{ get; set; }

    public string Title{ get; set; } = string.Empty;

    public string MeetLink{ get; set; } = string.Empty;

    public DateTime StartTime{ get; set; }

    public DateTime EndTime{ get; set; }
}