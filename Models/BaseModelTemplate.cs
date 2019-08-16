using System;
using System.ComponentModel.DataAnnotations;

namespace aliasmailapi.Models
{
    public abstract class BaseModelTemplate
    {
        [Key]
        [MaxLength(36)]
        public Guid Id { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public DateTimeOffset Created { get; set; }

        public BaseModelTemplate()
        {
            Created = DateTime.Now;
        }
    }
}