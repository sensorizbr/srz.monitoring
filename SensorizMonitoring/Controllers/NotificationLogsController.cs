using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Templates;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiKey]
    [ApiController]
    public class NotificationLogsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public NotificationLogsController(IConfiguration configuration, AppDbContext context, ILogger<NotificationLogsController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos o Log de Notificações de forma paginada
        /// </summary>
        [HttpGet("{limit}/{page}")]
        public async Task<IActionResult> GetAllNotificationsLogs(int iBranchId, long iDeviceID, int limit, int page)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var notificationLogs = await _context.NotificationLog
                    .AsNoTracking()
                    .Where(c => c.branch_id == iBranchId && (iDeviceID == 0 || c.device_id == iDeviceID))
                    .Join(_context.NotificationSettings, nl => nl.setting_id, dns => dns.id, (nl, dns) => new { nl, dns })
                    .Join(_context.Device, x => x.nl.device_id, d => d.device_code, (x, d) => new
                    {
                        x.nl.id,
                        x.nl.branch_id,
                        x.nl.seq,
                        DeviceId = x.nl.device_id,
                        DeviceDescription = d.description,
                        SettingDescription = x.dns.description,
                        x.nl.mail,
                        x.nl.phone_number,
                        //message_response = JsonConvert.DeserializeObject<WhatsAppMensagem>(x.nl.message_response),
                        //message_request = JsonConvert.DeserializeObject<WhatsAppMensagem>(x.nl.message_request),
                        message_request = x.nl.message_request,
                        message_response = x.nl.message_response,
                        x.nl.created_at
                    })
                    .OrderBy(c => c.created_at)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                return Ok(notificationLogs);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"Error Listing Notification Logs: {ex.Message}");
            }
        }
    }
}