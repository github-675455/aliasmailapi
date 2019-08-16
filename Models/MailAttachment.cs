using System;
using aliasmailapi.Models;
using Newtonsoft.Json;

namespace AliasMailApi.Models
{
    public class MailAttachment : BaseModelTemplate
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonIgnore]
        public byte[] Data { get; set; }
        [JsonProperty("size")]
        public long Size { get; set; }
        [JsonProperty("content-type")]
        public string ContentType { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonIgnore]
        public Guid MailId { get; set; }
        [JsonIgnore]
        public virtual Mail mail { get; set; }
    }
}