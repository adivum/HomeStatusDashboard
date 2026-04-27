using System.Net.Http.Json;
using Core.Interfaces;
using Core.Models;
using Core.Options;
using Microsoft.Extensions.Options;

namespace Core.Services
{
    public class HomeAssistantService : IHomeAssistantService
    {
        private readonly HttpClient _http;

        public HomeAssistantService(HttpClient http, IOptions<HomeAssistantOptions> options)
        {
            http.BaseAddress = new Uri(options.Value.BaseUrl);
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.Value.Token);
            _http = http;
        }

        public async Task<IEnumerable<HaState>> GetAllStatesAsync(CancellationToken ct = default)
        {
            var states = await _http.GetFromJsonAsync<List<HaState>>("api/states", ct);
            return states ?? [];
        }

        public async Task<HaState?> GetEntityAsync(string entityId, CancellationToken ct = default)
        {
            return await _http.GetFromJsonAsync<HaState>($"api/states/{entityId}", ct);
        }
    }
}