using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace Feniks.Services
{
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly string _chatId;

        public TelegramService()
        {
            _botToken = (ConfigurationManager.AppSettings["TelegramBotToken"] ?? "").Trim();
            _chatId = (ConfigurationManager.AppSettings["TelegramChatId"] ?? "").Trim();
        }

        public bool IsConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_botToken)
                    && !string.IsNullOrWhiteSpace(_chatId);
            }
        }

        public void SendMessage(string message)
        {
            if (!IsConfigured)
                return;

            if (string.IsNullOrWhiteSpace(message))
                return;

            string url = "https://api.telegram.org/bot" + _botToken + "/sendMessage";

            string json =
                "{"
                + "\"chat_id\":\"" + EscapeJson(_chatId) + "\","
                + "\"text\":\"" + EscapeJson(message) + "\","
                + "\"parse_mode\":\"HTML\""
                + "}";

            byte[] data = Encoding.UTF8.GetBytes(json);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Timeout = 30000;
            request.ReadWriteTimeout = 30000;
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                string responseText = sr.ReadToEnd();

                if (string.IsNullOrWhiteSpace(responseText))
                    return;

                JObject obj = JObject.Parse(responseText);
                bool ok = obj["ok"] != null && obj["ok"].ToObject<bool>();

                if (!ok)
                    throw new Exception("Telegram send failed. Response=" + responseText);
            }
        }

        private static string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }
    }
}