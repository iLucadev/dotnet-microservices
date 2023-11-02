using Polly;
using Polly.Timeout;

namespace Play.Inventory.Service.Clients.Config;
public static class CatalogClientConfig
{
  public static void AddCatalogClient(IServiceCollection services)
  {
    Random jitterer = new Random();
    var serviceProvider = services.BuildServiceProvider();

    services.AddHttpClient<CatalogClient>(client =>
    {
      client.BaseAddress = new Uri("https://localhost:7108");
    }
    )
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)), onRetry: (outcome, timespan, retryAttempt) =>
    {

      serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
    }))
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
        3,
        TimeSpan.FromSeconds(15),
        onBreak: (outcome, timespan) =>
        {
          serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
        },
        onReset: () =>
        {
          serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Closing the circuit...");
        }
    ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
  }
}

