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

        /// <summary>
        /// Sincroniza Todos os Modelos de Dispositivos, sincronizãndo diretamente com a API da LocoAware
        /// </summary>
        [HttpPost]
        public IActionResult SincronizeDeviceReferences()
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            bnDeviceReference mf = new bnDeviceReference(_configuration);
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

        /// <summary>
        /// Lista todos os modelos de dispositivos na base de dados da Sensoriz
        /// </summary>
        [HttpGet]
        public IActionResult GetAllDeviceReference()
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            bnDeviceReference dr = new bnDeviceReference(_configuration);
            Globals utl = new Globals();

            return Ok(dr.GetAllDeviceReference());
        }
    }
}