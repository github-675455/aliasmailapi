using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AliasMailApi.Configuration;
using AliasMailApi.Interfaces;
using AliasMailApi.Models;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace AliasMailApi.Services
{
    public class MailgunAttachmentService : IMailgunAttachment
    {
        private readonly AppOptions _options;
        public MailgunAttachmentService(IOptions<AppOptions> options){
            _options = options.Value;
        }
        private RestRequest SetupBasicMail(Uri url)
        {
            RestRequest request = new RestRequest();
            request.AddParameter ("domain", _options.mailgunApiDomain, ParameterType.UrlSegment);
            request.Resource = url.AbsolutePath;
            return request;
        }

        private byte[] DoSend(IRestRequest request, Uri url)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri(url.GetLeftPart(UriPartial.Authority));
            client.Authenticator = new HttpBasicAuthenticator("api", _options.mailgunApiToken);
            return client.Execute(request).RawBytes;
        }

        public Task<MailAttachment> get(MailAttachment mailAttachment)
        {
            var newUrl = new Uri(mailAttachment.url);
            var request = SetupBasicMail(newUrl);
            var response = DoSend(request, newUrl);
            mailAttachment.Data = response;
            return Task.FromResult(mailAttachment);
        }
    }
}