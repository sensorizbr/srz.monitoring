using Database;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SensorizMonitoring.Models;
using System.Data;
using System.Diagnostics;

namespace SensorizMonitoring.Business
{
    public class SensorType
    {
        private readonly IConfiguration _configuration;

        public SensorType(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        DataTable dt = new DataTable();
        DataBase db = new DataBase();
        Globals utl = new Globals();

        public dynamic GetAllSensorType()
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT id, description FROM sensor_type " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    null
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
