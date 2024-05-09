using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
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

                // Lógica para manipular a solicitação POST
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