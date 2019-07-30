using System;
using System.ComponentModel.DataAnnotations;
using aliasmailapi.Models.Enum;
using Newtonsoft.Json;

namespace AliasMailApi.Models
{
    public class BaseMessageSpf
    {
        public Guid Id { get; set; }
        public SpfResult Result { get; set; }
        public DateTimeOffset Validated { get; set; }
        public bool Error { get; set; }
        [MaxLength(4096)]
        public string ErrorMessage { get; set; }
        [JsonIgnore]
        public Guid MailId { get; set; }
        [JsonIgnore]
        public virtual Mail mail { get; set; }
    }
}