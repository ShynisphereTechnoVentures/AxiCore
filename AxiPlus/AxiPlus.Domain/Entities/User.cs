using AxiPlus.Domain.Common;
using AxiPlus.Domain.Enums;

namespace AxiPlus.Domain.Entities;

public class User : BaseEntity
{
    public string FullName{ get; set; } = string.Empty;

    public string Email{ get; set; } = string.Empty;

    public string PasswordHash{ get; set; } = string.Empty;

    public int RoleId{ get; set; }

    public Role Role{ get; set; } = null!;

    public bool IsActive{ get; set; } = true;

    public ICollection<StudentLessonProgress>
    LessonProgresses
   { get; set; }
    = new List<StudentLessonProgress>();

}
