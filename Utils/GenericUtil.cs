using System;
using System.Net.Mail;
using AliasMailApi.Models.Enum;

namespace aliasmailapi.Utils
{
    public static class GenericUtil
    {
        public static string ParseMailAddress(string fullEmail, EmailSection emailSection)
        {
            if(string.IsNullOrWhiteSpace(fullEmail))
                return string.Empty;

            var mailAdress = new MailAddress(fullEmail);
            return emailSection == EmailSection.Address ? mailAdress.Address : mailAdress.DisplayName;
        }

        public static DateTimeOffset? CustomDateEmailFormat(string date)
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