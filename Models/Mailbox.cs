using System;
using System.ComponentModel.DataAnnotations;

namespace AliasMailApi.Models
{
    public class Mailbox
    {
        public Mailbox()
        {
            this.Created = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }
        
        [StringLength(512)]
        public string Email { get; set; }
        public Guid DomainId { get; set; }
        public Domain Domain { get; set; }

        [StringLength(2048)]
        public string Description { get; set; }
        public int StoreQuantity { get; set; }
        public bool Reject { get; set; }
        public bool Delete { get; set; }
        public bool CreatedManually { get; set; }
        public DateTime Created { get; set; }

        
    }
}