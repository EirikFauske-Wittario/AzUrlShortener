using System.Threading.Tasks;
using Cloud5mins.ShortenerTools.Core.Domain;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Cloud5mins.ShortenerTools.Functions.Functions;

public class UrlHealthCheck(ShortenerSettings settings, ILogger<UrlHealthCheck> logger)
{
    [Function("HealthCheck")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/HealthCheck")] HttpRequestData req,
        ExecutionContext context)
    {
        var response = req.CreateResponse();

        var stgHelper = new StorageTableHelper(settings.DataStorage);
        var healthCheck = stgHelper.IsHealthy();
        if (!healthCheck.IsHealthy)
        {
            logger.LogError(healthCheck.Exception, "Health check failed.");

            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Not healthy: "+healthCheck.Exception?.Message);
            return response;
        }
        else
        {
            response.StatusCode = System.Net.HttpStatusCode.OK;
            await response.WriteStringAsync("OK");
        }

        return response;
    }
}
