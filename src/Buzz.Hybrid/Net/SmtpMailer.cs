namespace Buzz.Hybrid.Net
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    using Umbraco.Core.Logging;

    /// <summary>
    /// The SMTP mailer. 
    /// </summary>
    /// <remarks>
    /// Responsible for sending email
    /// </remarks>
    public class SmtpMailer
    {
        /// <summary>
        /// Sends an email 
        /// </summary>
        /// <param name="fromEmail">
        /// The from email.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="messageBody">
        /// The message body.
        /// </param>
        /// <param name="isHtml">
        /// The is html.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Send(string fromEmail, string toEmail, string subject, string messageBody, bool isHtml)
        {
            return Send(fromEmail, toEmail, subject, messageBody, isHtml, null);
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="fromEmail">
        /// The from email.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="messageBody">
        /// The message body.
        /// </param>
        /// <param name="isHtml">
        /// The is html.
        /// </param>
        /// <param name="credential">
        /// The credential.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Send(
            string fromEmail,
            string toEmail,
            string subject,
            string messageBody,
            bool isHtml,
            NetworkCredential credential)
        {
            var msg = new MailMessage(fromEmail, toEmail)
                          {
                              Subject = subject,
                              Body = messageBody,
                              IsBodyHtml = isHtml
                          };

            return Send(msg, credential);
        }

        /// <summary>
        /// Sends an email asynchronously, logging any errors.
        /// </summary>
        /// <param name="msg">
        /// The <see cref="MailMessage"/> to send.
        /// </param>
        /// <param name="credentials">
        /// The <see cref="NetworkCredential"/>s containing identity credentials.
        /// </param>
        /// <returns>
        /// True if the email is sent successfully; otherwise, false.
        /// </returns>
        public static bool Send(MailMessage msg, NetworkCredential credentials = null)
        {
            bool success = true;

            try
            {
                // We want to send the email async
                SmtpClient smtpClient = new SmtpClient()
                {
                    Credentials = credentials ?? CredentialCache.DefaultNetworkCredentials
                };

                smtpClient.SendCompleted += (sender, args) =>
                {
                    smtpClient.Dispose();
                    msg.Dispose();
                };

                smtpClient.SendAsync(msg, null);
            }
            catch (Exception ex)
            {
                success = false;
                LogHelper.Error<SmtpMailer>("Buzz.Hybrid.Net.SmtpMailer failed sending email", ex);
            }

            return success;
        }
    }
}
