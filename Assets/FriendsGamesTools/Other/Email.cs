//#if OTHER
using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FriendsGamesTools
{
    public static class Email
    {
        public static void SendFromDefaultApplication(string receiverEmail, string title, string body)
            => Application.OpenURL($"mailto:{receiverEmail}?subject={EscapeURL(title)}&body={EscapeURL(body)}");
        static string EscapeURL(string URL) => UnityWebRequest.EscapeURL(URL).Replace("+", "%20");
        public static async Task<string> SendFromGoogleSMTPServer(string receiverEmail, string title, string body, 
            string senderGMail, string senderGMailPass)
        {
            // Written this method from here:
            // https://forum.unity.com/threads/how-to-send-mail-from-unity.580639/

            // Notes:
            // Not tested so far, but:
            // Works if you allowed secure less apps here:
            // https://support.google.com/accounts/answer/6010255?hl=en
            // Also set stripping to low.

            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(senderGMail);
            mail.To.Add(receiverEmail);
            mail.Subject = title;
            mail.Body = body;

            //System.Net.Mail.Attachment attachment;
            //attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
            //mail.Attachments.Add(attachment);

            smtp.Port = 587;// 465;// 25;
            smtp.Credentials = new System.Net.NetworkCredential(senderGMail, senderGMailPass);
            smtp.EnableSsl = true;

            string error = string.Empty;
            smtp.SendCompleted += (object sender, AsyncCompletedEventArgs e) =>
            {
                if (e.Error != null)
                    error = e.Error.Message;//.GetBaseException().ToString();
                else if (e.Cancelled)
                    error = "sending cancelled";
            };
            try
            {
                await smtp.SendMailAsync(mail);
            } catch (Exception e)
            {
                error = e.Message;//.GetBaseException().ToString();
            }
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError(error);
                error = "Error - can't send a message";
            }

            return error;
        }
    }
}
//#endif