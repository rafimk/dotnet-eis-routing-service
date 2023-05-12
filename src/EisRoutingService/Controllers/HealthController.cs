using EisRoutingService.Options;
using EisRoutingService.Publishers;
using EisRoutingService.Subscribers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EisRoutingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        //private readonly IMessagePublisher _publisher;
        //private readonly IMessageSubscriber _subscriber;
        private readonly EisRoutingOption _option;
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger, EisRoutingOption option)
        {
            // _publisher = publisher;
            _logger = logger;
            _option = option;
            // _subscriber = subscriber;
        }

        [HttpGet()]
        public ActionResult<string> Health()
        {
           return Ok("Healthy.......");
        }

        [HttpGet("Routs")]
        public ActionResult<EisRoutingOption> Routs()
        {
            return Ok(_option);
        }

    }
}