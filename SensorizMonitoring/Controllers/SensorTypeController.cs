using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SensorTypeController : Controller
    {
        private readonly IConfiguration _configuration;

        public SensorTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllSensorType()
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            SensorType st = new SensorType(_configuration);
            Globals utl = new Globals();

            return Ok(st.GetAllSensorType());
        }
    }
}