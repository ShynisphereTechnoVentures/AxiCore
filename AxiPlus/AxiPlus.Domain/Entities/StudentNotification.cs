using AxiPlus.Domain.Common;

namespace AxiPlus.Domain.Entities;

public class StudentNotification : BaseEntity
{
    public Guid StudentId{ get; set; }

    public Student Student{ get; set; } = null!;

    public string Title{ get; set; } = string.Empty;

    public string Message{ get; set; } = string.Empty;

    public string Type{ get; set; } = "Info";

    public bool IsRead{ get; set; }
}
