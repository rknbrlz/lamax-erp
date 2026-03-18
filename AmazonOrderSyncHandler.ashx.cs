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

            string key = (context.Request["key"] ?? string.Empty).Trim();
            string expected = (ConfigurationManager.AppSettings["AmazonSyncKey"] ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(expected) || !string.Equals(key, expected, StringComparison.Ordinal))
            {
                context.Response.StatusCode = 403;
                context.Response.Write("Forbidden");
                return;
            }

            try
            {
                AmazonOrderSyncService svc = new AmazonOrderSyncService();
                SyncRunResult result = svc.SyncInboxOnly(true);

                if (result.WasSkippedBecauseLocked)
                {
                    context.Response.StatusCode = 202;
                    context.Response.Write("SKIPPED_LOCKED");
                    return;
                }

                context.Response.StatusCode = 200;
                context.Response.Write(
                    "OK | Orders=" + result.OrderCount +
                    " | NewOrders=" + result.NewOrderCount +
                    " | Items=" + result.ItemCount
                );
            }
            catch (Exception ex)
            {
                try
                {
                    new AmazonOrderSyncService().SaveSyncError(ex.ToString());
                }
                catch
                {
                }

                context.Response.StatusCode = 500;
                context.Response.Write(ex.Message);
            }
        }
    }
}