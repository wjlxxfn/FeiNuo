using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class PublicController : ControllerBase
    {
        private readonly ILogger<PublicController> logger;

        public PublicController(ILogger<PublicController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public string Test()
        {
            logger.LogDebug("debug,{a}", DateTime.Now);
            logger.LogInformation("information,{now}", DateTime.Now);
            logger.LogWarning("warning,{now}", DateTime.Now);
            logger.LogError("Error,{now}", DateTime.Now);

            return "success";
        }
    }
}
