using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Monitoreo.Helpers
{
    public class Logger
    {
        
        public static async Task LogEvent(string userName, string title ,string text, string emailTo, DateTime date)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.abcdtips.com";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("anibal@abcdtips.com", "samurai234517");

            var body = text;
            var message = new MailMessage();
            if(emailTo != ""){
                message.To.Add(new MailAddress(emailTo));
            }
            message.To.Add(new MailAddress("ana.sanchez@ceed.edu.do"));
            message.To.Add(new MailAddress("anibal.perez@ceed.edu.do"));

            message.From = new MailAddress("anibal@abcdtips.com"); // quien lo envia
            message.Subject = title;
            message.Body = body;
            message.IsBodyHtml = true;
            smtp.EnableSsl = false;
            try {
                await smtp.SendMailAsync(message);
            }
            catch(Exception e){
                var msj = e.Message;
            }
            
        }

    }
}