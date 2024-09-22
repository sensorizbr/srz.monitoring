using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using SensorizMonitoring.Utils;
using System.Reflection;
using ZenviaApi;
using static SensorizMonitoring.Models.ZenviaResposeModel;

namespace SensorizMonitoring.Business
{
    public class DecisionRules
    {

        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        Globals gb = new Globals();
             

        public DecisionRules(IConfiguration configuration, AppDbContext context, ILogger logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public bool GetNotificationSettings(MonitoringModel monit)
        {
            try
            {
                _logger.LogInformation("Iniciando telemetria do dispositivo: ");
                _logger.LogInformation(monit.deviceId.ToString());

                bnNotificationSettings ns = new bnNotificationSettings(_configuration, _context);
                List<NotificationSettings> lstSettings = ns.GetNotificationSettingsForDevice(monit.deviceId);

                long seq = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                if (lstSettings != null && lstSettings.Count > 0)
                {
                    string sMessage = string.Empty;
                    string sSensor = string.Empty;
                    string sTemplateID = string.Empty;
                    bool isRecovery = false;
                    dynamic fields = false;
                    bool doTelemetry = false;

                    _logger.LogInformation("Achei configuração...");
                    foreach (var setting in lstSettings)
                    {
                        if (ShouldSendNotification(setting, monit, ref fields, ref sSensor, ref isRecovery, ref sTemplateID))
                        {
                            bnNotificationOwner no = new bnNotificationOwner(_configuration, _context);
                            List<NotificationOwner> lstOwners = no.GetNotificationOwnersForDevice(monit.deviceId, setting.id);

                            _logger.LogInformation("É passível de notificação!");
                            if (lstOwners != null && lstOwners.Count > 0)
                            {
                                foreach (var owner in lstOwners)
                                {
                                    _logger.LogInformation("Tem owner!");
                                    _logger.LogInformation(owner.phone_number);
                                    _logger.LogInformation(owner.mail);
                                    if (SendNotification(setting, owner, fields, monit, sTemplateID, seq))
                                    {
                                        doTelemetry = true;
                                        bnNotificationControl nc = new bnNotificationControl(_configuration, _context, _logger);

                                        if (isRecovery)
                                        {

                                            _logger.LogInformation("É recovery");
                                            nc.DeleteNotificationControl(setting.device_id, setting.id, isRecovery);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("É notificação normal");
                                            nc.InsertNotificationControl(setting);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (doTelemetry)
                    {
                        _logger.LogInformation("Notificamos... Vamos inserir no monitoramento.");
                        bnMonitoring monitoring = new bnMonitoring(_configuration, _context, _logger);
                        monitoring.InsertMonitoring(monit, seq); //Insertindo o monitoramento apenas se notificar
                    }
                }
                else
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                return false;
            }
        }

        private bool ShouldSendNotification(NotificationSettings setting, MonitoringModel monit, ref dynamic fields, ref string sSensor, ref bool isRecovery, ref string sTemplateID)
        {
            sSensor = "";

            bool ResultMath = false;
            dynamic monitoringValue = null;
            string unitOfMeasurement = "";
            bnNotificationControl nc = new bnNotificationControl(_configuration, _context, _logger);
            isRecovery = nc.ExistsNotificationControl(setting);
            string evento = string.Empty;
            bnMathConditions mc = new bnMathConditions();

            Globals gb = new Globals();

            switch (setting.sensor_type_id)
            {
                case 1: //Temperature


                    ResultMath = mc.CheckMathConditionDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.temperature, isRecovery);
                    if (isRecovery) setting.priority = "RETORNO AO PADRÃO";

                    monitoringValue = monit.status.temperature;
                    unitOfMeasurement = " ºC";
                    fields = GetMessageTemplate(setting, monit.status.temperature, 0, 0, unitOfMeasurement, "");
                    sTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID"];

                    break;
                case 2: //athmospheric pressure

                    ResultMath = mc.CheckMathConditionDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.atmosphericPressure, isRecovery);
                    if (isRecovery) setting.priority = "RETORNO AO PADRÃO";

                    unitOfMeasurement = " hPa";
                    fields = GetMessageTemplate(setting, monit.status.atmosphericPressure, 0, 0, unitOfMeasurement, "");
                    sTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID"];

                    break;
                case 3: //lat (entrada e saída)
                    if (monit.pos is not null && (monit.status.movement.Equals("STATIONARY")))
                    {
                        ResultMath = mc.CheckMathComparation_Directions_GetInGetOut(setting.comparation_id, setting.lat_origin, setting.long_origin, monit.pos.lat, monit.pos.lon, setting.tolerance_radius, ref evento, isRecovery);

                        fields = GetMessageTemplate(setting, 0, monit.pos.lat, monit.pos.lon, unitOfMeasurement, evento);
                        sTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID_GEOINOUT"];
                    }
                    else if (monit.pos is not null && monit.status.movement.Equals("MOVING"))
                    {
                        evento = "EM MOVIMENTAÇÃO";
                        ResultMath = true;
                    }
                    else
                    {
                        ResultMath = false;
                    }
                    break;
                case 4: //Geolocation Router
                    //monit.pos.lon.ToString());
                    bnSensorRoute route = new bnSensorRoute();
                    ResultMath = route.isOnRoute(setting.lat_origin, setting.long_origin, setting.lat_destination, setting.long_destination, monit.pos.lat, monit.pos.lon, (double)setting.tolerance_radius);
                    break;
                case 5: //external power
                    ResultMath = mc.CheckMathComparationBool(setting.comparation_id, setting.b_value, monit.status.externalPower);
                    monitoringValue = monit.status.externalPower;
                    break;
                case 6: //charging
                    ResultMath = mc.CheckMathComparationBool(setting.comparation_id, setting.b_value, monit.status.charging);
                    monitoringValue = monit.status.charging;
                    break;
                case 7: //battery_voltage
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.batteryVoltage);
                    //monitoringValue = monit.status.batteryVoltage;
                    break;
                case 8: //light_level
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.lightLevel);
                    //monitoringValue = monit.status.lightLevel;
                    break;
                case 9: //orientation x
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.x);
                    //monitoringValue = monit.status.orientation.x;
                    break;
                case 10: //orientation y
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.y);
                    //monitoringValue = monit.status.orientation.y;
                    break;
                case 11: //orientation z
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.z);
                    //monitoringValue = monit.status.orientation.z;
                    break;
                case 12: //vibration x
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.x);
                    //monitoringValue = monit.status.vibration.x;
                    break;
                case 13: //vibration y
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.y);
                    //monitoringValue = monit.status.vibration.y;
                    break;
                case 14: //vibration z
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.z);
                    //monitoringValue = monit.status.vibration.z;
                    break;
                case 15: //comm_signal
                    //ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.signal);
                    //monitoringValue = monit.status.signal;
                    break;
                case 16: //tamper
                    ResultMath = mc.CheckMathComparationBool(setting.comparation_id, setting.b_value, gb.IntToBool(monit.status.tamper));
                    monitoringValue = monit.status.tamper;
                    break;
                case 17: //movement
                    ResultMath = false;
                    break;
                case 18: //Directions
                    ResultMath = false;
                    break;
                case 19: //Impact
                         //ResultMath = false;
                    if (monit.@event is not null && (monit.@event.type == "impact"))
                    {
                        ResultMath = true;
                        monitoringValue = monit.@event.G;

                        isRecovery = true;

                        unitOfMeasurement = "";
                        fields = GetMessageTemplate(setting, monit.@event.G, monit.pos.lat, monit.pos.lon, unitOfMeasurement, "IMPACTO");
                        sTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID_DROPIMPACT"];
                    }
                    break;
                case 20: //Drop
                         //ResultMath = false;
                    if (monit.@event is not null && (monit.@event.type == "drop"))
                    {
                        ResultMath = true;
                        monitoringValue = monit.@event.G;

                        isRecovery = true;

                        unitOfMeasurement = "";
                        fields = GetMessageTemplate(setting, monit.@event.G, monit.pos.lat, monit.pos.lon, unitOfMeasurement, "QUEDA");
                        sTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID_DROPIMPACT"];
                    }
                    break;
                default:
                    return false;
            }

            if (ResultMath)
            {
                sSensor = GetDescriptionFromValue(setting.sensor_type_id);
            }

            return ResultMath;
        }

        public static string GetDescriptionFromValue(int value)
        {
            enumSensorType sensorType = (enumSensorType)System.Enum.ToObject(typeof(enumSensorType), value);
            return GetDescription(sensorType);
        }

        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public dynamic GetMessageTemplate(NotificationSettings setting, dynamic value, double lat, double lon, string unitOfMeasurement, string evento)
        {
            GoogleLocation gl = new GoogleLocation(_configuration);
            switch (setting.sensor_type_id)
            {
                case 1:
                case 2:
                    return new
                    {
                        icon = DecideIcon(setting.priority),
                        codigo = setting.device_id.ToString(),
                        sensor = GetDescriptionFromValue(setting.sensor_type_id),
                        priority = setting.priority,
                        valor = value.ToString() + unitOfMeasurement,
                        valor_min = setting.min_value.ToString() + unitOfMeasurement,
                        valor_max = setting.max_value.ToString() + unitOfMeasurement,
                        current_location = gl.GetGoogleAddress(lat, lon)
                    };
                    break;
                case 3:
                    //Encaixe Template Geolocation
                    return new
                    {
                        icon = DecideIconLocation(setting.priority),
                        codigo = setting.device_id.ToString(),
                        evento = evento,
                        lat_ref = setting.lat_origin.ToString(),
                        long_ref = setting.long_origin.ToString(),
                        lat_conf = lat.ToString(),
                        long_conf = lon.ToString(),
                        current_location = gl.GetGoogleAddress(lat, lon)
                    };
                    break;
                case 6:
                case 7:
                    return null;
                case 17:
                    return null;
                case 19:
                case 20:
                    return new
                    {
                        icon = DecideIcon(setting.priority),
                        codigo = setting.device_id.ToString(),
                        evento = evento,
                        g_value = value,
                        current_location = gl.GetGoogleAddress(lat, lon)
                    };
                default:
                    return null;
            }
        }

        public string GetMailMessageTemplate(NotificationSettings setting, MonitoringModel monit, NotificationOwner no)
        {
            GoogleLocation gl = new GoogleLocation(_configuration);
            Globals gb = new Globals();
            string templateHtml = File.ReadAllText("wwwroot/MailTemplates/Temperature.html");

            switch (setting.sensor_type_id)
            {
                case 1:
                case 2:
                    DateTime now = DateTime.Now;

                    templateHtml = templateHtml.Replace("$Sensor_Name$", GetDescriptionFromValue(setting.sensor_type_id));
                    templateHtml = templateHtml.Replace("$Device_Id$", setting.device_id.ToString().Trim());
                    templateHtml = templateHtml.Replace("$Device_Name$", setting.device_id.ToString().Trim());
                    templateHtml = templateHtml.Replace("$Owner_Mail$", no.mail);
                    templateHtml = templateHtml.Replace("$Setting_Min_Value$", setting.min_value.ToString().Trim());
                    templateHtml = templateHtml.Replace("$Setting_Max_Value$", setting.max_value.ToString().Trim());
                    templateHtml = templateHtml.Replace("$Monitoring_Value$", monit.status.temperature.ToString().Trim());

                    if (monit.pos != null)
                    {
                        templateHtml = templateHtml.Replace("$Endereco$", gl.GetGoogleAddress(monit.pos.lat, monit.pos.lon));
                    }
                    else
                    {
                        templateHtml = templateHtml.Replace("$Endereco$", "Não Identificado");
                    }
                    templateHtml = templateHtml.Replace("$Data$", now.ToString("yyyy-MM-dd"));
                    templateHtml = templateHtml.Replace("$Hora$", now.ToString("HH:mm:ss"));

                    //templateHtml = gb.TrataHtmlRegex(templateHtml);

                    break;
                case 3:
                    //Encaixe Template Geolocation
                    break;
                case 6:
                case 7:
                    break;
                case 17:
                    break;
                case 19:
                case 20:
                default:
                    break;
            }

            return templateHtml;
        }

        public string DecideIcon(string priority)
        {
            if (priority.ToUpper().Equals("URGENTE"))
            {
                return "🔥";
            }
            else if (priority.ToUpper().Equals("ATENÇÂO"))
            {
                return "⚠️";
            }
            else if (priority.ToUpper().Equals("RETORNO AO PADRÃO"))
            {
                return "✅";
            }
            else
            {
                return "";
            }
        }

        public string DecideIconLocation(string priority)
        {
            if (priority.ToUpper().Equals("GET IN"))
            {
                return "⬇️📍";
            }
            else if (priority.ToUpper().Equals("GET OUT"))
            {
                return "⬆️📍";
            }
            else if (priority.ToUpper().Equals("RETORNO AO PADRÃO"))
            {
                return "✅";
            }
            else if (priority.ToUpper().Equals("EM MOVIMENTAÇÃO"))
            {
                return "...";
            }
            else
            {
                return "";
            }
        }

        private bool SendNotification(NotificationSettings ns, NotificationOwner owner, dynamic fields, MonitoringModel monit, string sTemplateID, long seq)
        {
            try
            {
                bnZenvia zv = new bnZenvia(_configuration, _logger);
                bnNotificationLog nl = new bnNotificationLog(_configuration, _context, _logger);

                SendResponse sr = new SendResponse();

                switch (owner.notification_type_id)
                {
                    case (int)enumNotificationType.Email:
                        // Send email notification
                        sr = zv.SendEmail(owner.mail, GetMailMessageTemplate(ns, monit, owner));
                        break;
                    case (int)enumNotificationType.Sms:
                        //sr = zv.SendSms(owner.phone_number, template);
                        break;
                    case (int)enumNotificationType.WhatsApp:
                        // Send WhatsApp notification
                        sr = zv.SendWhatsApp(owner.phone_number, fields, sTemplateID);
                        break;
                }

                if (sr.Success)
                {
                    var json = JsonConvert.SerializeObject(sr);

                    return nl.InsertNotificationLog(ns, owner, seq, fields.ToString(), json.ToString(), monit.deviceId.ToString());
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
