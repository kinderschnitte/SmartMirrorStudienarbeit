using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using LightBuzz.SMTP;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;

namespace SmartMirrorServer.Notification
{
    internal static class Notification
    {
        /// <summary>
        /// Sendet Email an eingetragen E-Mail Adressen
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task SendEmail(string subject, string message)
        {
            string[] emails = {""};

            using (SmtpClient client = new SmtpClient("mail.gmx.net", 465, true, "", ""))
            {
                foreach (string t in emails)
                {
                    EmailMessage emailMessage = new EmailMessage();

                    emailMessage.To.Add(new EmailRecipient(t));
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;

                    await client.SendMailAsync(emailMessage);
                }
            }
        }

        /// <summary>
        /// Sendet eine Push Nachricht an bestimmtes Gerät
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public static void SendPushNotification(string subject, string message)
        {
            // TODO PushBullet Access Token von gespeicherten Einstellungen
            //PushbulletClient pushbulletClient = new PushbulletClient("");

            //UserDevices devices = pushbulletClient.CurrentUsersDevices();

            //Device device = devices.Devices.FirstOrDefault(o => o.Manufacturer.Contains("OnePlus"));

            //if (device == null) return;

            //PushNoteRequest request = new PushNoteRequest
            //{
            //    DeviceIden = device.Iden,
            //    Title = subject,
            //    Body = message
            //};

            //PushResponse response = pushbulletClient.PushNote(request);
        }
    }
}
