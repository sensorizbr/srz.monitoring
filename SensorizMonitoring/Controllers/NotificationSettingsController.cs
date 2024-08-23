using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models.NotificationsSettings;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class NotificationSettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public NotificationSettingsController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Cria a configuração de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertNotificationSettings([FromBody] NotificationSettingsModel ns)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Globals gb = new Globals();
                var insertNS = new NotificationSettings();

                insertNS.device_id = ns.device_id;
                insertNS.branch_id = ns.branch_id;
                insertNS.description = ns.description;
                insertNS.sensor_type_id = ns.sensor_type_id;
                insertNS.comparation_id = ns.comparation_id;
                insertNS.min_value = ns.min_value;
                insertNS.max_value = ns.max_value;
                insertNS.lat_origin = ns.lat_origin;
                insertNS.long_origin = ns.long_origin;
                insertNS.lat_destination = ns.lat_destination;
                insertNS.long_destination = ns.long_destination;
                insertNS.tolerance_radius = ns.tolerance_radius;
                insertNS.b_value = ns.b_value;
                insertNS.enabled = 1;
                insertNS.created_at = DateTime.Now;

                _context.Add(insertNS);
                _context.SaveChanges();
                return Ok(insertNS);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error insert notification settings: " + ex.Message);
            }
        }

        /// <summary>
        /// Atualiza a configuração de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotificationSettings(int id, [FromBody] NotificationSettings ns)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingNS = _context.NotificationSettings.Find(id);

                existingNS.device_id = ns.device_id;
                existingNS.branch_id = ns.branch_id;
                existingNS.description = ns.description;
                existingNS.sensor_type_id = ns.sensor_type_id;
                existingNS.comparation_id = ns.comparation_id;
                existingNS.min_value = ns.min_value;
                existingNS.max_value = ns.max_value;
                existingNS.lat_origin = ns.lat_origin;
                existingNS.long_origin = ns.long_origin;
                existingNS.lat_destination = ns.lat_destination;
                existingNS.long_destination = ns.long_destination;
                existingNS.b_value = ns.b_value;
                existingNS.enabled = 1;
                existingNS.created_at = DateTime.Now;

                if (existingNS == null)
                {
                    return NotFound("Notification Settings not found.");
                }

                _context.Update(existingNS);
                _context.SaveChanges();
                return Ok(existingNS);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error to update notification settings: " + ex.Message);
            }
        }



        /// <summary>
        /// Remove a configuração de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> DeleteNotificationSettings(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var delNS = await _context.NotificationSettings.FindAsync(id);
                _context.Remove(delNS);
                _context.SaveChanges();
                return Ok("Registro removido!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Ativa/Desativa a configuração de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpPut("{id}/{flag}")]
        public async Task<IActionResult> EnableNotificationSettings(int id, int flag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingNS = await _context.NotificationSettings.FindAsync(id);

                existingNS.enabled = flag;

                if (existingNS == null)
                {
                    return NotFound("Notification Settings not found.");
                }

                _context.Update(existingNS);
                _context.SaveChanges();
                return Ok(existingNS);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error updating notification settings: " + ex.Message);
            }
        }


        /// <summary>
        /// Lista todas as configurações de forma paginada
        /// </summary>
        [HttpGet("{limit}/{page}")]
        public async Task<IActionResult> GetAllNotificationsSettings(int iBranchId, int limit, int page)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingNS = await _context.NotificationSettings.AsNoTracking().ToListAsync<NotificationSettings>();

                // Lógica para manipular a solicitação POST
                var notificationsettings = existingNS
                    .Where(c => c.branch_id == iBranchId)
                   .OrderBy(c => c.created_at) // or any other column you want to sort by
                   .Skip((page - 1) * limit)
                   .Take(limit);

                return Ok(notificationsettings.ToList());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Notification Settings: " + ex.Message);
            }
        }

        /// <summary>
        /// Lista a configuração de uma notificação de um determinado dispositivo por ID da Configuração
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationSettingsById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                NotificationSettings ns = await _context.NotificationSettings
                  .Where(d => d.id == id)
                  .AsNoTracking()
                  .FirstOrDefaultAsync();

                return Ok(ns);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Notification Settings: " + ex.Message);
            }
        }


        /// <summary>
        /// Lista a configuração de uma notificação de um determinado dispositivo por ID do Dispositivo
        /// </summary>
        [HttpGet("{device_id}")]
        public async Task<IActionResult> GetNotificationSettingsByDeviceId(long device_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                List<NotificationSettings> ns = await _context.NotificationSettings
                  .Where(d => d.device_id == device_id)
                  .AsNoTracking()
                  .ToListAsync();

                return Ok(ns);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Notification Settings: " + ex.Message);
            }
        }
    }
}