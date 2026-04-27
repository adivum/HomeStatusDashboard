using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Core.Interfaces;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        // Core handles its own configuration loading (appsettings.json + user-secrets)
        services.AddHomeAssistantCore();
    })
    .Build();

var service = host.Services.GetRequiredService<IHomeAssistantService>();

try
{
    Console.WriteLine("Fetching all Home Assistant states...\n");
    var states = await service.GetAllStatesAsync();
    
    var statesList = states.OrderByDescending(e => e.LastChanged).ToList();
    Console.WriteLine($"Total entities: {statesList.Count}\n");
    Console.WriteLine("First 10 entities:");
    
    // Calculate dynamic column widths
    var entityIdWidth = Math.Max("Entity ID".Length, statesList.Max(s => s.EntityId.Length));
    var stateWidth = Math.Max("State".Length, statesList.Max(s => s.State.Length));
    var timestampWidth = Math.Max("Last Changed".Length, "2026-04-26 12:34:56 PM".Length);
    
    // Print header
    Console.WriteLine(new string('-', entityIdWidth + stateWidth + timestampWidth + 8));
    Console.WriteLine(string.Format("{0,-" + entityIdWidth + "}  {1,-" + stateWidth + "}  {2}", 
        "Entity ID", "State", "Last Changed"));
    Console.WriteLine(new string('-', entityIdWidth + stateWidth + timestampWidth + 8));
    
    // Print data
    foreach (var state in statesList.Take(10))
    {
        Console.WriteLine(string.Format("{0,-" + entityIdWidth + "}  {1,-" + stateWidth + "}  {2:g}", 
            state.EntityId, state.State, state.LastChanged));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"\nMake sure:");
    Console.WriteLine("1. Home Assistant is running at the configured URL");
    Console.WriteLine("2. Set secrets in Core project:");
    Console.WriteLine("   dotnet user-secrets set \"HomeAssistant:Token\" \"<your-token>\" --project Core");
    Console.WriteLine("   dotnet user-secrets set \"HomeAssistant:BaseUrl\" \"<your-url>\" --project Core");
    Console.WriteLine("3. Or configure in Core/appsettings.json");
}
