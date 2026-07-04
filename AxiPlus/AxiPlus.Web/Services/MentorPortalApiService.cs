using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AxiPlus.Web.Models.MentorPortal;

namespace AxiPlus.Web.Services;

public class MentorPortalApiService
{
    private readonly AuthorizedApiClient _apiClient;

    public MentorPortalApiService(AuthorizedApiClient apiClient)
   {
        _apiClient = apiClient;
    }

    public Task<MentorDashboardModel?> GetDashboardAsync()
   {
        return GetAsync<MentorDashboardModel>("api/mentor-portal/dashboard");
    }

    public Task<List<MentorBatchModel>> GetBatchesAsync()
   {
        return GetListAsync<MentorBatchModel>("api/mentor-portal/batches");
    }

    public Task<List<MentorStudentModel>> GetStudentsAsync()
   {
        return GetListAsync<MentorStudentModel>("api/mentor-portal/students");
    }

    public Task<List<MentorLessonOptionModel>> GetLessonsAsync()
   {
        return GetListAsync<MentorLessonOptionModel>("api/mentor-portal/lessons");
    }

    public Task<List<MentorLiveClassModel>> GetLiveClassesAsync()
   {
        return GetListAsync<MentorLiveClassModel>(
            "api/mentor-portal/live-classes");
    }

    public Task<MentorLiveClassModel?> CreateLiveClassAsync(
        CreateMentorLiveClassModel model)
   {
        return PostAsync<MentorLiveClassModel>(
            "api/mentor-portal/live-classes",
            model);
    }

    public Task<bool> DeleteLiveClassAsync(Guid liveClassId)
   {
        return DeleteAsync($"api/mentor-portal/live-classes/{liveClassId}");
    }

    public Task<List<MentorAssignmentModel>> GetAssignmentsAsync()
   {
        return GetListAsync<MentorAssignmentModel>(
            "api/mentor-portal/assignments");
    }

    public Task<MentorAssignmentModel?> CreateAssignmentAsync(
        CreateMentorAssignmentModel model)
   {
        return PostAsync<MentorAssignmentModel>(
            "api/mentor-portal/assignments",
            model);
    }

    public Task<bool> DeleteAssignmentAsync(Guid assignmentId)
   {
        return DeleteAsync($"api/mentor-portal/assignments/{assignmentId}");
    }

    public Task<List<MentorSubmissionModel>> GetSubmissionsAsync()
   {
        return GetListAsync<MentorSubmissionModel>(
            "api/mentor-portal/submissions");
    }

    public Task<MentorSubmissionModel?> ReviewSubmissionAsync(
        Guid submissionId,
        ReviewAssignmentSubmissionModel model)
   {
        return PostAsync<MentorSubmissionModel>(
            $"api/mentor-portal/submissions/{submissionId}/review",
            model);
    }

    public Task<List<MentorSessionModel>> GetSessionsAsync()
   {
        return GetListAsync<MentorSessionModel>("api/mentor-portal/sessions");
    }

    public Task<MentorSessionModel?> CreateSessionAsync(
        CreateMentorSessionModel model)
   {
        return PostAsync<MentorSessionModel>(
            "api/mentor-portal/sessions",
            model);
    }

    public Task<List<MentorSessionModel>> CreateWeeklySessionsAsync(
        CreateMentorWeeklySessionsModel model)
   {
        return PostListAsync<MentorSessionModel>(
            "api/mentor-portal/weekly-sessions",
            model);
    }

    public Task<bool> DeleteSessionAsync(Guid sessionId)
   {
        return DeleteAsync($"api/mentor-portal/sessions/{sessionId}");
    }

    public Task<MentorAttendanceRosterModel?> GetAttendanceRosterAsync(
        Guid sessionId)
   {
        return GetAsync<MentorAttendanceRosterModel>(
            $"api/mentor-portal/sessions/{sessionId}/attendance");
    }

    public Task<MentorAttendanceRosterModel?> MarkAttendanceAsync(
        Guid sessionId,
        MarkMentorAttendanceModel model)
   {
        return PostAsync<MentorAttendanceRosterModel>(
            $"api/mentor-portal/sessions/{sessionId}/attendance",
            model);
    }

    public Task<List<MentorStudentReportModel>> GetStudentReportsAsync()
   {
        return GetListAsync<MentorStudentReportModel>(
            "api/mentor-portal/student-reports");
    }

    public Task<List<MentorSupportTicketModel>> GetSupportTicketsAsync()
   {
        return GetListAsync<MentorSupportTicketModel>(
            "api/mentor-portal/support-tickets");
    }

    public Task<MentorSupportTicketModel?> RespondToSupportTicketAsync(
        Guid ticketId,
        RespondSupportTicketModel model)
   {
        return PostAsync<MentorSupportTicketModel>(
            $"api/mentor-portal/support-tickets/{ticketId}/respond",
            model);
    }

    private async Task<List<T>> GetListAsync<T>(string url)
   {
        return await _apiClient.GetListAsync<T>(url);
    }

    private Task<T?> GetAsync<T>(string url)
   {
        return _apiClient.GetAsync<T>(url);
    }

    private Task<T?> PostAsync<T>(string url, object body)
   {
        return _apiClient.PostAsync<T>(url, body);
    }

    private async Task<List<T>> PostListAsync<T>(string url, object body)
   {
        return await _apiClient.PostListAsync<T>(url, body);
    }

    private Task<bool> DeleteAsync(string url)
   {
        return _apiClient.SendAsync(HttpMethod.Delete, url);
    }
}
