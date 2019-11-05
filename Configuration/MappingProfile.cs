using System;
using System.Net.Mail;
using AliasMailApi.Models;
using AliasMailApi.Models.DTO;
using AliasMailApi.Models.DTO.Response;
using AliasMailApi.Models.Enum;
using AutoMapper;

namespace AliasMailApi.Configuration
{
    public enum EmailSection
    {
        Address,
        DisplayName
    }
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<MailgunMessageRequest, MailgunMessage>();
            CreateMap<MailgunMessage, SimpleMailgunResponse>();
            CreateMap<Mail, SimpleMailResponse>();
            CreateMap<MailgunMessage, Mail>()
            .ForMember(e => e.Id, opt => opt.Ignore())
            .ForMember(e => e.BaseMessageId, opt => opt.MapFrom(src => src.Id))
            .ForMember(e => e.Date, opt => opt.MapFrom(src => CustomDateEmailFormat(src.Date)))
            .ForMember(e => e.OriginalDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(e => e.ToAddress, opt => opt.MapFrom(src => TreatEmptyMailAddress(src.To, EmailSection.Address)))
            .ForMember(e => e.ToDisplayName, opt => opt.MapFrom(src => TreatEmptyMailAddress(src.To, EmailSection.DisplayName)))
            .ForMember(e => e.FromAddress, opt => opt.MapFrom(src => TreatEmptyMailAddress(src.From, EmailSection.Address)))
            .ForMember(e => e.FromDisplayName, opt => opt.MapFrom(src => TreatEmptyMailAddress(src.From, EmailSection.DisplayName)))
            .ForMember(e => e.SenderAddress, opt => opt.MapFrom(src => TreatEmptyMailAddress(src.Sender, EmailSection.Address)))
            .ForMember(e => e.SenderDisplayName, opt => opt.MapFrom(src => TreatEmptyMailAddress(src.Sender, EmailSection.DisplayName)))
            .ForMember(e => e.Source, opt => opt.MapFrom( o => DataSource.Mailgun));
        }

        private static string TreatEmptyMailAddress(string fullEmail, EmailSection emailSection)
        {
            if(string.IsNullOrWhiteSpace(fullEmail))
                return string.Empty;

            try
            {
                var mailAdress = new MailAddress(fullEmail);
                return emailSection == EmailSection.Address ? mailAdress.Address : mailAdress.DisplayName;
            }
            catch(Exception exception) when (exception is ArgumentNullException || exception is FormatException)
            {
                return string.Empty;   
            }
        }

        private static DateTimeOffset? CustomDateEmailFormat(string date)
        {
            var result = new DateTimeOffset();

            var sanitizeDate = date.Replace("(UTC)", string.Empty).Replace("(CEST)", string.Empty);

            if(DateTimeOffset.TryParse(sanitizeDate, out result)) {
                return result;
            }

            return null;
        }
    }
}