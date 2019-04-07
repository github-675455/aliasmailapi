using System;
using System.Net.Mail;
using AliasMailApi.Models;
using AliasMailApi.Models.DTO;
using AliasMailApi.Models.Enum;
using AutoMapper;

namespace AliasMailApi.Configuration
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<MailgunMessageRequest, MailgunMessage>();
            CreateMap<MailgunMessage, Mail>()
            .ForMember(e => e.Id, opt => opt.Ignore())
            .ForMember(e => e.BaseMessageId, opt => opt.MapFrom(src => src.Id))
            .ForMember(e => e.Date, opt => opt.MapFrom(src => DateTimeOffset.Parse(src.Date)))
            .ForMember(e => e.OriginalDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(e => e.ToAddress, opt => opt.MapFrom(src => new MailAddress(src.To).Address))
            .ForMember(e => e.ToDisplayName, opt => opt.MapFrom(src => new MailAddress(src.To).DisplayName))
            .ForMember(e => e.FromAddress, opt => opt.MapFrom(src => new MailAddress(src.From).Address))
            .ForMember(e => e.FromDisplayName, opt => opt.MapFrom(src => new MailAddress(src.From).DisplayName))
            .ForMember(e => e.SenderAddress, opt => opt.MapFrom(src => new MailAddress(src.Sender).Address))
            .ForMember(e => e.SenderDisplayName, opt => opt.MapFrom(src => new MailAddress(src.Sender).DisplayName))
            .ForMember(e => e.Source, opt => opt.MapFrom( o => DataSource.Mailgun));
        }
    }
}