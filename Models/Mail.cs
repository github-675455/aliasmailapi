using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AliasMailApi.Models
{
    public class Mail
    {
        [Key]
        public Guid Id;
        public DateTime Created { get; set; }
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
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string UserAgent { get; set; }
        public string InReplyTo { get; set; }
        public string MessageId { get; set; }
        public string Received { get; set; }
        public string References { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
        public string BodyHtml { get; set; }
        public string BodyPlain { get; set; }
        public string Recipient { get; set; }
        public Mail()
        {
            this.Created = DateTime.Now;
        }
    }
}