using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AxiPlus.Web.Models.Operations;

namespace AxiPlus.Web.Services;

public class OperationsApiService
{
    private readonly AuthorizedApiClient _apiClient;

    public OperationsApiService(AuthorizedApiClient apiClient)
   {
        _apiClient = apiClient;
    }

    public Task<MentorProfileModel?> GetMentorProfileAsync()
   {
        return GetAsync<MentorProfileModel>("api/operations/mentor/profile");
    }

    public Task<SalarySlipModel?> CreateSalarySlipAsync(
        CreateSalarySlipModel model)
   {
        return PostAsync<SalarySlipModel>(
            "api/operations/admin/salary-slips",
            model);
    }

    public Task<List<MeetingRequestModel>> GetMentorMeetingRequestsAsync()
   {
        return GetListAsync<MeetingRequestModel>(
            "api/operations/mentor/meeting-requests");
    }

    public Task<MeetingRequestModel?> CreateMeetingRequestAsync(
        CreateMeetingRequestModel model)
   {
        return PostAsync<MeetingRequestModel>(
            "api/operations/mentor/meeting-requests",
            model);
    }

    public Task<MeetingRequestModel?> UpdateMeetingFollowUpAsync(
        Guid id,
        UpdateMeetingFollowUpModel model)
   {
        return PostAsync<MeetingRequestModel>(
            $"api/operations/mentor/meeting-requests/{id}/follow-up",
            model);
    }

    public Task<List<AttendanceDiscrepancyModel>>
        GetMentorAttendanceDiscrepanciesAsync()
   {
        return GetListAsync<AttendanceDiscrepancyModel>(
            "api/operations/mentor/attendance-discrepancies");
    }

    public Task<AttendanceDiscrepancyModel?> ReviewAttendanceDiscrepancyAsync(
        Guid id,
        ReviewAttendanceDiscrepancyModel model)
   {
        return PostAsync<AttendanceDiscrepancyModel>(
            $"api/operations/mentor/attendance-discrepancies/{id}/review",
            model);
    }

    public Task<List<MeetingRequestModel>> GetStudentMeetingRequestsAsync()
   {
        return GetListAsync<MeetingRequestModel>(
            "api/operations/student/meeting-requests");
    }

    public Task<MeetingRequestModel?> RespondToMeetingRequestAsync(
        Guid id,
        RespondMeetingRequestModel model)
   {
        return PostAsync<MeetingRequestModel>(
            $"api/operations/student/meeting-requests/{id}/respond",
            model);
    }

    public Task<List<AttendanceDiscrepancyModel>>
        GetStudentAttendanceDiscrepanciesAsync()
   {
        return GetListAsync<AttendanceDiscrepancyModel>(
            "api/operations/student/attendance-discrepancies");
    }

    public Task<AttendanceDiscrepancyModel?> CreateAttendanceDiscrepancyAsync(
        CreateAttendanceDiscrepancyModel model)
   {
        return PostAsync<AttendanceDiscrepancyModel>(
            "api/operations/student/attendance-discrepancies",
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
}
