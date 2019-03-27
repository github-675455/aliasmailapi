using System.Net.Http.Formatting;
using AliasMailApi.Models;
using Microsoft.AspNetCore.Http;

namespace AliasMailApi.Services
{
    public interface IMapService
    {
        MailGunMessage mapFrom(IFormCollection formDataCollection);
    }
}