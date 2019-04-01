using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AliasMailApi.Models.DTO
{
    public class MailgunMessageRequest
    {
        [BindNever]
        public string RemoteIpAddress { get; set; }

        [FromForm(Name="Content-Type")]
        public string ContentType { get; set; }
        [FromForm(Name="Date")]
        public string Date { get; set; }
        [FromForm(Name="From")]
        public string From { get; set; }
        [FromForm(Name="In-Reply-To")]
        public string InReplyTo { get; set; }
        [FromForm(Name="Message-Id")]
        public string MessageId { get; set; }
        [FromForm(Name="Mime-Version")]
        public string MimeVersion { get; set; }
        [FromForm(Name="Received")]
        public string Received { get; set; }
        [FromForm(Name="References")]
        public string References { get; set; }
        [FromForm(Name="Sender")]
        public string Sender { get; set; }
        [FromForm(Name="Subject")]
        public string Subject { get; set; }
        [FromForm(Name="To")]
        public string To { get; set; }
        [FromForm(Name="User-Agent")]
        public string UserAgent { get; set; }
        [FromForm(Name="X-Mailgun-Variables")]
        public string XMailgunVariables { get; set; }
        [FromForm(Name="attachment-count")]
        public int AttachmentCount { get; set; }
        [FromForm(Name="attachments")]
        public int Attachments { get; set; }
        [FromForm(Name="body-html")]
        public string BodyHtml { get; set; }
        [FromForm(Name="body-plain")]
        public string BodyPlain { get; set; }
        [FromForm(Name="content-id-map")]
        public string ContentIdMap { get; set; }
        [FromForm(Name="message-headers")]
        public string MessageHeaders { get; set; }
        [FromForm(Name="recipient")]
        public string Recipient { get; set; }
        [FromForm(Name="signature")]
        public string Signature { get; set; }
        [FromForm(Name="stripped-html")]
        public string StrippedHtml { get; set; }
        [FromForm(Name="stripped-signature")]
        public string StrippedSignature { get; set; }
        [FromForm(Name="stripped-text")]
        public string StrippedText { get; set; }
        [FromForm(Name="timestamp")]
        public string Timestamp { get; set; }
        [FromForm(Name="token")]
        public string Token { get; set; }   
    }
}