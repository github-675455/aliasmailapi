using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AliasMailApi.Models
{
    public class Attachment
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public Guid MailId { get; set; }
        public Mail mail { get; set; }
    }
}