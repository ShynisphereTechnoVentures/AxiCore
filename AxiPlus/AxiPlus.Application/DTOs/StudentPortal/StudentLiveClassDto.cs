namespace AxiPlus.Application.DTOs.StudentPortal;

public class StudentLiveClassDto
{
    public Guid SessionId{ get; set; }

    public string Title{ get; set; } = string.Empty;

    public string MeetingLink{ get; set; } = string.Empty;

    public DateTime StartTime{ get; set; }

    public DateTime EndTime{ get; set; }

    public string Status{ get; set; } = string.Empty;

    public string AttendanceStatus{ get; set; } = string.Empty;
}
