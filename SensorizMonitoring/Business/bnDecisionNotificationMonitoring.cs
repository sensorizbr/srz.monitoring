using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using System;
using System.Collections.Generic;
using Twilio.AspNet.Common;
using Zenvia.SMS;

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

                bnNotificationSettings ns = new bnNotificationSettings(_configuration, _context);
                List<NotificationSettings> lstSettings = ns.GetNotificationSettingsForDevice(monit.deviceId);

                bnNotificationOwner no = new bnNotificationOwner(_configuration, _context);
                List<NotificationOwner> lstOwners = no.GetNotificationOwnersForDevice(monit.deviceId);

                foreach (var setting in lstSettings)
                {
                    if (ShouldSendNotification(setting, monit))
                    {
                        foreach (var owner in lstOwners)
                        {
                             SendNotification(owner, setting);
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

        private bool ShouldSendNotification(NotificationSettings setting, MonitoringModel monit)
        {
            Globals gb = new Globals();
            // Get the sensor type ID and comparison ID from the setting
            int sensorTypeId = setting.sensor_type_id;
            int comparisonId = setting.comparation_id;

            // Get the start, end, and exact values from the setting
            double startValue = gb.ToDouble(setting.start_value);
            double endValue = gb.ToDouble(setting.end_value);
            double exactValue = gb.ToDouble(setting.exact_value);

            // Get the interval flag from the setting
            int intervalFlag = setting.interval_flag;

            // Cast monit.Value to decimal
            double currentValue = 0;
            switch (setting.sensor_type_id)
            {
                case 1: //Temperature
                    currentValue = monit.status.temperature;
                    break;
                case 2: //athmospheric pressure
                    currentValue = monit.status.atmosphericPressure;
                    break;
                case 3: //lat
                        //monit.pos.lat.ToString());
                    break;
                case 4: //lon

                    //monit.pos.lon.ToString());
                    break;
                case 5: //cep

                    //monit.pos.cep.ToString());
                    break;
                case 6: //external power

                    //monit.status.externalPower.ToString());
                    break;
                case 7: //charging

                    //monit.status.charging.ToString());
                    break;
                case 8: //battery_voltage

                    //monit.status.batteryVoltage.ToString());
                    break;
                case 9: //light_level

                    //monit.status.lightLevel.ToString());
                    break;
                case 10: //orientation x

                    // monit.status.orientation.x.ToString());
                    break;
                case 11: //orientation y

                    // monit.status.orientation.y.ToString());
                    break;
                case 12: //orientation z

                    //monit.status.orientation.z.ToString());
                    break;
                case 13: //vibration x
                         // monit.status.vibration.x.ToString());
                    break;
                case 14: //vibration y
                         //monit.status.vibration.y.ToString());
                    break;
                case 15: //vibration z
                         //monit.status.vibration.z.ToString());
                    break;
                case 16: //comm_signal
                         //monit.status.signal.ToString());
                    break;
                case 17: //tamper
                         // monit.status.tamper.ToString());
                    break;
                case 18: //movement
                         //monit.status.movement.ToString());
                    break;

                    break;
            }

            // Decide whether to send a notification based on the comparison type
            switch (comparisonId)
            {
                case 1: // Equal to
                    return currentValue == exactValue;
                case 2: // Not equal to
                    return currentValue != exactValue;
                case 3: // Greater than
                    return currentValue > exactValue;
                case 4: // Less than
                    return currentValue < exactValue;
                case 5: // Between
                    return currentValue >= startValue && currentValue <= endValue;
                case 6: // Outside
                    return currentValue < startValue || currentValue > endValue;
                default:
                    throw new ArgumentException("Invalid comparison ID", nameof(comparisonId));
            }
        }

        private async Task SendNotification(NotificationOwner owner, NotificationSettings setting)
        {
            switch (owner.notification_type_id)
            {
                case (int)NotificationType.Email:
                    // Send email notification
                    break;
                case (int)NotificationType.Sms:
                    await SendSmsNotification(owner, setting);
                    break;
                case (int)NotificationType.WhatsApp:
                    // Send WhatsApp notification
                    break;
            }
        }

        private async Task SendSmsNotification(NotificationOwner owner, NotificationSettings setting)
        {
            //bnZenvia zv = new bnZenvia();

            //var json = new JObject
            //    {
            //        { "from", "sms-account" },
            //        { "to", owner.phone_number},
            //        { "contents", new JArray(
            //            new JObject
            //            {
            //                { "type", "text" },
            //                { "text", "Hi Zenvia!" }
            //            }
            //        )}
            //    };

            //await zv.PostApiDataAsync("https://api.zenvia.com/v2/channels/sms/messages", json);
        }


        public bool DecideComparation(int comparationOperator,
                               int intervalOperatorFlag,
                               string startValue,
                               string endValue,
                               string exactValue,
                               string referenceValue)
        {
            if (comparationOperator == 1)
            {
                if (intervalOperatorFlag == 1) // Is Interval
                {
                    return false;
                }
                else
                { // Is not Interval 
                    return double.Parse(referenceValue) < double.Parse(exactValue);
                }
            }
            else if (comparationOperator == 2)
            {
                if (intervalOperatorFlag == 1) // Is Interval
                {
                    return false;
                }
                else
                { // Is not Interval 
                    return referenceValue == exactValue;
                }
            }
            else if (comparationOperator == 3)
            {
                if (intervalOperatorFlag == 1) // Is Interval
                {
                    return false;
                }
                else
                { // Is not Interval 
                    return double.Parse(referenceValue) > double.Parse(exactValue);
                }
            }
            else if (comparationOperator == 4)
            {
                if (intervalOperatorFlag == 1) // Is Interval
                {
                    return double.Parse(referenceValue) < double.Parse(exactValue) && double.Parse(referenceValue) > double.Parse(exactValue);
                }
                else
                { // Is not Interval 
                    return false;
                }
            }
            else //5
            {
                if (intervalOperatorFlag == 1) // Is Interval
                {
                    return false;
                }
                else
                { // Is not Interval 
                    return double.Parse(referenceValue) != double.Parse(exactValue);
                }
            }
        }
    }
}
