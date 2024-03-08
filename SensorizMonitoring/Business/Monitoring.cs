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

        public bool InsertMonitoring(MonitoringModel mv)
        {
            try
            {
                string sql = string.Empty;

                sql += "INSERT INTO monitoring ";
                sql += "( ";
                sql += "device_id, temperature, atmospheric_pressure, created_at ";
                sql += ") ";
                sql += "VALUES ";
                sql += "( ";
                sql += "?device_id, ?temperature, ?atmospheric_pressure, NOW() ";
                sql += "); ";

                sql += "SELECT 100 AS RETORNO;";

                dt = db.SelectAccessDBSqlServer(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?device_id", mv.deviceId),
                    new MySqlParameter("?temperature", mv.status.temperature),
                    new MySqlParameter("?atmospheric_pressure", mv.status.atmosphericPressure)
                    );

                if (db.TrataRetorno(dt))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
