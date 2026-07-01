namespace AxiPlus.Web.Models.Attendance;

public class StudentAttendanceSummaryModel
{ 
    public int TotalClasses{get;set; }

    public int PresentCount{get;set; }

    public int AbsentCount{get;set; }

    public int LateCount{get;set; }

    public decimal AttendancePercentage{get;set; }

    public List<AttendanceRecordModel> RecentClasses{get;set; } = new();
}
