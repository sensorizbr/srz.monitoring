using Microsoft.AspNetCore.Mvc;
using SensorizMonitoring.Business;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DeviceReferenceController : Controller
    {
        private readonly IConfiguration _configuration;

        public DeviceReferenceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Rota POST: api/Exemplo
        [HttpPost]
        public IActionResult SincronizeDeviceReferences()
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            DeviceReference mf = new DeviceReference(_configuration);
            Globals utl = new Globals();

            if (mf.SincronizeDeviceReferences())
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}