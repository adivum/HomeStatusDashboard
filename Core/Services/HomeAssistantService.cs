using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Constants;
using Core.Interfaces;
using Core.Models;
using Core.Models.EntityViews;
using Core.Options;
using Core.Utilities;
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

        public async Task TurnOnAsync(string entityId, CancellationToken ct = default)
        {
            await CallServiceAsync(ServiceDomain.Switch, ServiceAction.TurnOn, entityId, ct);
        }

        public async Task TurnOffAsync(string entityId, CancellationToken ct = default)
        {
            await CallServiceAsync(ServiceDomain.Switch, ServiceAction.TurnOff, entityId, ct);
        }

        private async Task CallServiceAsync(ServiceDomain domain, ServiceAction action, string entityId, CancellationToken ct = default)
        {
            var request = new ServiceRequest { EntityId = entityId };
            var endpoint = $"api/services/{domain.ToServicePath(action)}";
            
            using var response = await _http.PostAsJsonAsync(endpoint, request, JsonOptions, ct);
            response.EnsureSuccessStatusCode();
        }
    }
}