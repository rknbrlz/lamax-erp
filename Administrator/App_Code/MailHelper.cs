using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Feniks.Helpers
{
    public static class MailHelper
    {
        public static void SendMail(string to, string subject, string body, bool isHtml = true)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient e-mail is required.", nameof(to));

            string smtpHost = GetRequiredAppSetting("SmtpHost");
            int smtpPort = GetIntAppSetting("SmtpPort", 587);
            string smtpUser = GetRequiredAppSetting("SmtpUser");
            string smtpPassword = GetRequiredAppSetting("SmtpPassword");
            bool smtpEnableSsl = GetBoolAppSetting("SmtpEnableSsl", true);

            string mailFrom = GetRequiredAppSetting("MailFrom");
            string mailFromDisplayName = ConfigurationManager.AppSettings["MailFromDisplayName"] ?? "LamaX";

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(mailFrom, mailFromDisplayName);
                message.To.Add(new MailAddress(to));
                message.Subject = subject ?? string.Empty;
                message.Body = body ?? string.Empty;
                message.IsBodyHtml = isHtml;

                using (SmtpClient client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = smtpEnableSsl;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 30000;

                    client.Send(message);
                }
            }
        }

        public static void SendMail(string to, string cc, string bcc, string subject, string body, bool isHtml = true)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient e-mail is required.", nameof(to));

            string smtpHost = GetRequiredAppSetting("SmtpHost");
            int smtpPort = GetIntAppSetting("SmtpPort", 587);
            string smtpUser = GetRequiredAppSetting("SmtpUser");
            string smtpPassword = GetRequiredAppSetting("SmtpPassword");
            bool smtpEnableSsl = GetBoolAppSetting("SmtpEnableSsl", true);

            string mailFrom = GetRequiredAppSetting("MailFrom");
            string mailFromDisplayName = ConfigurationManager.AppSettings["MailFromDisplayName"] ?? "LamaX";

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(mailFrom, mailFromDisplayName);
                message.To.Add(new MailAddress(to));

                if (!string.IsNullOrWhiteSpace(cc))
                    message.CC.Add(new MailAddress(cc));

                if (!string.IsNullOrWhiteSpace(bcc))
                    message.Bcc.Add(new MailAddress(bcc));

                message.Subject = subject ?? string.Empty;
                message.Body = body ?? string.Empty;
                message.IsBodyHtml = isHtml;

                using (SmtpClient client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = smtpEnableSsl;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 30000;

                    client.Send(message);
                }
            }
        }

        private static string GetRequiredAppSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new ConfigurationErrorsException("Missing appSetting: " + key);

            return value;
        }

        private static int GetIntAppSetting(string key, int defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        private static bool GetBoolAppSetting(string key, bool defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }
    }
}