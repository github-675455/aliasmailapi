using System;

namespace aliasmailapi.Models
{
    public class UserDevice : BaseModelTemplate
    {
        public Guid UserId { get; set ;}
        public User User { get; set ; }
        public Guid DeviceId { get; set; }
        public Device Device { get; set; }
        
    }
}