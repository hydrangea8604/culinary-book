using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace WorkerRoleAds.Core
{
    public class EmailManager
    {
        static EmailQueueClient GetEmailQueueClient()
        {
            return new EmailQueueClient(CloudStorageAccount.DevelopmentStorageAccount.QueueEndpoint.AbsoluteUri, CloudStorageAccount.DevelopmentStorageAccount.Credentials, "email-queue");
        }

        static BaseServiceContext<EmailMessageEntity> GetEmailMessageContext()
        {
            return new BaseServiceContext<EmailMessageEntity>(CloudStorageAccount.DevelopmentStorageAccount.TableEndpoint.AbsoluteUri, CloudStorageAccount.DevelopmentStorageAccount.Credentials);
        }

        #region Email Template

        private const string EmailTemplate = @"
                        <!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>
                        <html>
	                        <head>
		                        <title></title>
                                <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
                                <meta http-equiv='Content-Language' content='en-us' />
                                <style type='text/css' media='screen'>
                                    table { font-size:14px; font-family: arial; color:#434244;}
                                    a {text-decoration:none;}
                                    a {color:#006C40;}
                                    a:hover { color:#006C40;text-decoration: underline;}
                                </style>
	                        </head>
	                        <body>	    
                                <table>        
                                <tr><td></td>
       
                                <td>
                                <#CONTENT#>
                                <br/><br/>        
                                Cheers,<br/>
                                From our web site
                                </td>
       
                                <td></td></tr>
                                </table>
	                        </body>
                        </html>";

        #endregion Email Template

        #region Properties

        public static string EmailAddress
        {
            get { return ConfigSettings.EmailAddress; }
        }

        public static string EmailDisplayName
        {
            get { return ConfigSettings.EmailDisplayName; }
        }

        public static string EmailHost
        {
            get { return ConfigSettings.EmailHost; }
        }

        public static int EmailPort
        {
            get { return ConfigSettings.EmailPort; }
        }

        public static string EmailUser
        {
            get { return ConfigSettings.EmailUser; }
        }

        public static string EmailPassword
        {
            get { return ConfigSettings.EmailPassword; }
        }

        public static bool EmailUseDefaultCredentials
        {
            get { return ConfigSettings.EmailUseDefaultCredentials; }
        }

        public static bool EmailEnableSsl
        {
            get
            {
                return ConfigSettings.EmailEnableSsl;
            }
        }

        public static int EmailQueueInterval
        {
            get
            {
                return ConfigSettings.EmailQueueInterval;
            }
        }
        #endregion

        #region Private Methods
        private static void PersistSentEmailMessage(Guid id, string from, string fromName, string to, string toName, string cc, string bcc, string subject, string body, DateTime createdOn, int sendTries, DateTime sentOn, bool success = true)
        {
            var context = GetEmailMessageContext();
            var message = new EmailMessageEntity
            {
                Bcc = bcc,
                Body = body,
                Cc = cc,
                CreatedOn = createdOn,
                From = from,
                FromName = fromName,
                SendFail = !success,
                SentOn = sentOn,
                SendTries = sendTries,
                Subject = subject,
                To = to,
                ToName = toName,
                PartitionKey = DateTime.Now.Year + DateTime.UtcNow.DayOfYear.ToString("000"),
                RowKey = id.ToString(),
            };

            context.Add(message);
            context.SaveChanges();
        }

        private static void SendEmail(string subject, string body, MailAddress from, MailAddress to, IEnumerable<string> bcc, IEnumerable<string> cc)
        {
            var message = new MailMessage {From = @from};
            message.To.Add(to);
            message.ReplyToList.Add(from);
            if (null != bcc)
            {
                foreach (string address in bcc)
                {
                    if (address != null)
                    {
                        if (!String.IsNullOrEmpty(address.Trim()))
                        {
                            message.Bcc.Add(address.Trim());
                        }
                    }
                }
            }

            if (null != cc)
            {
                foreach (string address in cc)
                {
                    if (address != null)
                    {
                        if (!String.IsNullOrEmpty(address.Trim()))
                        {
                            message.CC.Add(address.Trim());
                        }
                    }
                }
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient
                                 {
                                     UseDefaultCredentials = EmailUseDefaultCredentials,
                                     Host = EmailHost,
                                     Port = EmailPort,
                                     EnableSsl = EmailEnableSsl
                                 };
            smtpClient.Credentials = EmailUseDefaultCredentials ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(EmailUser, EmailPassword);

            smtpClient.Send(message);
        }
        #endregion

        #region Public Methods
        public static void QueueEmailMessage(MailAddress from, MailAddress to, string cc, string bcc, string subject, string body, DateTime createdOn, int sendTries, bool useStandardTemplate = true)
        {
            QueueEmailMessage(from.Address, from.DisplayName, to.Address, to.DisplayName, cc, bcc, subject, body, createdOn, sendTries, useStandardTemplate);
        }

        public static void QueueEmailMessage(string from, string fromName, string to, string toName, string cc, string bcc, string subject, string body, DateTime createdOn, int sendTries, bool useStandardTemplate)
        {
            var client = GetEmailQueueClient();

            if (useStandardTemplate)
                body = EmailTemplate.Replace("<#CONTENT#>", body);

            var emailMessage = new EmailMessage
            {
                From = from,
                FromName = fromName,
                To = to,
                ToName = toName,
                Cc = cc,
                Bcc = bcc,
                Subject = subject,
                Body = body,
                CreatedOn = createdOn,
                SendTries = sendTries,
                Id = Guid.NewGuid(),
            };

            client.AddMessageToQueue(new CloudQueueMessage(emailMessage.Serialize()));
        }

        public static void SendQueuedEmail()
        {
            try
            {
                var queue = GetEmailQueueClient();
                int numberOfQueueMessage = ConfigSettings.EmailNumberOfQueueMessage;
                var cloudQueueMessages = queue.GetQueuedMessages(numberOfQueueMessage);

                if (cloudQueueMessages != null)
                {

                    foreach (var cloudQueueMessage in cloudQueueMessages)
                    {
                        var emailMessage = cloudQueueMessage.Deserialize<EmailMessage>();
                        if (emailMessage != null)
                        {
                            try
                            {
                                SendEmail(
                                    emailMessage.Subject,
                                    emailMessage.Body,
                                    new MailAddress(emailMessage.From, emailMessage.FromName),
                                    new MailAddress(emailMessage.To, emailMessage.ToName),
                                    new List<string> { emailMessage.Bcc },
                                    new List<string> { emailMessage.Cc });
                                PersistSentEmailMessage(emailMessage.Id, emailMessage.From, emailMessage.FromName, emailMessage.To, emailMessage.ToName, emailMessage.Cc, emailMessage.Bcc, emailMessage.Subject, emailMessage.Body, emailMessage.CreatedOn, emailMessage.SendTries + 1, DateTime.UtcNow);
                                queue.DeleteMessage(cloudQueueMessage);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("Email Send Error: " + e.Message);

                                emailMessage.SendTries = emailMessage.SendTries + 1;
                                queue.DeleteMessage(cloudQueueMessage);

                                if (emailMessage.SendTries > 3)
                                {
                                    PersistSentEmailMessage(emailMessage.Id, emailMessage.From,
                                                            emailMessage.FromName, emailMessage.To, emailMessage.ToName,
                                                            emailMessage.Cc, emailMessage.Bcc, emailMessage.Subject,
                                                            emailMessage.Body, emailMessage.CreatedOn, emailMessage.SendTries, DateTime.UtcNow, false);
                                }
                                else
                                {
                                    queue.AddMessageToQueue(new CloudQueueMessage(emailMessage.Serialize()));
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Trace.TraceError("Email Send Error: " + e.Message);
            }
        }

        public static void SendSimpleMessage(string emailTo, string recipientDisplayName, string subject, string text)
        {
            try
            {
                var from = new MailAddress(EmailAddress, EmailDisplayName);
                var to = new MailAddress(emailTo, recipientDisplayName);
                QueueEmailMessage(from, to, string.Empty, string.Empty, subject, text, DateTime.UtcNow, 0);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        #endregion
    }
}