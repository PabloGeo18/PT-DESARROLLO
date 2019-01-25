using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    public class MailUtil
    {
        private SmtpClient smtpClient;

        public MailUtil() {
            this.smtpClient = new SmtpClient();        
        }

        public void sendTextMail(String email, String username, String subject, String content) {
            try
            {
                MailMessage msg = new MailMessage(username, email, subject, content);
                msg.BodyEncoding = Encoding.UTF8;
                msg.SubjectEncoding = Encoding.UTF8;

                //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html);
                //htmlView.ContentType = new System.Net.Mime.ContentType("text/html");
                //msg.AlternateViews.Add(htmlView);

                smtpClient.Send(msg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {

            }

        
        }


    }
}