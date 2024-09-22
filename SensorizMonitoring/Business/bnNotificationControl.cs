using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;

namespace SensorizMonitoring.Business
{
    public class bnNotificationControl
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public bnNotificationControl(IConfiguration configuration, AppDbContext context, ILogger logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public bool ExistsNotificationControl(NotificationSettings ns)
        {
            return _context.NotificationControl
                .Any(nc => nc.device_id == ns.device_id && nc.notification_id == ns.id);
        }

        public bool InsertNotificationControl(NotificationSettings ns)
        {
            try
            {
                if (!ExistsNotificationControl(ns))
                {
                    var insertNotifControl = new NotificationControl();

                    insertNotifControl.device_id = ns.device_id;
                    insertNotifControl.notification_id = ns.id;

                    _context.Add(insertNotifControl);
                    _context.SaveChanges();
                    //_context.Dispose();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                return false;
            }
        }
        public bool DeleteNotificationControl(long device_id, int notification_id, bool isRecovery)
        {
            try
            {
                if (isRecovery)
                {
                    var notifControl = _context.NotificationControl.FirstOrDefault(nc => nc.device_id == device_id && nc.notification_id == notification_id);
                    if (notifControl != null)
                    {
                        _context.NotificationControl.Remove(notifControl);
                        _context.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                return false;
            }
        }

    }
}
