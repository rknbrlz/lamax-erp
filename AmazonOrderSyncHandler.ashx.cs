using Feniks.Services;
using System;
using System.Configuration;
using System.Web;

namespace Feniks
{
    public class AmazonOrderSyncHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string key = (context.Request["key"] ?? "").Trim();
            string expected = (ConfigurationManager.AppSettings["AmazonSyncKey"] ?? "").Trim();

            if (string.IsNullOrWhiteSpace(expected) || key != expected)
            {
                context.Response.StatusCode = 403;
                context.Response.Write("Forbidden");
                return;
            }

            try
            {
                AmazonOrderSyncService svc = new AmazonOrderSyncService();
                svc.Sync(true, true);

                context.Response.Write("OK");
            }
            catch (Exception ex)
            {
                try
                {
                    new AmazonOrderSyncService().SaveSyncError(ex.ToString());
                }
                catch { }

                context.Response.StatusCode = 500;
                context.Response.Write(ex.Message);
            }
        }
    }
}