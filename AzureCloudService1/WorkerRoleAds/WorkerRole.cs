using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using WorkerRoleAds.Core;

namespace WorkerRoleAds
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        //public override void Run()
        //{
        //    Trace.TraceInformation("WorkerRoleAds is running");

        //    try
        //    {
        //        this.RunAsync(this.cancellationTokenSource.Token).Wait();
        //    }
        //    finally
        //    {
        //        this.runCompleteEvent.Set();
        //    }
        //}

        //public override bool OnStart()
        //{
        //    // Set the maximum number of concurrent connections
        //    ServicePointManager.DefaultConnectionLimit = 12;

        //    // For information on handling configuration changes
        //    // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

        //    bool result = base.OnStart();

        //    Trace.TraceInformation("WorkerRoleAds has been started");

        //    return result;
        //}

        public override void Run()
        {
            while (true)
            {
                try
                {
                    Trace.WriteLine("Email Working", "Information");
                    Thread.Sleep(ConfigSettings.EmailQueueInterval);

                    EmailManager.SendQueuedEmail();
                }
                catch (Exception e)
                {
                    Trace.TraceError("Email Error: " + e.Message);
                }
                finally
                {
                    try
                    {
                        Thread.ResetAbort();
                    }
                    catch { }
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
       
       


    }
}
