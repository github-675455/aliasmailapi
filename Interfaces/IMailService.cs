using System.Collections.Generic;

namespace AliasMailApi.Interfaces
{
    interface IMailService
    {
        void send(string from, string to, string subject, string textMessage);
        void send(string from, string to, string subject, string textMessage, string htmlMessage);
        void send(string from, string to, string subject, string textMessage, string htmlMessage, List<string> attachments);
        void send(string from, string to, string subject, string cc, string bcc, string textMessage, string htmlMessage, List<string> attachments);
    }
}