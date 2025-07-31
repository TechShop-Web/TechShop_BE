using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("admin/gateway")]
    public class GatewayController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GatewayController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public GatewayController(
            IConfiguration configuration,
            ILogger<GatewayController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public IActionResult Welcome()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var ocelotConfig = environment == "Docker" ? "ocelot.Docker.json" : "ocelot.json";

            return Ok(new
            {
                service = "TechShop API Gateway",
                version = "2.0.0",
                powered_by = "Ocelot",
                environment = environment,
                configuration = ocelotConfig,
                timestamp = DateTime.UtcNow,
                endpoints = new
                {
                    swagger = "/swagger",
                    health = "/health",
                    admin = new
                    {
                        welcome = "/admin/gateway",
                        status = "/admin/gateway/status",
                        routes = "/admin/gateway/routes",
                        config = "/admin/gateway/config",
                        health = "/admin/gateway/health"
                    }
                },
                gateway_routes = new
                {
                    user_service = "/gateway/user/*",
                    product_service = "/gateway/product/*",
                    category_service = "/gateway/category/*",
                    product_variant_service = "/gateway/productvariant/*",
                    cart_service = "/gateway/carts/*",
                    order_service = "/gateway/orders/*",
                    payment_service = "/gateway/payments/*"
                }
            });
        }


        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                var ocelotConfig = environment == "Docker" ? "ocelot.Docker.json" : "ocelot.json";

                return Ok(new
                {
                    service = "TechShop API Gateway",
                    status = "Running",
                    environment = environment,
                    ocelot_configuration = ocelotConfig,
                    version = "2.0.0",
                    uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\.hh\:mm\:ss"),
                    timestamp = DateTime.UtcNow,
                    runtime_info = new
                    {
                        dotnet_version = Environment.Version.ToString(),
                        os_version = Environment.OSVersion.ToString(),
                        machine_name = Environment.MachineName,
                        processor_count = Environment.ProcessorCount,
                        working_set = GC.GetTotalMemory(false) / 1024 / 1024 + " MB"
                    },
                    features = new[]
                    {
                        "Ocelot API Gateway",
                        "JWT Authentication",
                        "Rate Limiting",
                        "Load Balancing",
                        "Health Checks",
                        "Request/Response Logging",
                        "CORS Support",
                        "Swagger Integration",
                        "Circuit Breaker"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gateway status");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }


        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                var routes = _configuration.GetSection("Routes").Get<List<object>>();
                var globalConfig = _configuration.GetSection("GlobalConfiguration").Get<object>();

                return Ok(new
                {
                    gateway_info = new
                    {
                        environment = environment,
                        base_url = environment == "Docker" ? "http://apigateway:8080" : "https://localhost:7000",
                        total_routes = routes?.Count ?? 0,
                        configuration_file = environment == "Docker" ? "ocelot.Docker.json" : "ocelot.json"
                    },
                    route_mapping = new
                    {
                        user_service = new
                        {
                            upstream = "/gateway/user/{everything}",
                            downstream = environment == "Docker" ? "http://userservice:8080/api/User/{everything}" : "https://localhost:7259/api/User/{everything}",
                            methods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH" },
                            authentication_required = true,
                            examples = new[]
                            {
                                "POST /gateway/user/login",
                                "POST /gateway/user/register-user",
                                "GET /gateway/user/list",
                                "GET /gateway/user/detail/123"
                            }
                        },
                        product_service = new
                        {
                            upstream = "/gateway/product/{everything}",
                            downstream = environment == "Docker" ? "http://productservice:8080/api/Product/{everything}" : "https://localhost:7167/api/Product/{everything}",
                            methods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH" },
                            authentication_required = false,
                            examples = new[]
                            {
                                "GET /gateway/product/list",
                                "GET /gateway/product/detail/456",
                                "POST /gateway/product/create"
                            }
                        },
                        category_service = new
                        {
                            upstream = "/gateway/category/{everything}",
                            downstream = environment == "Docker" ? "http://productservice:8080/api/Category/{everything}" : "https://localhost:7167/api/Category/{everything}",
                            methods = new[] { "GET", "POST", "DELETE" },
                            authentication_required = false,
                            examples = new[]
                            {
                                "GET /gateway/category/list",
                                "GET /gateway/category/detail/789",
                                "POST /gateway/category/create"
                            }
                        },
                        cart_service = new
                        {
                            upstream = "/gateway/carts/{everything}",
                            downstream = environment == "Docker" ? "http://cartservice:8080/api/Carts/{everything}" : "https://localhost:7075/api/Carts/{everything}",
                            methods = new[] { "GET", "POST", "DELETE" },
                            authentication_required = true,
                            examples = new[]
                            {
                                "GET /gateway/carts",
                                "POST /gateway/carts",
                                "DELETE /gateway/carts/123"
                            }
                        },
                        order_service = new
                        {
                            upstream = "/gateway/orders/{everything}",
                            downstream = environment == "Docker" ? "http://orderservice:8080/api/orders/{everything}" : "https://localhost:7267/api/orders/{everything}",
                            methods = new[] { "GET", "POST", "PATCH" },
                            authentication_required = true,
                            examples = new[]
                            {
                                "POST /gateway/orders",
                                "GET /gateway/orders/456",
                                "PATCH /gateway/orders/456/cancel"
                            }
                        },
                        payment_service = new
                        {
                            upstream = "/gateway/payments/{everything}",
                            downstream = environment == "Docker" ? "http://paymentservice:8080/api/payments/{everything}" : "https://localhost:7262/api/payments/{everything}",
                            methods = new[] { "GET", "POST" },
                            authentication_required = false,
                            examples = new[]
                            {
                                "POST /gateway/payments/create-vnpay",
                                "GET /gateway/payments/vnpay-return"
                            }
                        }
                    },
                    ocelot_routes = routes,
                    global_configuration = globalConfig
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting routes configuration");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }


        [HttpGet("config")]
        public IActionResult GetConfiguration()
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                var ocelotConfig = environment == "Docker" ? "ocelot.Docker.json" : "ocelot.json";

                return Ok(new
                {
                    gateway_configuration = new
                    {
                        environment = environment,
                        ocelot_config_file = ocelotConfig,
                        jwt_configuration = new
                        {
                            issuer = _configuration["Jwt:Issuer"],
                            audience = _configuration["Jwt:Audience"],
                            key_configured = !string.IsNullOrEmpty(_configuration["Jwt:Key"])
                        },
                        cors_policy = "AllowAllOrigins",
                        features_enabled = new
                        {
                            rate_limiting = true,
                            circuit_breaker = true,
                            load_balancing = true,
                            authentication = true,
                            health_checks = true,
                            request_logging = true
                        }
                    },
                    ocelot_global_config = _configuration.GetSection("GlobalConfiguration").Get<object>(),
                    routes_count = _configuration.GetSection("Routes").GetChildren().Count(),
                    swagger_endpoints = _configuration.GetSection("SwaggerEndPoints").Get<object>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gateway configuration");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }


        [HttpGet("health")]
        public async Task<IActionResult> CheckDownstreamHealth()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var services = new Dictionary<string, object>();

            var serviceEndpoints = environment == "Docker"
                ? new Dictionary<string, string>
                {
                    { "UserService", "http://userservice:8080" },
                    { "ProductService", "http://productservice:8080" },
                    { "CartService", "http://cartservice:8080" },
                    { "OrderService", "http://orderservice:8080" },
                    { "PaymentService", "http://paymentservice:8080" }
                }
                : new Dictionary<string, string>
                {
                    { "UserService", "https://localhost:7259" },
                    { "ProductService", "https://localhost:7167" },
                    { "CartService", "https://localhost:7075" },
                    { "OrderService", "https://localhost:7267" },
                    { "PaymentService", "https://localhost:7262" }
                };

            var healthTasks = serviceEndpoints.Select(async service =>
            {
                try
                {
                    using var httpClient = _httpClientFactory.CreateClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(10);

                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var response = await httpClient.GetAsync($"{service.Value}/health");
                    stopwatch.Stop();

                    var content = response.IsSuccessStatusCode
                        ? await response.Content.ReadAsStringAsync()
                        : "Service unavailable";

                    return new KeyValuePair<string, object>(service.Key, new
                    {
                        status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                        status_code = (int)response.StatusCode,
                        response_time_ms = stopwatch.ElapsedMilliseconds,
                        endpoint = service.Value,
                        last_checked = DateTime.UtcNow,
                        response_preview = content.Length > 100 ? content.Substring(0, 100) + "..." : content
                    });
                }
                catch (Exception ex)
                {
                    return new KeyValuePair<string, object>(service.Key, new
                    {
                        status = "Unhealthy",
                        status_code = 0,
                        response_time_ms = 0,
                        endpoint = service.Value,
                        last_checked = DateTime.UtcNow,
                        error = ex.Message
                    });
                }
            });

            var results = await Task.WhenAll(healthTasks);
            foreach (var result in results)
            {
                services[result.Key] = result.Value;
            }

            var healthyCount = services.Values.Count(s => ((dynamic)s).status == "Healthy");
            var totalCount = services.Count;
            var healthPercentage = (double)healthyCount / totalCount * 100;

            var overallStatus = healthPercentage >= 80 ? "Healthy" :
                               healthPercentage >= 50 ? "Degraded" : "Unhealthy";

            return Ok(new
            {
                gateway = "TechShop API Gateway",
                overall_status = overallStatus,
                environment = environment,
                health_summary = new
                {
                    healthy_services = healthyCount,
                    total_services = totalCount,
                    health_percentage = Math.Round(healthPercentage, 1)
                },
                services = services,
                timestamp = DateTime.UtcNow,
                notes = new[]
                {
                    "Health checks are performed against /health endpoints of each service",
                    "Services should return 200 status code to be considered healthy",
                    "Response times are measured in milliseconds"
                }
            });
        }
    }
}