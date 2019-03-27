using System;
using System.ComponentModel.DataAnnotations;

namespace AliasMailApi.Models
{
    public class MailGunMessage : BaseMessage
    {
        public string ContentType { get; set; }
        public DateTime Date { get; set; }
        public string OriginalFrom { get; set; }
        public string From { get; set; }
        public string InReplyTo { get; set; }
        public string MessageId { get; set; }
        public string MimeVersion { get; set; }
        public string Received { get; set; }
        public string References { get; set; }
        public string OriginalSender { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string OriginalTo { get; set; }
        public string To { get; set; }
        public string UserAgent { get; set; }
        public string XMailgunVariables { get; set; }
        public int AttachmentCount { get; set; }
        public string BodyHtml { get; set; }
        public string BodyPlain { get; set; }
        public string ContentIdMap { get; set; }
        public string SFrom { get; set; }
        public string MessageHeaders { get; set; }
        public string Recipient { get; set; }
        public string SSender { get; set; }
        public string Signature { get; set; }
        public string StrippedHtml { get; set; }
        public string StrippedSignature { get; set; }
        public string StrippedText { get; set; }
        public string SSubject { get; set; }
        public string Timestamp { get; set; }
        
        [StringLength(50)]
        public string Token { get; set; }

    }
}