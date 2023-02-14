
//Created by Alexander Fields 
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Optimization.Utility
{
    public class Mailing
    {
        /// <summary>
        /// Creates an StpClient
        /// </summary>
        /// <param name="smtpServer"></param>
        /// <param name="emailToLoginWith"></param>
        /// <param name="password"></param>
        /// <param name="portNumber"></param>
        /// <returns>SmtpClient object</returns>
        public SmtpClient CreateSmtpClient(string smtpServer, string emailToLoginWith, string password, int portNumber)
        {
            SmtpClient client = new SmtpClient(smtpServer, portNumber);

            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(emailToLoginWith, password);

            return client;
        }

        public MailMessage CreateEmail(string emailTo, string emailFrom, string subject, string body, bool isHTML, List<Attachment> attachments)
        {
            return CreateEmail(emailTo, emailFrom, null, null, subject, body, isHTML, attachments);
        }

        public MailMessage CreateEmail(string emailTo, string emailFrom, string subject, string body, bool isHTML, List<string> filesToAttach)
        {
            return CreateEmail(emailTo, emailFrom, null, null, subject, body, isHTML, filesToAttach);
        }

        public MailMessage CreateEmail(string emailTo, string emailFrom, string emailToCC, string subject, string body, bool isHTML, List<Attachment> attachments)
        {
            return CreateEmail(emailTo, emailFrom, emailToCC, null, subject, body, isHTML, attachments);
        }

        public MailMessage CreateEmail(string emailTo, string emailFrom, string emailToCC, string subject, string body, bool isHTML, List<string> filesToAttach)
        {
            return CreateEmail(emailTo, emailFrom, emailToCC, null, subject, body, isHTML, filesToAttach);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="emailTo">Can add multiple reicipients so long as comma seperating</param>
        /// <param name="emailFrom"></param>
        /// <param name="emailToCC"></param>
        /// <param name="emailToBCC"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHTML"></param>
        /// <param name="attachments"></param>
        /// <returns>MailMessage Object</returns>
        public MailMessage CreateEmail(string emailTo, string emailFrom, string emailToCC, string emailToBCC, string subject, string body, bool isHTML, List<Attachment> attachments)
        {
            MailMessage email = EmailBuilder(emailTo, emailFrom, emailToCC, emailToBCC, subject, body, isHTML);

            if (attachments == null)
            {
                return email;
            }

            foreach (Attachment attachment in attachments)
            {
                email.Attachments.Add(attachment);
            }

            return email;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="emailTo">Can add multiple reicipients so long as comma seperating</param>
        /// <param name="emailFrom"></param>
        /// <param name="emailToCC"></param>
        /// <param name="emailToBCC"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHTML"></param>
        /// <param name="filesToAttach"></param>
        /// <returns>MailMessage Object</returns>
        public MailMessage CreateEmail(string emailTo, string emailFrom, string emailToCC, string emailToBCC, string subject, string body, bool isHTML, List<string> filesToAttach)
        {
            MailMessage email = EmailBuilder(emailTo, emailFrom, emailToCC, emailToBCC, subject, body, isHTML);

            if (filesToAttach == null)
            {
                return email;
            }
            List<Attachment> attachments = new List<Attachment>();

            foreach (string file in filesToAttach)
            {
                attachments.Add(new Attachment(file));
            }

            foreach (Attachment attachment in attachments)
            {
                email.Attachments.Add(attachment);
            }

            return email;
        }

        public MailMessage EmailBuilder(string emailTo, string emailFrom, string emailToCC, string emailToBCC, string subject, string body, bool isHTML)
        {
            MailMessage email = new MailMessage();
            email.To.Add(emailTo);

            if (!string.IsNullOrWhiteSpace(emailToCC))
            {
                email.CC.Add(emailToCC);
            }

            if (!string.IsNullOrWhiteSpace(emailToBCC))
            {
                email.Bcc.Add(emailToBCC);
            }
            email.From = new MailAddress(emailFrom);
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = isHTML;

            return email;
        }
    }
}