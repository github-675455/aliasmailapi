using System;
using System.ComponentModel.DataAnnotations;

namespace AliasMailApi.Models.DTO
{
    public class MailRequest
    {
        [Required]
        [RegularExpression(@"^[0-9a-z]+$")]
        
        public string Id { get; set; }
    }
}