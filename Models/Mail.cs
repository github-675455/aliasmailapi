using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AliasMailApi.Models
{
    public class Mail
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        [StringLength(254)]
        public string SenderAddress { get; set; }
        [StringLength(254)]
        public string SenderDisplayName { get; set; }
        [StringLength(254)]
        public string FromAddress { get; set; }
        [StringLength(254)]
        public string FromDisplayName { get; set; }
        [StringLength(254)]
        public string ToAddress { get; set; }
        [StringLength(254)]
        public string ToDisplayName { get; set; }
        public DateTimeOffset Date { get; set; }
        public string OriginalDate { get; set; }
        public string Subject { get; set; }
        [StringLength(4096)]
        public string UserAgent { get; set; }
        public string InReplyTo { get; set; }
        public string MessageId { get; set; }
        public string Received { get; set; }
        public string References { get; set; }
        public string Attachments { get; set; }
        public List<MailAttachment> MailAttachments { get; set; }
        public string BodyHtml { get; set; }
        public string BodyPlain { get; set; }
        public string Recipient { get; set; }
        public string remoteIpAddress { get; set; }
        public Guid? BaseMessageId { get; set; }
        public virtual BaseMessage BaseMessage { get; set; }
        public Mail()
        {
            this.Created = DateTime.Now;
        }
    }
}