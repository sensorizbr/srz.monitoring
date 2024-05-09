using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;

namespace SensorizMonitoring.Business
{
    public class bnNotificationOwner
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public bnNotificationOwner(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public List<NotificationOwner> GetNotificationOwnersForDevice(long deviceId)
        {
            if (deviceId == 0) throw new ArgumentNullException(nameof(deviceId));

            return _context.NotificationOwner
              .Where(d => d.device_id == deviceId)
              .AsNoTracking()
              .ToList();
        }
    }
}
