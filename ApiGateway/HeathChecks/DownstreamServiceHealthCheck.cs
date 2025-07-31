using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateway.HealthChecks
{
    public class DownstreamServicesHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DownstreamServicesHealthCheck> _logger;

        public DownstreamServicesHealthCheck(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<DownstreamServicesHealthCheck> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

                var services = environment == "Docker"
                    ? new[]
                    {
                        "http://userservice:8080/health",
                        "http://productservice:8080/health",
                        "http://cartservice:8080/health",
                        "http://orderservice:8080/health",
                        "http://paymentservice:8080/health"
                    }
                    : new[]
                    {
                        "https://localhost:7259/health",
                        "https://localhost:7167/health",
                        "https://localhost:7075/health",
                        "https://localhost:7267/health",
                        "https://localhost:7262/health"
                    };

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                var healthyServices = 0;
                var totalServices = services.Length;
                var serviceResults = new List<string>();

                foreach (var service in services)
                {
                    try
                    {
                        var response = await client.GetAsync(service, cancellationToken);
                        if (response.IsSuccessStatusCode)
                        {
                            healthyServices++;
                            serviceResults.Add($"✓ {GetServiceName(service)}");
                        }
                        else
                        {
                            serviceResults.Add($"✗ {GetServiceName(service)} ({response.StatusCode})");
                        }
                    }
                    catch (Exception ex)
                    {
                        serviceResults.Add($"✗ {GetServiceName(service)} ({ex.Message})");
                        _logger.LogWarning("Health check failed for {Service}: {Error}", service, ex.Message);
                    }
                }

                var healthPercentage = (double)healthyServices / totalServices * 100;
                var data = new Dictionary<string, object>
                {
                    ["HealthyServices"] = healthyServices,
                    ["TotalServices"] = totalServices,
                    ["HealthPercentage"] = healthPercentage,
                    ["ServiceDetails"] = serviceResults,
                    ["Environment"] = environment
                };

                if (healthPercentage >= 80)
                {
                    return HealthCheckResult.Healthy(
                        $"{healthyServices}/{totalServices} services healthy ({healthPercentage:F1}%)",
                        data);
                }
                else if (healthPercentage >= 50)
                {
                    return HealthCheckResult.Degraded(
                        $"{healthyServices}/{totalServices} services healthy ({healthPercentage:F1}%)",null,
                        data);
                }
                else
                {
                    return HealthCheckResult.Unhealthy(
                        $"Only {healthyServices}/{totalServices} services healthy ({healthPercentage:F1}%)",
                        null,
                        data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Downstream services health check failed");
                return HealthCheckResult.Unhealthy($"Health check failed: {ex.Message}", ex);
            }
        }

        private static string GetServiceName(string serviceUrl)
        {
            if (serviceUrl.Contains("userservice")) return "UserService";
            if (serviceUrl.Contains("productservice")) return "ProductService";
            if (serviceUrl.Contains("cartservice")) return "CartService";
            if (serviceUrl.Contains("orderservice")) return "OrderService";
            if (serviceUrl.Contains("paymentservice")) return "PaymentService";
            if (serviceUrl.Contains("7259")) return "UserService";
            if (serviceUrl.Contains("7167")) return "ProductService";
            if (serviceUrl.Contains("7075")) return "CartService";
            if (serviceUrl.Contains("7267")) return "OrderService";
            if (serviceUrl.Contains("7262")) return "PaymentService";
            return "Unknown Service";
        }
    }

    public class OcelotConfigurationHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OcelotConfigurationHealthCheck> _logger;

        public OcelotConfigurationHealthCheck(
            IConfiguration configuration,
            ILogger<OcelotConfigurationHealthCheck> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                var ocelotFileName = environment == "Docker" ? "ocelot.Docker.json" : "ocelot.json";

                var routes = _configuration.GetSection("Routes");
                var globalConfig = _configuration.GetSection("GlobalConfiguration");

                var routeCount = routes.GetChildren().Count();
                var hasGlobalConfig = globalConfig.Exists();

                var data = new Dictionary<string, object>
                {
                    ["ConfigurationFile"] = ocelotFileName,
                    ["Environment"] = environment,
                    ["RouteCount"] = routeCount,
                    ["HasGlobalConfiguration"] = hasGlobalConfig
                };

                if (routes.Exists() && hasGlobalConfig && routeCount > 0)
                {
                    return Task.FromResult(HealthCheckResult.Healthy(
                        $"Ocelot configuration loaded from {ocelotFileName} with {routeCount} routes",
                        data));
                }
                else
                {
                    var issues = new List<string>();
                    if (!routes.Exists()) issues.Add("Routes section missing");
                    if (!hasGlobalConfig) issues.Add("GlobalConfiguration section missing");
                    if (routeCount == 0) issues.Add("No routes configured");

                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        $"Ocelot configuration issues: {string.Join(", ", issues)}",
                        null,
                        data));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocelot configuration health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy($"Configuration error: {ex.Message}", ex));
            }
        }
    }
}