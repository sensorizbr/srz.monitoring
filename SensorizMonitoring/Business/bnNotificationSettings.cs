using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;

namespace SensorizMonitoring.Business
{
    public class bnNotificationSettings
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public bnNotificationSettings(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public List<NotificationSettings> GetNotificationSettingsForDevice(long deviceId)
        {
            try
            {
                if (deviceId == 0) throw new ArgumentNullException(nameof(deviceId));

                List<NotificationSettings> lst = new List<NotificationSettings>();

                lst = _context.NotificationSettings
                   .Where(d => d.device_id == deviceId && d.enabled == 1)
                   .AsNoTracking()
                   .ToList();

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
