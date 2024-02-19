using Alyas.OrderCloud.EnvironmentManagementApi.Extensions;
using Alyas.OrderCloud.EnvironmentManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;

namespace Alyas.OrderCloud.EnvironmentManagementApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class EnvironmentManagementController : ControllerBase
    {
        [HttpPost("CloneEnvironment")]
        public async Task<IActionResult> CloneEnvironment([FromBody]CloneModel model)
        {
            var sourceClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = model.SourceApiUrl,
                AuthUrl = model.SourceApiUrl,
                ClientId = model.SourceClientId,
                ClientSecret = model.SourceSecret,
                Roles = new[] { ApiRole.FullAccess }
            });
            var destinationClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = model.DestinationApiUrl,
                AuthUrl = model.DestinationApiUrl,
                ClientId = model.DestinationClientId,
                ClientSecret = model.DestinationSecret,
                Roles = new[] { ApiRole.FullAccess }
            });

            await sourceClient.CloneEnvironment(destinationClient);
            return new ObjectResult(new {Success = true});
        }

        [HttpPost("CleanupEnvironment")]
        public async Task<IActionResult> CleanupEnvironment([FromBody] EnvCleanupModel model)
        {
            var sourceClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = model.ApiUrl,
                AuthUrl = model.ApiUrl,
                ClientId = model.ClientId,
                ClientSecret = model.Secret,
                Roles = new[] { ApiRole.FullAccess }
            });

            await sourceClient.CleanupEnvironment(model);
            return new ObjectResult(new { Success = true });
        }

        [HttpPost("CloneCatalog")]
        public async Task<IActionResult> CloneCatalog([FromBody] CloneModel model)
        {
            var sourceClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = model.SourceApiUrl,
                AuthUrl = model.SourceApiUrl,
                ClientId = model.SourceClientId,
                ClientSecret = model.SourceSecret,
                Roles = new[] { ApiRole.FullAccess }
            });
            var destinationClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = model.DestinationApiUrl,
                AuthUrl = model.DestinationApiUrl,
                ClientId = model.DestinationClientId,
                ClientSecret = model.DestinationSecret,
                Roles = new[] { ApiRole.FullAccess }
            });

            await sourceClient.CloneCatalog(destinationClient);
            return new ObjectResult(new { Success = true });
        }

        [HttpPost("CleanupCatalog")]
        public async Task<IActionResult> CleanupCatalog([FromBody] CatalogCleanupModel model)
        {
            var sourceClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = model.ApiUrl,
                AuthUrl = model.ApiUrl,
                ClientId = model.ClientId,
                ClientSecret = model.Secret,
                Roles = new[] { ApiRole.FullAccess }
            });

            await sourceClient.CleanupCatalog();
            return new ObjectResult(new { Success = true });
        }
    }
}
