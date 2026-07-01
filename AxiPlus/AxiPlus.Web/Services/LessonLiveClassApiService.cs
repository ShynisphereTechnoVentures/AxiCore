using AxiPlus.Web.Models;

namespace AxiPlus.Web.Services;

public class LessonLiveClassApiService
{
    private readonly HttpClient _httpClient;

    public LessonLiveClassApiService(HttpClient httpClient)
   {
        _httpClient = httpClient;
    }

    public async Task<List<LessonLiveClassModel>> GetByLessonAsync(
        Guid lessonId)
   {
        try
       {
            var response = await _httpClient.GetAsync(
                $"api/lessonliveclass/lesson/{lessonId}");

            if (!response.IsSuccessStatusCode)
           {
                return new List<LessonLiveClassModel>();
            }

            return await response.Content
                .ReadFromJsonAsync<List<LessonLiveClassModel>>()
                ?? new List<LessonLiveClassModel>();
        }
        catch (HttpRequestException)
       {
            return new List<LessonLiveClassModel>();
        }
    }
}
