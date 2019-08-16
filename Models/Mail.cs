using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using aliasmailapi.Models;
using AliasMailApi.Models.Enum;

namespace AliasMailApi.Models
{
    public class Mail : BaseModelTemplate
    {
        [MaxLength(256)]
        public string SenderAddress { get; set; }
        [MaxLength(256)]
        public string SenderDisplayName { get; set; }
        [MaxLength(256)]
        public string FromAddress { get; set; }
        [MaxLength(256)]
        public string FromDisplayName { get; set; }
        [MaxLength(256)]
        public string ToAddress { get; set; }
        [MaxLength(256)]
        public string ToDisplayName { get; set; }
        public DateTimeOffset? Date { get; set; }
        [MaxLength(256)]
        public string OriginalDate { get; set; }
        public string Subject { get; set; }
        [MaxLength(4096)]
        public string UserAgent { get; set; }
        [MaxLength(4096)]
        public string InReplyTo { get; set; }
        [MaxLength(4096)]
        public string MessageId { get; set; }
        [MaxLength(16384)]
        public string Received { get; set; }
        [MaxLength(4096)]
        public string References { get; set; }
        [MaxLength(16384)]
        public string Attachments { get; set; }
        public List<MailAttachment> MailAttachments { get; set; }
        [MaxLength(32)]
        public JobStats MailAttachmentsJobStatus { get; set; }
        [MaxLength(4096)]
        public string MailAttachmentsJobErrorMessage { get; set; }
        [MaxLength(10485760)]
        public string BodyHtml { get; set; }
        [MaxLength(10485760)]
        public string BodyPlain { get; set; }
        public string Recipient { get; set; }
        [MaxLength(45)]
        public string remoteIpAddress { get; set; }
        [MaxLength(36)]
        public Guid? BaseMessageId { get; set; }
        public virtual BaseMessage BaseMessage { get; set; }
        public JobStats JobStats { get; set; }
        [MaxLength(4096)]
        public string ErrorMessage { get; set; }
        public DateTimeOffset? ErrorDate { get; set; }
        public int Retries { get; set; }
        public DateTimeOffset? NextRetry { get; set; }
        public DataSource Source { get; set; }

        public Mail(BaseMessage baseMessage) : this()
        {
            this.BaseMessageId = baseMessage.Id;
        }
        public Mail()
        {
            this.MailAttachmentsJobStatus = JobStats.Pending;
            this.JobStats = JobStats.Pending;
            this.Retries = 10;
        }
    }
}