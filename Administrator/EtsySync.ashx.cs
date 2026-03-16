using Feniks.Services;
using System;
using System.Web;

namespace Feniks.Administrator
{
    public class EtsySyncHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            try
            {
                var sync = new EtsyOrderSyncService();
                sync.Sync(true);
                context.Response.Write("OK");
            }
            catch (Exception ex)
            {
                try
                {
                    var sync = new EtsyOrderSyncService();
                    sync.SaveSyncError(ex.ToString());
                }
                catch { }

                context.Response.StatusCode = 500;
                context.Response.Write(ex.Message);
            }
        }

        public bool IsReusable { get { return false; } }
    }
}