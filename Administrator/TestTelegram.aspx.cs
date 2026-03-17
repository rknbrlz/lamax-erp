using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Feniks.Administrator
{
    public partial class TestTelegram : System.Web.UI.Page
    {
        protected void btnSend_Click(object sender, EventArgs e)
        {
            string responseText;
            string errorText;

            bool ok = SendTelegramMessage(
                "LamaX test message",
                out responseText,
                out errorText
            );

            if (ok)
            {
                lblResult.Text = "Telegram message sent successfully.<br/><br/>Response: "
                                 + Server.HtmlEncode(responseText);
            }
            else
            {
                lblResult.Text = "Telegram message failed.<br/><br/>Error: "
                                 + Server.HtmlEncode(errorText)
                                 + "<br/><br/>Response: "
                                 + Server.HtmlEncode(responseText);
            }
        }

        private bool SendTelegramMessage(string message, out string responseText, out string errorText)
        {
            responseText = string.Empty;
            errorText = string.Empty;

            try
            {
                string botToken = ConfigurationManager.AppSettings["TelegramBotToken"];
                string chatId = ConfigurationManager.AppSettings["TelegramChatId"];

                if (string.IsNullOrWhiteSpace(botToken))
                {
                    errorText = "TelegramBotToken is empty in web.config";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(chatId))
                {
                    errorText = "TelegramChatId is empty in web.config";
                    return false;
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string url = "https://api.telegram.org/bot" + botToken + "/sendMessage";

                string postData =
                    "chat_id=" + HttpUtility.UrlEncode(chatId) +
                    "&text=" + HttpUtility.UrlEncode(message);

                byte[] data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.ContentLength = data.Length;
                request.Timeout = 20000;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    return responseText.Contains("\"ok\":true");
                }
            }
            catch (WebException ex)
            {
                errorText = ex.Message;

                if (ex.Response != null)
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        errorText += " | " + reader.ReadToEnd();
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                errorText = ex.ToString();
                return false;
            }
        }
    }
}