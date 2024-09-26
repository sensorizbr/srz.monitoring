using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using SensorizMonitoring.Utils;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MonitoringController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public MonitoringController(IConfiguration configuration, AppDbContext context, ILogger<MonitoringController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Recebe as notificações dos dispositivos da LocoAware
        /// </summary>

        [HttpPost]
        //public async Task<IActionResult> InsertMonitoring([FromBody] MonitoringModel mnt)
        public IActionResult InsertMonitoring([FromBody] dynamic json)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string jsonString = json.ToString();
                MonitoringModel mnt = JsonConvert.DeserializeObject<MonitoringModel>(jsonString);

                DecisionRules dec = new DecisionRules(_configuration, _context, _logger);
                Globals gb = new Globals();

                dec.GetNotificationSettings(mnt);

                return Ok(mnt);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error insert monitoring: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error insert monitoring: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CalculaDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            Globals gb = new Globals();
            return Ok(gb.CalculateDistance(lat1, lon1, lat2, lon2));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Monitoring>>> GetMonitoring()
        {
            var monitorings = await _context.Monitoring.AsNoTracking().ToListAsync();
            return Ok(monitorings);
        }

        //[HttpPost]
        //public async Task<ActionResult> ZenviaTest(string sPhoneNumber)
        //{
        //    var smsSender = new bnZenvia(_configuration, _logger);
        //    smsSender.SendSms(sPhoneNumber, "SENSORIZ TEST");
        //    return Ok("Enviado com sucesso!");
        //}

        [HttpPost]
        public async Task<ActionResult> GetAddressFromLatLong(double lat, double lon)
        {
            GoogleLocation gl = new GoogleLocation(_configuration);
            return Ok(gl.GetAddressByCoordinators(lat, lon));
        }
    }
}