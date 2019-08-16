using System;
using System.ComponentModel.DataAnnotations;
using aliasmailapi.Models;

namespace AliasMailApi.Models
{
    public class Domain : BaseModelTemplate
    {
        [StringLength(253)]
        public string Name { get; set; }
        [StringLength(2048)]
        public string Description { get; set; }
        public bool Active { get; set; }

    }
}