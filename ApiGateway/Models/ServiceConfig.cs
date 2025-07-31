namespace ApiGateway.Models
{
    public class ServiceConfig
    {
        public string BaseUrl { get; }
        public string ApiPath { get; }

        public ServiceConfig(string baseUrl, string apiPath)
        {
            BaseUrl = baseUrl;
            ApiPath = apiPath;
        }
    }
}
