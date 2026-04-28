using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Interfaces;
using Core.Models;
using Core.Models.EntityViews;
using Core.Options;
using Microsoft.Extensions.Options;

namespace Core.Services
{
    public class HomeAssistantService : IHomeAssistantService
    {
        private readonly HttpClient _http;
        private readonly IEntityViewFactory _factory;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public HomeAssistantService(HttpClient http, IOptions<HomeAssistantOptions> options, IEntityViewFactory factory)
        {
            http.BaseAddress = new Uri(options.Value.BaseUrl);
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.Value.Token);
            _http = http;
            _factory = factory;
        }

        private async Task<IEnumerable<HaState>> GetAllStatesAsync(CancellationToken ct = default)
        {
            var states = await _http.GetFromJsonAsync<List<HaState>>("api/states", JsonOptions, ct);
            return states ?? [];
        }

        private async Task<HaState?> GetEntityAsync(string entityId, CancellationToken ct = default)
        {
            return await _http.GetFromJsonAsync<HaState>($"api/states/{entityId}", JsonOptions, ct);
        }

        public async Task<IEnumerable<IEntityView>> GetDashboardViewsAsync(CancellationToken ct = default)
        {
            var states = await GetAllStatesAsync(ct);
            return states.Select(_factory.CreateView);
        }

        public async Task<IEnumerable<IEntityView>> GetEntitiesListViewsAsync(CancellationToken ct = default)
        {
            var states = await GetAllStatesAsync(ct);
            return states.Select(_factory.CreateView);
        }

        public async Task<IEntityView?> GetEntityViewAsync(string entityId, CancellationToken ct = default)
        {
            var state = await GetEntityAsync(entityId, ct);
            return state != null ? _factory.CreateView(state) : null;
        }
    }
}