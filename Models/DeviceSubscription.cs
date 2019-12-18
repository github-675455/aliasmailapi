using System;
using System.ComponentModel.DataAnnotations;

namespace aliasmailapi.Models
{
    public class DeviceSubscription : BaseModelTemplate
    {
        public Guid? DeviceId { get; set; }
        [MaxLength(4096)]

        public string PushEndpoint { get; set; }

        [MaxLength(4096)]
        public string PushP256DH { get; set; }
        [MaxLength(4096)]
        public string PushAuth { get; set; }
    }
}