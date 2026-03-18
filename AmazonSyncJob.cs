using Feniks.Services;
using System;
using System.Configuration;
using System.IO;

namespace Feniks.AmazonSyncJob
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                Log("AmazonSyncJob started.");

                AmazonOrderSyncService svc = new AmazonOrderSyncService();
                svc.SyncInboxOnly(true);

                Log("AmazonSyncJob completed successfully.");
                return 0;
            }
            catch (Exception ex)
            {
                Log("AmazonSyncJob failed: " + ex);
                return 1;
            }
        }

        private static void Log(string message)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string logFile = Path.Combine(baseDir, "AmazonSyncJob.log");

                File.AppendAllText(
                    logFile,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + message + Environment.NewLine
                );
            }
            catch
            {
                // log yazamazsa job düşmesin
            }
        }
    }
}