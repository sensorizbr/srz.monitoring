using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DeviceController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public DeviceController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Insere o dispositivo apenas na base da Sensoriz
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertDevice([FromBody] DeviceModel device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var insertCompany = new Device();

                insertCompany.device_code = device.device_code;
                insertCompany.company_id = device.company_id;
                insertCompany.description = device.description;
                insertCompany.device_reference_id = device.device_reference_id;
                insertCompany.enabled = 1;
                insertCompany.created_at = DateTime.Now;

                _context.Add(insertCompany);
                _context.SaveChanges();
                return Ok(insertCompany);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Atualiza o dispositivo apenas na base da Sensoriz
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] DeviceModel device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingDevice = _context.Device.Find(id);

                existingDevice.device_code = device.device_code;
                existingDevice.description = device.description;

                if (existingDevice == null)
                {
                    return NotFound("Device not found.");
                }

                _context.Update(existingDevice);
                _context.SaveChanges();
                return Ok(existingDevice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Ativa e desativa o dispositivo
        /// </summary>
        [HttpPut("{id}/{flag}")]
        public async Task<IActionResult> EnableDisableDevice(int id, int flag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingDevice = _context.Device.Find(id);

                existingDevice.enabled = flag;

                if (existingDevice == null)
                {
                    return NotFound("Device not found.");
                }

                _context.Update(existingDevice);
                _context.SaveChanges();
                return Ok(existingDevice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Remove o dispositivo
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> RemoveDevice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var delDevice = _context.Device.Find(id);
                _context.Remove(delDevice);
                _context.SaveChanges();
                return Ok("Registro removido!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Lista Todos os Dispositivos por CompanyID
        /// </summary>
        [HttpGet("{company_id}")]
        public async Task<IActionResult> GetDeviceByCompanyID(int company_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                List<Device> devices = await _context.Device
                  .Where(d => d.company_id == company_id)
                  .AsNoTracking()
                  .ToListAsync();

                return Ok(devices);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Pega as informações do Dispositivo na API da LocoAware pelo Device Code.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDeviceInformationByDeviceIdAsync(string sDeviceId)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            bnDevice dvc = new bnDevice(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a Listing...");

            // Lógica para manipular a solicitação POST

            return Content(await dvc.GetDeviceInformationByDeviceId(sDeviceId));
        }
    }
}