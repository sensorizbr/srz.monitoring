using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Business
{
    public class bnMonitoring
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        public bnMonitoring(IConfiguration configuration, AppDbContext context, ILogger logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public bool InsertMonitoring(MonitoringModel mnt, long seq)
        {
            try
            {
                Globals gb = new Globals();

                var insertMonitoring = new Monitoring();

                insertMonitoring.device_id = mnt.deviceId;
                insertMonitoring.seq = seq;
                insertMonitoring.temperature = mnt.status.temperature;
                insertMonitoring.atmospheric_pressure = mnt.status.atmosphericPressure;
                insertMonitoring.lat = mnt.pos?.lat;
                insertMonitoring.lon = mnt.pos?.lon;
                insertMonitoring.cep = mnt.pos?.cep;
                insertMonitoring.external_power = mnt.status.externalPower;
                insertMonitoring.charging = mnt.status.charging;
                insertMonitoring.battery_voltage = mnt.status.batteryVoltage;
                insertMonitoring.light_level = mnt.status.lightLevel;
                insertMonitoring.orientation_x = mnt.status.orientation?.x;
                insertMonitoring.orientation_y = mnt.status.orientation?.y;
                insertMonitoring.orientation_z = mnt.status.orientation?.z;
                insertMonitoring.vibration_x = mnt.status.vibration?.x;
                insertMonitoring.vibration_y = mnt.status.vibration?.y;
                insertMonitoring.vibration_z = mnt.status.vibration?.z;
                insertMonitoring.com_signal = mnt.status.signal;
                insertMonitoring.tamper = mnt.status.tamper;
                insertMonitoring.movement = mnt.status.movement;
                insertMonitoring.created_at = gb.ToBRDateTimeDT(DateTime.Now);
                insertMonitoring.report_date = gb.ToBRDateTime(mnt.rxTime);

                _context.Add(insertMonitoring);
                _context.SaveChanges();
                //_context.Dispose();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCritical(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return false;
            }
        }
    }
}
