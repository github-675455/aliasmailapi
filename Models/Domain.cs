using System;
using System.ComponentModel.DataAnnotations;

namespace AliasMailApi.Models
{
    public class Domain
    {
        public Guid Id { get; set; }
        [StringLength(253)]
        public string Name { get; set; }
        [StringLength(2048)]
        public string Description { get; set; }
        public bool Active { get; set; }

    }
}