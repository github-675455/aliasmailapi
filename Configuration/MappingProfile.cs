using AliasMailApi.Models;
using AliasMailApi.Models.DTO;
using AutoMapper;

namespace AliasMailApi.Configuration
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<MailgunMessageRequest, MailgunMessage>();
        }
    }
}