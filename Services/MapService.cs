using System;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using AliasMailApi.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;

namespace AliasMailApi.Services
{
    public class MapService : IMapService
    {
        public MailGunMessage mapFrom(IFormCollection formDataCollection) {
            MailGunMessage message = new MailGunMessage();
            
            message.ContentType = formDataCollection["Content-Type"];
            message.Date = DateTime.Parse(formDataCollection["Date"]);
            message.OriginalFrom = formDataCollection["From"];
            
            var emailFrom = new MailAddress(message.OriginalFrom);
            message.From = emailFrom.Address;

            message.InReplyTo = formDataCollection["In-Reply-To"];
            message.MessageId = formDataCollection["Message-Id"];
            message.MimeVersion = formDataCollection["Mime-Version"];
            message.Received = formDataCollection["Received"];
            message.References = formDataCollection["References"];
            message.OriginalSender = formDataCollection["Sender"];

            var emailSender = new MailAddress(message.OriginalSender);
            message.Sender = emailSender.Address;

            message.Subject = formDataCollection["Subject"];
            message.OriginalTo = formDataCollection["To"];

            var emailTo = new MailAddress(message.OriginalTo);
            message.To = emailTo.Address;

            message.UserAgent = formDataCollection["User-Agent"];
            message.XMailgunVariables = formDataCollection["X-Mailgun-Variables"];
            message.AttachmentCount = Int32.Parse(formDataCollection["attachment-count"]);
            message.BodyHtml = formDataCollection["body-html"];
            message.BodyPlain = formDataCollection["body-plain"];
            message.ContentIdMap = formDataCollection["content-id-map"];
            message.SFrom = formDataCollection["from"];
            message.MessageHeaders = formDataCollection["message-headers"];
            message.Recipient = formDataCollection["recipient"];
            message.SSender = formDataCollection["sender"];
            message.Signature = formDataCollection["signature"];
            message.StrippedHtml = formDataCollection["stripped-html"];
            message.StrippedSignature = formDataCollection["stripped-signature"];
            message.StrippedText = formDataCollection["stripped-text"];
            message.SSubject = formDataCollection["subject"];
            message.Timestamp = formDataCollection["timestamp"];
            message.Token = formDataCollection["token"];

            return message;
        }
    }
}