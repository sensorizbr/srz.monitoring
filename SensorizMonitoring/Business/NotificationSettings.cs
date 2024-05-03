using Database;
using MySql.Data.MySqlClient;
using SensorizMonitoring.Models;
using SensorizMonitoring.Models.NotificationsSettings;
using System.Data;
using System.Diagnostics;

namespace SensorizMonitoring.Business
{
    public class NotificationSettings
    {
        private readonly IConfiguration _configuration;

        public NotificationSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        DataTable dt = new DataTable();
        DataBase db = new DataBase();
        Globals utl = new Globals();

        public bool InsertNotificationSettings(NotificationSettingsModel mv)
        {
            try
            {
                string sql = string.Empty;

                //if (CompanyExists(mv.document))
                //{
                //    throw new Exception("Company already exists! " + mv.document + " - " + mv.name);
                //}

                sql += "INSERT INTO device_notification_settings " + Environment.NewLine;
                sql += "( " + Environment.NewLine;
                sql += "device_id, sensor_type_id, interval_flag, " + Environment.NewLine;
                sql += "start_value, end_value, exact_value, enabled, created_at " + Environment.NewLine;
                sql += ") VALUES ( " + Environment.NewLine;
                sql += "?device_id, ?sensor_type_id, ?interval_flag, " + Environment.NewLine;
                sql += "?start_value, ?end_value, ?exact_value, 1, NOW() " + Environment.NewLine;
                sql += "); " + Environment.NewLine;

                sql += "SELECT 100 AS RETORNO, LAST_INSERT_ID() AS LASTID;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?device_id", mv.device_id),
                    new MySqlParameter("?sensor_type_id", mv.sensor_type_id),
                    new MySqlParameter("?interval_flag", mv.interval_flag),
                    new MySqlParameter("?start_value", mv.start_value),
                    new MySqlParameter("?end_value", mv.end_value),
                    new MySqlParameter("?exact_value", mv.exatc_value)
                    );

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Inserted. " + dt.Rows[0]["LASTID"]);
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Insert Device Notification. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public bool UpdateNotificationSettings(NotificationSettingsModel mv, int id)
        {
            try
            {
                string sql = string.Empty;

                sql += "UPDATE device_notification_settings " + Environment.NewLine;
                sql += " SET " + Environment.NewLine;
                sql += " device_id = ?device_id " + Environment.NewLine;
                sql += ",sensor_type_id = ?sensor_type_id " + Environment.NewLine;
                sql += ",interval = ?interval " + Environment.NewLine;
                sql += ",start_value = ?start_value " + Environment.NewLine;
                sql += ",end_value = ?end_value " + Environment.NewLine;
                sql += ",exact_value = ?exact_value " + Environment.NewLine;
                sql += "WHERE id = ?id " + Environment.NewLine;
                sql += "SELECT 100 AS RETORNO;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?device_id", mv.device_id),
                    new MySqlParameter("?sensor_type_id", mv.sensor_type_id),
                    new MySqlParameter("?interval", mv.interval_flag),
                    new MySqlParameter("?start_value", mv.start_value),
                    new MySqlParameter("?end_value", mv.end_value),
                    new MySqlParameter("?exact_value", mv.exatc_value),
                    new MySqlParameter("?id", id)
                    );

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Updated. " + id);
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Update Notification Settings. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public bool DisableEnableNotificationSettings(int id, int flag)
        {
            try
            {
                string sql = string.Empty;

                sql += "UPDATE device_notification_settings " + Environment.NewLine;
                sql += " SET " + Environment.NewLine;
                sql += " enabled = ?enabled " + Environment.NewLine;
                sql += "WHERE id = ?id " + Environment.NewLine;
                sql += "SELECT 100 AS RETORNO;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?enabled", flag),
                    new MySqlParameter("?id", id)
                    );

                if (db.TrataRetorno(dt))
                {
                    if (flag == 1)
                    {
                        utl.EscreverArquivo("Enabled Notification Settings. " + id);
                    }
                    else
                    {
                        utl.EscreverArquivo("Disabled Notification Settings. " + id);
                    }

                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Enable/Disable Notification Settings. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public bool DeleteNotificationSettings(int id)
        {
            try
            {
                string sql = string.Empty;

                sql += "DELETE device_notification_settings " + Environment.NewLine;
                sql += "WHERE id = ?id " + Environment.NewLine;
                sql += "SELECT 100 AS RETORNO;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?id", id)
                    );

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Deleted. " + id);
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Delete Notification Settings. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public dynamic GetNotificationSettings(int limit, int page)
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT * FROM device_notification_settings " + Environment.NewLine;
                sql += "LIMIT ?limit offset ?offset " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?limit", limit),
                    new MySqlParameter("?offset", utl.OffSet(limit, page))
                    );

                return db.DTToJson(dt);
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }

        public dynamic GetNotificationSettingsById(int id)
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT * FROM device_notification_settings " + Environment.NewLine;
                sql += "WHERE id = ?id " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?id", id)
                    );

                return db.DTToJson(dt);
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }

        public dynamic GetNotificationSettingsByDeviceId(string DeviceID)
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT * FROM device_notification_settings " + Environment.NewLine;
                sql += "WHERE device_id = ?device_id " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?device_id", DeviceID)
                    );

                return db.DTToJson(dt);
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }
    }
}
