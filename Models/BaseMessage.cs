using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using aliasmailapi.Models;

namespace AliasMailApi.Models
{
    public class BaseMessage : BaseModelTemplate
    {
        public bool Valid { get; set; }
        public DateTimeOffset Validated { get; set; }
        public bool Error { get; set; }
        [MaxLength(4096)]
        public string ErrorMessage { get; set; }

        [MaxLength(45)]
        public string RemoteIpAddress { get; set; }
        public virtual ICollection<Mail> Mails { get; set; }
    }
}