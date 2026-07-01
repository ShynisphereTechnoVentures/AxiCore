namespace AxiPlus.Web.Models;

public class StudentDashboardModel
{
    public string StudentName{get;set; } = string.Empty;

    public string TrackName{get;set; } = string.Empty;

    public string BatchName{get;set; } = string.Empty;

    public string CurrentModule{get;set; } = string.Empty;

    public string CurrentLesson{get;set; } = string.Empty;

    public decimal AttendancePercentage{get;set; }

    public decimal ProgressPercentage{get;set; }

    public int PendingAssignments{get;set; }

    public string MainMentor{get;set; } = string.Empty;

    public string AssistantMentor{get;set; } = string.Empty;
}