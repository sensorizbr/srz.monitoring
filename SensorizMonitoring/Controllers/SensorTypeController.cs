using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiKey]
    [ApiController]
    public class SensorTypeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public SensorTypeController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Lista Todos os Tipos de Sensores dos Dispositivos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllSensorType()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var sensorTypeCompany = await _context.SensorType.AsNoTracking().ToListAsync<SensorType>();

                // L�gica para manipular a solicita��o POST
                var sensorTypes = sensorTypeCompany;

                return Ok(sensorTypes.ToList());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Company: " + ex.Message);
            }
        }
    }
}