using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Web.Helpers
{
    public static class EmailHelper
    {
        public static Logger _logger = LogManager.GetCurrentClassLogger();
        public enum EmailType
        {
            PasswordReminder,
            StudentStatusChange,
            RommApplicationStatusChange
        }


        public static bool Send(EmailType emailType, string to, string body)
        {

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("medipoldormitoryapp@gmail.com", "medipoldormitoryapp2022"),
                EnableSsl = true,
            };
            int tryCount = 0;
            bool result = false;  

            while (true)
            {
                tryCount++;
                try
                {
                    smtpClient.Send("medipoldormitoryapp@gmail.com", to, emailType.ToSubject(), body);
                    result = true;
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Info(ex);

                }
                if (tryCount == 5)
                {
                    break;
                }
            }
            return result;


        }

        private static string ToSubject(this EmailType emailType)
        {
            string subjectFormat = "Dormitory App {0}";
            switch (emailType)
            {
                case EmailType.PasswordReminder:
                    return string.Format(subjectFormat, "Password Reminder");
                case EmailType.StudentStatusChange:
                    return string.Format(subjectFormat, "Students Account");
                case EmailType.RommApplicationStatusChange:
                    return string.Format(subjectFormat, "Romm Application");
                default:
                    return "";

            }
        }

    }
}