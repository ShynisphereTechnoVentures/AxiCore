namespace AxiPlus.Domain.Entities;

public class Batch
{
    public Guid Id{ get; set; }

    public string Name{ get; set; }
        = string.Empty;

    public int BatchNumber{ get; set; }

    public int TrackId{ get; set; }

    public Track Track{ get; set; }
        = null!;

    public string Level{ get; set; }
        = string.Empty;

    public int Capacity{ get; set; } = 15;

    public int CurrentStrength{ get; set; }

    public bool IsActive{ get; set; } = true;

    public DateTime CreatedAt{ get; set; }

    public Guid? MentorId{ get; set; }

    public User? Mentor{ get; set; }

    public Guid? AssistantMentorId{ get; set; }

    public User? AssistantMentor{ get; set; }

    public ICollection<Student> Students
        = new List<Student>();
}