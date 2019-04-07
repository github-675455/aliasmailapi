using System;
using System.Collections.Generic;
using AliasMailApi.Configuration;
using AliasMailApi.Interfaces;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace AliasMailApi.Services
{
    public class MailgunService
    {
        private readonly AppOptions _options;
        private readonly Uri urlApi = new Uri ("https://api.eu.mailgun.net/v3");
        public MailgunService(IOptions<AppOptions> options){
            _options = options.Value;
        }
        public void send(string from, string to, string subject, string textMessage)
        {
            var request = SetupBasicMail(from, to, subject, textMessage);
            DoSend(request);
        }

        public void send(string from, string to, string subject, string textMessage, string htmlMessage)
        {
            var request = SetupBasicMail(from, to, subject, textMessage);
        }

        public void send(string from, string to, string subject, string textMessage, string htmlMessage, List<string> attachments)
        {
            throw new System.NotImplementedException();
        }

        public void send(string from, string to, string subject, string cc, string bcc, string textMessage, string htmlMessage, List<string> attachments)
        {
            throw new System.NotImplementedException();
        }
        private RestRequest SetupBasicMail(string from, string to, string subject, string textMessage)
        {
            RestRequest request = new RestRequest();
            request.AddParameter ("domain", _options.mailgunApiDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter ("from", from);
            request.AddParameter ("to", to);
            request.AddParameter ("subject", subject);
            request.AddParameter ("text", textMessage);
            request.Method = Method.POST;
            return request;
        }

        private void DoSend(RestRequest request)
        {
            RestClient client = new RestClient();
            client.BaseUrl = urlApi;
            client.Authenticator = new HttpBasicAuthenticator("api",  _options.mailgunApiToken);
            var response = client.Execute (request);
            System.Console.Out.WriteLine(response.Content.ToString());
        }
    }
}