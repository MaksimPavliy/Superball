//#if OTHER
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class EmailToDevs
    {
        public static string EmailReceiver = FriendsGamesConstants.DeveloperEmail;
        static string GetTitleFromBody(string body)
        {
            const int MaxTitleLength = 30;
            string title;
            if (body.Length < MaxTitleLength)
                title = body;
            else
            {
                title = body.Substring(0, MaxTitleLength);
                int endLineInd = title.IndexOf('\n');
                if (endLineInd != -1)
                    title = title.Substring(0, endLineInd);
            }
            return title;
        }
        const string SMTPSenderGmail = "friendsgamesincubator@gmail.com";
        const string SMTPSenderGmailPass = "dzxewuyxjklxuwud"; // Application password.
        public static async Task<string> Send(string body)
            => await Email.SendFromGoogleSMTPServer(EmailReceiver, GetTitleFromBody(body), body, SMTPSenderGmail, SMTPSenderGmailPass);
        public static async Task<string> Send(string title, string body)
        {
            title = GetEmailTitle(title);
            return await Email.SendFromGoogleSMTPServer(EmailReceiver, title, body, SMTPSenderGmail, SMTPSenderGmailPass);
        }
        public static string GetEmailTitle(string title)
            => $"{title} from {Application.productName} {SystemInfo.deviceModel} {DateTime.Now.ToShortTimeString()} build {BuildInfoManager.buildInfo}";
    }
}
//#endif