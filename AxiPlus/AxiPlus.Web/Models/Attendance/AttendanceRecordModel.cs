namespace AxiPlus.Web.Models.Attendance;

public class AttendanceRecordModel
{
    public Guid SessionId{get;set; }

    public string Title{get;set; } = string.Empty;

    public DateTime StartTime{get;set; }

    public DateTime EndTime{get;set; }

    public string Status{get;set; } = string.Empty;
}
