namespace AxiPlus.Application.DTOs.Attendance;

public class AttendanceRecordDto
{
    public Guid SessionId{ get; set; }

    public string Title{ get; set; } = string.Empty;

    public DateTime StartTime{ get; set; }

    public DateTime EndTime{ get; set; }

    public string Status{ get; set; } = string.Empty;
}
