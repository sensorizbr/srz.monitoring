using Database;
using MySql.Data.MySqlClient;
using SensorizMonitoring.Models;
using System.Data;
using System.Diagnostics;

namespace SensorizMonitoring.Business
{
    public class Monitoring
    {
        private readonly IConfiguration _configuration;

        public Monitoring(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        DataTable dt = new DataTable();
        DataBase db = new DataBase();
        Globals utl = new Globals();

        public bool InsertMonitoring(MonitoringModel mv)
        {
            try
            {
                string sql = string.Empty;

                sql += "INSERT INTO monitoring " + Environment.NewLine;
                sql += "( " + Environment.NewLine;
                sql += "device_id, temperature, atmospheric_pressure, " + Environment.NewLine;
                sql += "lat, lon, cep, external_power, " + Environment.NewLine;
                sql += "charging, battery_voltage, light_level, " + Environment.NewLine;
                sql += "orientation_x, orientation_y, orientation_z, " + Environment.NewLine;
                sql += "vibration_x, vibration_y, vibration_z, " + Environment.NewLine;
                sql += "com_signal, tamper, movement, created_at " + Environment.NewLine;
                sql += ") VALUES ( " + Environment.NewLine;
                sql += "?device_id, ?temperature, ?atmospheric_pressure, " + Environment.NewLine;
                sql += "?lat, ?lon, ?cep, ?external_power, " + Environment.NewLine;
                sql += "?charging, ?battery_voltage, ?light_level, " + Environment.NewLine;
                sql += "?orientation_x, ?orientation_y, ?orientation_z, " + Environment.NewLine;
                sql += "?vibration_x, ?vibration_y, ?vibration_z, " + Environment.NewLine;
                sql += "?com_signal, ?tamper, ?movement, NOW() " + Environment.NewLine;
                sql += "); " + Environment.NewLine; 

                sql += "SELECT 100 AS RETORNO;";

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?device_id", mv.deviceId),
                    new MySqlParameter("?temperature",  mv.status.temperature),
                    new MySqlParameter("?atmospheric_pressure",  mv.status.atmosphericPressure),
                    new MySqlParameter("?lat",  mv.pos.lat),
                    new MySqlParameter("?lon",  mv.pos.lon),
                    new MySqlParameter("?cep",  mv.pos.cep),
                    new MySqlParameter("?external_power", mv.status.externalPower),
                    new MySqlParameter("?charging", mv.status.charging),
                    new MySqlParameter("?battery_voltage",  mv.status.batteryVoltage),
                    new MySqlParameter("?light_level", mv.status.lightLevel),
                    new MySqlParameter("?orientation_x",  mv.status.orientation.x),
                    new MySqlParameter("?orientation_y",  mv.status.orientation.y),
                    new MySqlParameter("?orientation_z",  mv.status.orientation.z),
                    new MySqlParameter("?vibration_x",  mv.status.vibration.x),
                    new MySqlParameter("?vibration_y",  mv.status.vibration.y),
                    new MySqlParameter("?vibration_z",  mv.status.vibration.z),
                    new MySqlParameter("?com_signal", mv.status.signal),
                    new MySqlParameter("?tamper", mv.status.tamper),
                    new MySqlParameter("?movement", mv.status.movement)
                );

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Inserted. " + mv.deviceId);
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Insert Monitoring. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }
    }
}
