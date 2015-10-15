using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;


namespace WorkerRoleAds.Core
{
    public class ConfigSettings
    {
        public static readonly int EmailQueueInterval = Int32.Parse(ConfigurationManager.AppSettings["Email.QueueInterval"]);
        public static readonly bool EmailEnableSsl = bool.Parse(ConfigurationManager.AppSettings["Email.EnableSsl"]);

        public static readonly bool EmailUseDefaultCredentials = bool.Parse(ConfigurationManager.AppSettings["Email.UseDefaultCredentials"]);
        public static readonly string EmailMailTo = ConfigurationManager.AppSettings["Email.MailTo"];
        public static readonly string EmailAddress = ConfigurationManager.AppSettings["Email.Address"];
        public static readonly string EmailDisplayName = ConfigurationManager.AppSettings["Email.DisplayName"];
        public static readonly string EmailHost = ConfigurationManager.AppSettings["Email.Host"];
        public static readonly string EmailUser = ConfigurationManager.AppSettings["Email.User"];
        public static readonly string EmailPassword = ConfigurationManager.AppSettings["Email.Password"];
        public static readonly int EmailPort = int.Parse(ConfigurationManager.AppSettings["Email.Port"]);
        public static readonly int EmailNumberOfQueueMessage = int.Parse(ConfigurationManager.AppSettings["Email.NumberOfQueueMessage"]);
    }
}
