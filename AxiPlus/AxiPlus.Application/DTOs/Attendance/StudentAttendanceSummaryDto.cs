namespace AxiPlus.Application.DTOs.Attendance;

public class StudentAttendanceSummaryDto
{
    public int TotalClasses{ get; set; }

    public int PresentCount{ get; set; }

    public int AbsentCount{ get; set; }

    public int LateCount{ get; set; }

    public decimal AttendancePercentage{ get; set; }

    public List<AttendanceRecordDto> RecentClasses{ get; set; } = new();
}
