using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using SensorizMonitoring.Utils;
using static SensorizMonitoring.Business.bnDevice;

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
                insertCompany.branch_id = device.branch_id;
                insertCompany.description = device.description;
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
        [HttpDelete("{id}")]
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
        [HttpGet]
        public async Task<IActionResult> GetDeviceByBranchId(int iBranchID)
        {
            bnDevice dv = new bnDevice(_configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var devices = await _context.Device
                .Where(d => d.branch_id == iBranchID)
                .AsNoTracking()
                .Select(d => new DeviceResponseTable
                {
                    id = d.id,
                    device_code = d.device_code,
                    description = d.description,
                    created_at = d.created_at,
                })
                .ToListAsync();

                LocoDevicesResponse dr = await dv.GetDevicesToList();
                List<DeviceResponseTable> dvcTable = new List<DeviceResponseTable>();

                // Merge the lists
                foreach (var locoDevice in dr.results)
                {
                    var device = devices.Find(d => d.device_code == locoDevice.id);
                    if (device != null)
                    {
                        device.model = locoDevice.model.product;
                        if (locoDevice.firmware != null) { device.firmware = locoDevice.firmware.current; }
                        device.charging = locoDevice.statusIndicators.charging;
                        if (locoDevice.lastKnownLocation != null) {
                            device.lkl_lat = locoDevice.lastKnownLocation.global.lat;
                            device.lkl_lng = locoDevice.lastKnownLocation.global.lon;
                        }
                        if (locoDevice.statusIndicators.battery != null) { device.battery = locoDevice.statusIndicators.battery; }
                    }
                }

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
        public IActionResult GetDeviceInformationByDeviceId(string sDeviceId)
        {
            string sql = string.Empty;

            var baseUrl = _configuration["Settings:LocoBaseUrl"];
            var token = _configuration["Settings:LocoToken"];
            //var endpoint = "getDevice";

            Include inc = new Include();
            inc.pairings = false;

            DeviceInformationRequest dir = new DeviceInformationRequest();
            dir.device = sDeviceId;
            dir.include = inc;

            var apiClient = new ApiClient(baseUrl, token);

            try
            {
                string data = apiClient.GetApiData(baseUrl + "device/" + sDeviceId);
                Console.WriteLine($"Dados obtidos com sucesso: {data}");

                // Deserialize the JSON data into a dynamic object
                var serializer = new JavaScriptSerializer();
                dynamic deviceInfo = serializer.Deserialize<dynamic>(data);

                // Return the deserialized object as JSON
                return Ok(deviceInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter dados: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}