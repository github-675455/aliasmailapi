using System;
using System.ComponentModel.DataAnnotations;

namespace aliasmailapi.Models
{
    public class Device : BaseModelTemplate
    {
        [MaxLength(1024)]
        public string UserAgent { get; set; }
        [MaxLength(45)]
        public string RemoteIpAddress { get; set; }
        public DateTimeOffset LastConnected { get; set; }
    }
}