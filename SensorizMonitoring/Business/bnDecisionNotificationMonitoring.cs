using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using System;
using System.Collections.Generic;
using Twilio.AspNet.Common;
using Zenvia.SMS;
using ZenviaApi;

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
                            if (SendNotification(owner, sFullMassage)) { 
                            
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
            Globals gb = new Globals();
            // Get the sensor type ID and comparison ID from the setting
            int sensorTypeId = setting.sensor_type_id;
            int comparationId = setting.comparation_id;

            // Get the start, end, and exact values from the setting
            double startValue = gb.ToDouble(setting.start_value);
            double endValue = gb.ToDouble(setting.end_value);
            double exactValue = gb.ToDouble(setting.exact_value);

            // Cast monit.Value to decimal
            double currentValueDouble = 0;
            bool currentValueBool = false;
            string currentValueString = "";
            sMessage = "";
            sSensor = "";

            switch (setting.sensor_type_id)
            {
                case 1: //Temperature
                    currentValueDouble = monit.status.temperature;
                    sSensor = "Temperatura";
                    break;
                case 2: //athmospheric pressure
                    currentValueDouble = monit.status.atmosphericPressure;
                    sSensor = "Pressão Atmosférica";
                    break;
                case 3: //lat
                    //monit.pos.lat.ToString());
                    break;
                case 4: //lon
                    //monit.pos.lon.ToString());
                    break;
                case 5: //cep
                    currentValueDouble = monit.pos.cep;
                    sSensor = "CEP";
                    break;
                case 6: //external power
                    currentValueBool = monit.status.externalPower;
                    sSensor = "Potência Externa";
                    break;
                case 7: //charging
                    currentValueBool = monit.status.charging;
                    sSensor = "Status da Carga (Bateria)";
                    break;
                case 8: //battery_voltage
                    currentValueDouble = monit.status.batteryVoltage;
                    sSensor = "Voltagem da Bateria";
                    break;
                case 9: //light_level
                    currentValueDouble = monit.status.lightLevel;
                    sSensor = "Nível da Luz";
                    break;
                case 10: //orientation x
                    currentValueDouble = monit.status.orientation.x;
                    sSensor = "Orientação x";
                    break;
                case 11: //orientation y
                    currentValueDouble = monit.status.orientation.y;
                    sSensor = "Orientação y";
                    break;
                case 12: //orientation z
                    currentValueDouble = monit.status.orientation.z;
                    sSensor = "Orientação z";
                    break;
                case 13: //vibration x
                    currentValueDouble = monit.status.vibration.x;
                    sSensor = "Vibração x";
                    break;
                case 14: //vibration y
                    currentValueDouble = monit.status.vibration.y;
                    sSensor = "Vibração y";
                    break;
                case 15: //vibration z
                    currentValueDouble = monit.status.vibration.z;
                    sSensor = "Vibração z";
                    break;
                case 16: //comm_signal
                    currentValueDouble = monit.status.signal;
                    sSensor = "Status do Sinal de Comunicação";
                    break;
                case 17: //tamper
                    currentValueDouble = monit.status.tamper;
                    sSensor = "Tamper";
                    break;
                case 18: //movement
                    currentValueString = monit.status.movement;
                    sSensor = "Movimento Atual";
                    break;
                default:
                    return false;
            }

            sMessage = "Valor Identificado: " + monit.status.temperature.ToString() + "\n" + "Valor de Referência: " + setting.exact_value.ToString();

            // Decide whether to send a notification based on the comparison type
            if (currentValueDouble > 0)
            {
                switch (comparationId)
                {
                    case 1: // Equal to
                        return currentValueDouble == exactValue;
                    case 2: // Not equal to
                        return currentValueDouble != exactValue;
                    case 3: // Greater than
                        return currentValueDouble > exactValue;
                    case 4: // Less than
                        return currentValueDouble < exactValue;
                    case 5: // Between
                        return currentValueDouble >= startValue && currentValueDouble <= endValue;
                    case 6: // Outside
                        return currentValueDouble < startValue && currentValueDouble > endValue;
                    default:
                        return false;
                }
            }
            else if (currentValueBool)
            {
                switch (comparationId)
                {
                    case 1: // Equal to
                        return currentValueBool == Convert.ToBoolean(exactValue);
                    case 2: // Not equal to
                        return currentValueBool != Convert.ToBoolean(exactValue);
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
            else if (!String.IsNullOrEmpty(currentValueString))
            {
                switch (comparationId)
                {
                    case 1: // Equal to
                        return currentValueString == currentValueString;
                    case 2: // Not equal to
                        return currentValueString == currentValueString;
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
            else
            {
                return false;
            }
        }

        private bool SendNotification(NotificationOwner owner, string sMessage)
        {
            try
            {
                bnZenvia zv = new bnZenvia();
                switch (owner.notification_type_id)
                {
                    case (int)NotificationType.Email:
                        // Send email notification
                        break;
                    case (int)NotificationType.Sms:
                        zv.SendSmsAsync(owner.phone_number, sMessage);
                        break;
                    case (int)NotificationType.WhatsApp:
                        // Send WhatsApp notification
                        zv.SendWhatsAppAsync(owner.phone_number, sMessage);
                        break;
                }
                return true;
            }
            catch(Exception ex) {
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
                        sFullMessage += "Sensor Configurado e Identificado: "+ Sensor + "\n";
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
                                         string sDeviceDescription,
                                         string sReferenceValue,
                                         string sMonitoringValue)
        {


            try
            {
                var insertNotificationLog = new NotificationLog();

                insertNotificationLog.device_id = st.device_id;
                insertNotificationLog.description_device = sDeviceDescription;
                insertNotificationLog.sensor_type_id = st.sensor_type_id;
                insertNotificationLog.comparation_id = st.comparation_id;
                insertNotificationLog.phone_number = on.phone_number;
                insertNotificationLog.mail = on.mail;
                insertNotificationLog.message = sMessage;
                insertNotificationLog.reference_value = sReferenceValue;
                insertNotificationLog.monitoring_value = sMonitoringValue;
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
