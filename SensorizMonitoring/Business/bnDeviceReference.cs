using Database;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using SensorizMonitoring.Models;
using SensorizMonitoring.Models.DeviceReference;
using SensorizMonitoring.Utils;
using System.Data;

namespace SensorizMonitoring.Business
{
    public class bnDeviceReference
    {
        private readonly IConfiguration _configuration;

        public bnDeviceReference(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        DataTable dt = new DataTable();
        DataBase db = new DataBase();
        Globals utl = new Globals();

        public bool SincronizeDeviceReferences()
        {
            try
            {
                string sql = string.Empty;

                //var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "Data", "family_model.json");
                //var conteudoJson = File.ReadAllText(caminhoArquivo);

                var baseUrl = _configuration["Settings:LocoBaseUrl"];
                var token = _configuration["Settings:LocoToken"];
                var endpoint = "deviceModels";

                var apiClient = new SensorizMonitoring.Utils.ApiClient(baseUrl, token);
                var apiData = apiClient.GetApiData(endpoint);

                if (!string.IsNullOrEmpty(apiData))
                {
                    if (TableRowsExists())
                    {
                        if (TruncateDevicereferenceTable())
                        {
                            LoopingAndInsertDeviceReference(apiData);
                        }
                        else
                        {
                            throw new Exception("Was not possible clear the Device Reference Table");
                        }
                    }
                    else
                    {
                        // Agora você pode desserializar o conteúdo JSON em um objeto C#
                        LoopingAndInsertDeviceReference(apiData);
                    }
                }
                else
                {
                    throw new Exception("Was not possible consuming API!");
                }

                return true;

            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        private void LoopingAndInsertDeviceReference(string apiData)
        {
            // Agora você pode desserializar o conteúdo JSON em um objeto C#
            ReferenceModel mdl = JsonConvert.DeserializeObject<ReferenceModel>(apiData);

            if (mdl.models.Count > 0)
            {
                for (int a = 0; a <= mdl.models.Count - 1; a++)
                {
                    InsertDeviceReference(mdl.models[a]);
                }
            }
        }

        public bool InsertDeviceReference(ReferenceFamilyModel mv)
        {
            try
            {
                string sql = string.Empty;

                sql += "INSERT INTO device_reference " + Environment.NewLine;
                sql += "( " + Environment.NewLine;
                sql += "family_name, name, reader, " + Environment.NewLine;
                sql += "tag, profiles, settings, " + Environment.NewLine;
                sql += "otaFirmware, flightMode, oceanMode, " + Environment.NewLine;
                sql += "movementDetection, shockDetection, tipDetection, " + Environment.NewLine;
                sql += "wifi, sensors, created_at " + Environment.NewLine;
                sql += ") VALUES ( " + Environment.NewLine;
                sql += "?family_name, ?name, ?reader, " + Environment.NewLine;
                sql += "?tag, ?profiles, ?settings, " + Environment.NewLine;
                sql += "?otaFirmware, ?flightMode, ?oceanMode, " + Environment.NewLine;
                sql += "?movementDetection, ?shockDetection, ?tipDetection, " + Environment.NewLine;
                sql += "?wifi, ?sensors, NOW() " + Environment.NewLine;
                sql += "); " + Environment.NewLine;

                sql += "SELECT 100 AS RETORNO, LAST_INSERT_ID() AS LASTID;";

                string strSensors = string.Join(", ", mv.capabilities.sensors);

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?family_name", mv.family.Trim().ToUpper()),
                    new MySqlParameter("?name", mv.name.Trim().ToUpper()),
                    new MySqlParameter("?reader", mv.capabilities.reader),
                    new MySqlParameter("?tag", mv.capabilities.tag),
                    new MySqlParameter("?profiles", mv.capabilities.profiles),
                    new MySqlParameter("?settings", mv.capabilities.settings),
                    new MySqlParameter("?otaFirmware", mv.capabilities.otaFirmware),
                    new MySqlParameter("?flightMode", mv.capabilities.flightModel),
                    new MySqlParameter("?oceanMode", mv.capabilities.oceanModel),
                    new MySqlParameter("?movementDetection", mv.capabilities.movementDetection),
                    new MySqlParameter("?shockDetection", mv.capabilities.shockDetection),
                    new MySqlParameter("?tipDetection", mv.capabilities.tipDetection),
                    new MySqlParameter("?wifi", mv.capabilities.wifi),
                    new MySqlParameter("?sensors", strSensors.Trim())
                    );

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Inserted. " + dt.Rows[0]["LASTID"]);
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Insert Company. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public bool TruncateDevicereferenceTable()
        {
            try
            {
                string sql = string.Empty;

                sql += "TRUNCATE TABLE device_reference; " + Environment.NewLine;
                sql += "SELECT 100 AS RETORNO;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"), null);

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Truncated Device Reference Table");
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Device Reference Table Company. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public bool TableRowsExists()
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT COUNT(*) as QTD FROM device_reference " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"), null
                    );

                return db.TrataExists(dt);
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public dynamic GetAllDeviceReference()
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT id, name, sensors FROM device_reference " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"), null);

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
