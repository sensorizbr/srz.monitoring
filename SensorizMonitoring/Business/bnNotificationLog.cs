using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;

namespace SensorizMonitoring.Business
{
    public class bnNotificationLog
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public bnNotificationLog(IConfiguration configuration, AppDbContext context, ILogger logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public bool InsertNotificationLog(NotificationSettings st,
                                                   NotificationOwner on,
                                                   long seq,
                                                   string sMessageRequest,
                                                   string sMessageResponse,
                                                   string sDeviceDescription)
        {
            try
            {
                var insertNotificationLog = new NotificationLog();

                insertNotificationLog.device_id = st.device_id;
                insertNotificationLog.branch_id = st.branch_id;
                insertNotificationLog.seq = seq;
                insertNotificationLog.setting_id = st.id;
                insertNotificationLog.phone_number = on.phone_number;
                insertNotificationLog.mail = on.mail;
                insertNotificationLog.message_request = JsonConvert.SerializeObject(sMessageRequest);
                insertNotificationLog.message_response = sMessageResponse;
                insertNotificationLog.created_at = DateTime.Now;

                _context.Add(insertNotificationLog);
                _context.SaveChanges();
                //_context.Dispose();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                return false;
            }
        }

    }
}
