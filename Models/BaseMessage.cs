using System;
using System.ComponentModel.DataAnnotations;

namespace AliasMailApi.Models
{
    public class BaseMessage
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public bool Valid { get; set; }
        public DateTime Validated { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }

        [StringLength(128)]
        public string RemoteIpAddress { get; set; }

        public BaseMessage(){
            this.Created = DateTime.Now;
        }
    }
}