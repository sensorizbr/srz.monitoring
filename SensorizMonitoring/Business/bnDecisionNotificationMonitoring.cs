using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using ZenviaApi;
using static SensorizMonitoring.Models.ZenviaResposeModel;

namespace SensorizMonitoring.Business
{
    public class bnDecisionNotificationMonitoring
    {

        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public enum NotificationType
        {
            Email = 1,
            Sms = 2,
            WhatsApp = 3
        }

        public bnDecisionNotificationMonitoring(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        public bool sendNotification(MonitoringModel monit)
        {
            try
            {
                GetNotificationSettings(monit);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetNotificationSettings(MonitoringModel monit)
        {
            try
            {
                int ComparationOperator = 0;
                int IntervalOperatorFlag = 0;
                string sMessage = string.Empty;
                string sSensor = string.Empty;

                bnNotificationSettings ns = new bnNotificationSettings(_configuration, _context);
                List<NotificationSettings> lstSettings = ns.GetNotificationSettingsForDevice(monit.deviceId);

                bnNotificationOwner no = new bnNotificationOwner(_configuration, _context);
                List<NotificationOwner> lstOwners = no.GetNotificationOwnersForDevice(monit.deviceId);

                foreach (var setting in lstSettings)
                {
                    if (ShouldSendNotification(setting, monit, ref sMessage, ref sSensor))
                    {
                        foreach (var owner in lstOwners)
                        {
                            string sFullMassage = TrataMensagem(owner, sSensor, sMessage);
                            if (SendNotification(setting, owner, sFullMassage, monit))
                            {

                            }
                        }
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool ShouldSendNotification(NotificationSettings setting, MonitoringModel monit, ref string sMessage, ref string sSensor)
        {
            sMessage = "";
            sSensor = "";

            bool ResultMath = false;
            dynamic monitoringValue = null;

            Globals gb = new Globals();

            switch (setting.sensor_type_id)
            {
                case 1: //Temperature
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.temperature);
                    monitoringValue = monit.status.temperature;
                    sSensor = "Temperatura";
                    break;
                case 2: //athmospheric pressure
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.atmosphericPressure);
                    monitoringValue = monit.status.atmosphericPressure;
                    sSensor = "Pressão Atmosférica";
                    break;
                case 3: //lat
                        //monit.pos.lat.ToString());
                    ResultMath =  CheckMathComparation_Directions_GetInGetOut(setting.comparation_id, setting.lat_origin, setting.long_origin, monit.pos.lat, monit.pos.lon);
                    //monitoringValue = monit.pos.lat;
                    sSensor = "Geolocation (In/Out)";
                    break;
                case 4: //lon
                    //monit.pos.lon.ToString());
                    break;
                case 5: //cep
                    //currentValueDouble = monit.pos.cep;
                    sSensor = "CEP";
                    break;
                case 6: //external power
                    ResultMath = CheckMathComparationBool(setting.comparation_id, setting.b_value, monit.status.externalPower);
                    monitoringValue = monit.status.externalPower;
                    sSensor = "Potência Externa";
                    break;
                case 7: //charging
                    ResultMath = CheckMathComparationBool(setting.comparation_id, setting.b_value, monit.status.charging);
                    monitoringValue = monit.status.charging;
                    sSensor = "Status da Carga (Bateria)";
                    break;
                case 8: //battery_voltage
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.batteryVoltage);
                    monitoringValue = monit.status.batteryVoltage;
                    sSensor = "Voltagem da Bateria";
                    break;
                case 9: //light_level
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.lightLevel);
                    monitoringValue = monit.status.lightLevel;
                    sSensor = "Nível da Luz";
                    break;
                case 10: //orientation x
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.x);
                    monitoringValue = monit.status.orientation.x;
                    sSensor = "Orientação x";
                    break;
                case 11: //orientation y
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.y);
                    monitoringValue = monit.status.orientation.y;
                    sSensor = "Orientação y";
                    break;
                case 12: //orientation z
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.orientation.z);
                    monitoringValue = monit.status.orientation.z;
                    sSensor = "Orientação z";
                    break;
                case 13: //vibration x
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.x);
                    monitoringValue = monit.status.vibration.x;
                    sSensor = "Vibração x";
                    break;
                case 14: //vibration y
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.y);
                    monitoringValue = monit.status.vibration.y;
                    sSensor = "Vibração y";
                    break;
                case 15: //vibration z
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.vibration.z);
                    monitoringValue = monit.status.vibration.z;
                    sSensor = "Vibração z";
                    break;
                case 16: //comm_signal
                    ResultMath = CheckMathComparationDouble(setting.comparation_id, setting.min_value, setting.max_value, monit.status.signal);
                    monitoringValue = monit.status.signal;
                    sSensor = "Status do Sinal de Comunicação";
                    break;
                case 17: //tamper
                    ResultMath = CheckMathComparationBool(setting.comparation_id, setting.b_value, gb.IntToBool(monit.status.tamper));
                    monitoringValue = monit.status.tamper;
                    sSensor = "Tamper";
                    break;
                case 18: //movement
                    ResultMath = false;
                    break;
                case 19: //Directions
                    ResultMath = false;
                    break;
                default:
                    return false;
            }

            sMessage = GetMessageTemplate(setting, monitoringValue);

            return ResultMath;
        }

        public string GetMessageTemplate(NotificationSettings setting, dynamic value)
        {

            string sMessage = string.Empty;
            Globals gb = new Globals();

            switch (setting.sensor_type_id)
            {
                case 1:
                case 2:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                    sMessage = "Valor Identificado: " + value
                        + "\n" + "Valor de Referência (Min): " + setting.min_value.ToString()
                        + "\n" + "Valor de Referência (Max): " + setting.max_value.ToString();
                    break;
                case 6:
                case 7:
                    sMessage = "Valor Identificado: " + gb.TrataBool(Convert.ToBoolean(value))
                        + "\n" + "Valor de Referência: " + gb.TrataBool(setting.b_value);
                    break;
                case 17:
                    sMessage = "Tamper: " + gb.TrataTamper(Convert.ToInt32(value))
                        + "\n" + "Valor de Referência: " + gb.TrataTamper(Convert.ToInt32(setting.min_value));
                    break;
                default:
                    return "";
            }

            return sMessage;
        }

        public bool CheckMathComparationDouble(int comparationID, double minValue, double maxValue, double monitoringValue)
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
                    return monitoringValue < minValue && monitoringValue > maxValue;
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

        public bool CheckMathComparation_Directions_GetInGetOut(int comparationID, double latDevice, double longDevice, double latSettings, double longSettings)
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

        private bool SendNotification(NotificationSettings ns, NotificationOwner owner, string sMessage, MonitoringModel monit)
        {
            try
            {
                bnZenvia zv = new bnZenvia(_configuration);

                SendResponse sr = new SendResponse();

                switch (owner.notification_type_id)
                {
                    case (int)NotificationType.Email:
                        // Send email notification
                        break;
                    case (int)NotificationType.Sms:
                        sr = zv.SendSms(owner.phone_number, sMessage);
                        break;
                    case (int)NotificationType.WhatsApp:
                        // Send WhatsApp notification
                        sr = zv.SendWhatsApp(owner.phone_number, sMessage);
                        break;
                }

                if (sr.Success)
                {
                    return InsertNotificationLog(ns, owner, sMessage, monit.deviceId.ToString());
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string TrataMensagem(NotificationOwner owner, string Sensor, string sMessage)
        {
            try
            {
                string sFullMessage = string.Empty;
                switch (owner.notification_type_id)
                {
                    case (int)NotificationType.Email:
                        // Send email notification
                        break;
                    case (int)NotificationType.Sms:
                        sFullMessage = ":::Alerta Sensoriz:::\n";
                        sFullMessage += "Sensor Configurado e Identificado: " + Sensor + "\n";
                        sFullMessage += "---------------\n";
                        sFullMessage += sMessage;
                        break;
                    case (int)NotificationType.WhatsApp:
                        sFullMessage = ":::Alerta Sensoriz:::\n";
                        sFullMessage += "Sensor Configurado e Identificado: " + Sensor + "\n";
                        sFullMessage += "---------------\n";
                        sFullMessage += sMessage;
                        break;
                }
                return sFullMessage;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public bool InsertNotificationLog(NotificationSettings st,
                                         NotificationOwner on,
                                         string sMessage,
                                         string sDeviceDescription)
        {


            try
            {
                var insertNotificationLog = new NotificationLog();

                insertNotificationLog.device_id = st.device_id;
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
                return false;
            }
        }
    }

}
