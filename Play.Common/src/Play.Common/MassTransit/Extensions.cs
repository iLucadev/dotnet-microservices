using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit;
public static class Extensions
{
  public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
  {
    services.AddMassTransit(configure =>
    {
      configure.AddConsumers(Assembly.GetEntryAssembly());
      configure.UsingRabbitMq((context, configurator) =>
      {
        
        var configuration = context.GetRequiredService<IConfiguration>();
        var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>() ?? new ServiceSettings();

        var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
        configurator.Host(rabbitMQSettings.Host);
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
        configurator.UseMessageRetry(retryConfiguration =>
        {
          retryConfiguration.Interval(3, TimeSpan.FromSeconds(5));
        });
      });
    });

    return services;
  }
}
