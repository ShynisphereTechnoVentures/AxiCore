namespace AxiForge.Application.DTOs.Dashboard;

public sealed class StudentDashboardDto
{
    public string StudentName { get; set; } = string.Empty;

    public int SolvedProblems { get; set; }

    public int ActiveRoadmaps { get; set; }

    public int PendingAssessments { get; set; }

    public string CurrentFocus { get; set; } = "Coding practice foundation";
}
