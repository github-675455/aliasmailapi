using System.ComponentModel.DataAnnotations;

namespace aliasmailapi.Models
{
    public class User : BaseModelTemplate
    {
        [MaxLength(256)]
        public string Username { get; set; }
        [MaxLength(512)]
        public string Password { get; set; }
        [MaxLength(512)]
        public string Salt { get; set; }
    }
}