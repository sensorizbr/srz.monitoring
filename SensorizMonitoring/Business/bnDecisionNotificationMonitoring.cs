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
    public class bnDecisionNotificationMonitoring
    {

        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        Globals gb = new Globals();

        public enum NotificationType
        {
            Email = 1,
            Sms = 2,
            WhatsApp = 3
        }

        public enum SensorType
        {
            [Description("Temperatura")]
            Temperatura = 1,

            [Description("Pressão Atmosférica")]
            PressaoAtmosferica = 2,

            [Description("Geolocalização (Fora do Ponto)")]
            Geolocation = 3,

            [Description("Geolocalização (Rota)")]
            Cep = 4,

            [Description("Potência Externa")]
            PotenciaExterna = 5,

            [Description("Status Bateria")]
            StatusBateria = 6,

            [Description("Voltagem Bateria")]
            VoltagemBateria = 7,

            [Description("Nível da Luz")]
            NivelLuz = 8,

            [Description("Orientation x")]
            OrientationX = 9,

            [Description("Orientation y")]
            OrientationY = 10,

            [Description("Orientation z")]
            OrientationZ = 11,

            [Description("Vibration x")]
            VibrationX = 12,

            [Description("Vibration y")]
            VibrationY = 13,

            [Description("Vibration z")]
            VibrationZ = 14,

            [Description("Status Sinal Comunicação")]
            StatusSinalCom = 15,

            [Description("Tamper")]
            Tamper = 16,

            [Description("Movimentação")]
            Movement = 17,

            [Description("Direção")]
            Direction = 18,

            [Description("Impacto")]
            Impact = 19
        }

        public bnDecisionNotificationMonitoring(IConfiguration configuration, AppDbContext context, ILogger logger)
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

                                        if (isRecovery)
                                        {
                                            _logger.LogInformation("É recovery");
                                            DeleteNotificationControl(setting.device_id, setting.id, isRecovery);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("É notificação normal");
                                            InsertNotificationControl(setting);
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
            isRecovery = ExistsNotificationControl(setting);
            string evento = string.Empty;

            Globals gb = new Globals();

            switch (setting.sensor_type_id)
            {
                case 1: //Temperature
                    if (isRecovery)
                    {
                        ResultMath = CheckMathRecoveryDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.temperature);
                        setting.priority = "RETORNO AO PADRÃO";
                    }
                    else
                    {
                        ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.temperature);
                    }

                    monitoringValue = monit.status.temperature;
                    unitOfMeasurement = " ºC";
                    fields = GetMessageTemplate(setting, monit.status.temperature, 0, 0, unitOfMeasurement, "");
                    sTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID"];

                    break;
                case 2: //athmospheric pressure
                    if (isRecovery)
                    {
                        ResultMath = CheckMathRecoveryDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.atmosphericPressure);
                        setting.priority = "RETORNO AO PADRÃO";
                    }
                    else
                    {
                        ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.atmosphericPressure);
                    }

                    unitOfMeasurement = " hPa";
                    fields = GetMessageTemplate(setting, monit.status.atmosphericPressure, 0, 0, unitOfMeasurement, "");
                    sTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID"];
                    break;
                case 3: //lat (entrada e saída)
                    if (monit.pos is not null && (monit.status.movement.Equals("STATIONARY")))
                    {
                        if (isRecovery)
                            ResultMath = CheckMathComparation_Directions_GetInGetOut_Recovery(setting.comparation_id, setting.lat_origin, setting.long_origin, monit.pos.lat, monit.pos.lon, setting.tolerance_radius, ref evento);
                        else
                            ResultMath = CheckMathComparation_Directions_GetInGetOut(setting.comparation_id, setting.lat_origin, setting.long_origin, monit.pos.lat, monit.pos.lon, setting.tolerance_radius, ref evento);


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
                    ResultMath = CheckMathComparationBool(setting.comparation_id, setting.b_value, monit.status.externalPower);
                    monitoringValue = monit.status.externalPower;
                    break;
                case 6: //charging
                    ResultMath = CheckMathComparationBool(setting.comparation_id, setting.b_value, monit.status.charging);
                    monitoringValue = monit.status.charging;
                    break;
                case 7: //battery_voltage
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.batteryVoltage);
                    monitoringValue = monit.status.batteryVoltage;
                    break;
                case 8: //light_level
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.lightLevel);
                    monitoringValue = monit.status.lightLevel;
                    break;
                case 9: //orientation x
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.x);
                    monitoringValue = monit.status.orientation.x;
                    break;
                case 10: //orientation y
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.y);
                    monitoringValue = monit.status.orientation.y;
                    break;
                case 11: //orientation z
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.z);
                    monitoringValue = monit.status.orientation.z;
                    break;
                case 12: //vibration x
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.x);
                    monitoringValue = monit.status.vibration.x;
                    break;
                case 13: //vibration y
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.y);
                    monitoringValue = monit.status.vibration.y;
                    break;
                case 14: //vibration z
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.z);
                    monitoringValue = monit.status.vibration.z;
                    break;
                case 15: //comm_signal
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.signal);
                    monitoringValue = monit.status.signal;
                    break;
                case 16: //tamper
                    ResultMath = CheckMathComparationBool(setting.comparation_id, setting.b_value, gb.IntToBool(monit.status.tamper));
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
            SensorType sensorType = (SensorType)System.Enum.ToObject(typeof(SensorType), value);
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
        public string GetGoogleAddress(double lat, double lon) {

            GoogleLocation gl = new GoogleLocation(_configuration);
            return gl.GetAddressByCoordinators(lat, lon);
        }

        public dynamic GetMessageTemplate(NotificationSettings setting, dynamic value, double lat, double lon, string unitOfMeasurement, string evento)
        {
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
                        valor_max = setting.max_value.ToString() + unitOfMeasurement
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
                        current_location = GetGoogleAddress(lat, lon)
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
                        current_location = GetGoogleAddress(lat, lon)
                    };
                default:
                    return null;
            }
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

        public bool CheckMathComparationDouble(int comparationID, double minValue, double maxValue, double? monitoringValue)
        {

            switch (comparationID)
            {
                case 1: // Equal to
                    return minValue == monitoringValue && maxValue == monitoringValue;
                case 2: // Not equal to
                    return minValue != monitoringValue && maxValue != monitoringValue;
                case 3: // Greater than
                    return monitoringValue > maxValue;
                case 4: // Less than
                    return monitoringValue < minValue;
                case 5: // Between
                    return monitoringValue >= minValue && monitoringValue <= maxValue;
                case 6: // Outside
                    return monitoringValue < minValue || monitoringValue > maxValue;
                default:
                    return false;
            }
        }

        public bool CheckMathRecoveryDouble(int comparationID, double minValue, double maxValue, double? monitoringValue)
        {

            switch (comparationID)
            {
                case 1: // Equal to
                    return minValue != monitoringValue && maxValue != monitoringValue;
                case 2: // Not equal to
                    return minValue == monitoringValue || maxValue == monitoringValue;
                case 3: // Greater than
                    return monitoringValue <= maxValue;
                case 4: // Less than
                    return monitoringValue >= minValue;
                case 5: // Between
                    return monitoringValue < minValue || monitoringValue > maxValue;
                case 6: // Outside
                    return monitoringValue >= minValue && monitoringValue <= maxValue;
                default:
                    return false;
            }
        }

        public bool CheckMathComparationBool(int comparationID, bool bool_value, bool monitoringValue)
        {
            switch (comparationID)
            {
                case 1: // Equal to
                    return bool_value == monitoringValue;
                case 2: // Not equal to
                    return bool_value != monitoringValue;
                case 3: // Greater than
                    return false;
                case 4: // Less than
                    return false;
                case 5: // Between
                    return false;
                case 6: // Outside
                    return false;
                default:
                    return false;
            }
        }

        public bool CheckMathComparation_Directions_GetInGetOut(int comparationID, double latDevice, double longDevice, double latSettings, double longSettings, int? radius, ref string evento)
        {
            double latDevicePrecision = gb.FormatValuePrecision(latDevice);
            double longDevicePrecision = gb.FormatValuePrecision(longDevice);
            double latSettingsPrecision = gb.FormatValuePrecision(latSettings);
            double longSettingsPrecision = gb.FormatValuePrecision(longSettings);
            double radiusPrecision = gb.FormatValuePrecision(radius ?? 0);

            double distance = gb.CalculateDistance(latDevicePrecision, longDevicePrecision, latSettingsPrecision, longSettingsPrecision);

            switch (comparationID)
            {
                case 1: // Get In To
                    evento = "ENTRADA";
                    return distance <= radius;
                case 2: // Out To
                    evento = "SAIDA";
                    return distance > radius;
                default:
                    return false;
            }
        }



        public bool CheckMathComparation_Directions_GetInGetOut_Recovery(int comparationID, double latDevice, double longDevice, double latSettings, double longSettings, int? radius, ref string evento)
        {
            double latDevicePrecision = gb.FormatValuePrecision(latDevice);
            double longDevicePrecision = gb.FormatValuePrecision(longDevice);
            double latSettingsPrecision = gb.FormatValuePrecision(latSettings);
            double longSettingsPrecision = gb.FormatValuePrecision(longSettings);
            double radiusPrecision = gb.FormatValuePrecision(radius ?? 0);

            double distance = gb.CalculateDistance(latDevicePrecision, longDevicePrecision, latSettingsPrecision, longSettingsPrecision);

            switch (comparationID)
            {
                case 1: // Get In To
                    evento = "SAIDA";
                    return distance >= radius;
                case 2: // Out To
                    evento = "ENTRADA";
                    return distance < radius;
                default:
                    return false;
            }
        }

        public bool CheckMathComparation_Router(int comparationID, double latDevice, double longDevice, double latSettings, double longSettings)
        {
            switch (comparationID)
            {
                case 1: // Get In To
                    return latDevice == latSettings && longDevice == longSettings;
                case 2: // Out To
                    return latDevice != latSettings && longDevice != longSettings;
                default:
                    return false;
            }
        }

        private bool SendNotification(NotificationSettings ns, NotificationOwner owner, dynamic fields, MonitoringModel monit, string sTemplateID, long seq)
        {
            try
            {
                bnZenvia zv = new bnZenvia(_configuration, _logger);

                SendResponse sr = new SendResponse();

                switch (owner.notification_type_id)
                {
                    case (int)NotificationType.Email:
                        // Send email notification
                        break;
                    case (int)NotificationType.Sms:
                        //sr = zv.SendSms(owner.phone_number, template);
                        break;
                    case (int)NotificationType.WhatsApp:
                        // Send WhatsApp notification
                        sr = zv.SendWhatsApp(owner.phone_number, fields, sTemplateID);
                        break;
                }

                if (sr.Success)
                {
                    var json = JsonConvert.SerializeObject(sr);

                    return InsertNotificationLog(ns, owner, seq, json.ToString(), monit.deviceId.ToString());
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                return false;
            }
        }

        public List<NotificationControl> GetNotificationControl(int deviceID, int notificationID)
        {
            try
            {
                if (deviceID == 0) throw new ArgumentNullException(nameof(deviceID));

                return _context.NotificationControl
                  .Where(d => d.device_id == deviceID && d.notification_id == notificationID)
                  .AsNoTracking()
                  .ToList();

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                return null;
            }
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
        public bool InsertNotificationLog(NotificationSettings st,
                                             NotificationOwner on,
                                             long seq,
                                             string sMessage,
                                             string sDeviceDescription)
        {


            try
            {
                var insertNotificationLog = new NotificationLog();

                insertNotificationLog.device_id = st.device_id;
                insertNotificationLog.seq = seq;
                insertNotificationLog.description = sDeviceDescription;
                insertNotificationLog.sensor_type_id = st.sensor_type_id;
                insertNotificationLog.comparation_id = st.comparation_id;
                insertNotificationLog.phone_number = on.phone_number;
                insertNotificationLog.mail = on.mail;
                insertNotificationLog.message = sMessage;
                insertNotificationLog.reference_value = "";
                insertNotificationLog.monitoring_value = "";
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
