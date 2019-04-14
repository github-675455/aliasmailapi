using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AliasMailApi.Models.Enum;
using Microsoft.AspNetCore.Mvc;

namespace AliasMailApi.Models.DTO.Response
{
    public class SimpleMailResponse
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string SenderAddress { get; set; }
        public string SenderDisplayName { get; set; }
        public string FromAddress { get; set; }
        public string FromDisplayName { get; set; }
        public string ToAddress { get; set; }
        public string ToDisplayName { get; set; }
        public DateTimeOffset Date { get; set; }
        public string OriginalDate { get; set; }
        public string Subject { get; set; }
        public string UserAgent { get; set; }
        public string InReplyTo { get; set; }
        public string MessageId { get; set; }
        public string Received { get; set; }
        public string References { get; set; }
        public string Attachments { get; set; }
        public List<MailAttachment> MailAttachments { get; set; }
        public JobStats MailAttachmentsJobStatus { get; set; }
        public string MailAttachmentsJobErrorMessage { get; set; }
        public string Recipient { get; set; }
        public string remoteIpAddress { get; set; }
        public Guid? BaseMessageId { get; set; }
        public virtual BaseMessage BaseMessage { get; set; }
        public JobStats JobStatus { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public DateTimeOffset ErrorDate { get; set; }
        public int Retries { get; set; }
        public DateTimeOffset NextRetry { get; set; }
        public DataSource Source { get; set; }
    }
}