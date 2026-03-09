using System;
using System.IO;
using System.Web;
using System.Diagnostics;

namespace Feniks
{
    public static class Logger
    {
        public static void Log(string message)
        {
            try
            {
                // ROOT'a yaz: ~/login_debug.log
                string file = HttpContext.Current.Server.MapPath("~/login_debug.log");
                File.AppendAllText(file,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " | " + message + Environment.NewLine);
            }
            catch { }

            // Ek garanti: Trace (Output penceresinde görünebilir)
            try { Trace.WriteLine(message); } catch { }
        }
    }
}