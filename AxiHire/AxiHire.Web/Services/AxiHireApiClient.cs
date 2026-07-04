using System.Net.Http.Json;
using AxiHire.Application.DTOs;

namespace AxiHire.Web.Services;

public sealed class AxiHireApiClient
{
    private readonly HttpClient _httpClient;

    public AxiHireApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CandidateSummaryDto>> GetCandidatesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CandidateSummaryDto>>(
                "api/candidates") ?? new List<CandidateSummaryDto>();
        }
        catch (HttpRequestException)
        {
            return new List<CandidateSummaryDto>();
        }
        catch (TaskCanceledException)
        {
            return new List<CandidateSummaryDto>();
        }
    }
}
