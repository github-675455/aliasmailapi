using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DNS.Client;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using Polly;
using Polly.Timeout;

namespace aliasmailapi.Utils
{
    public static class DNSManager
    {
        public static async Task<IList<IResourceRecord>> Resolve(string domain, RecordType recordType)
        {
            IResponse response = await Policy
                .Handle<ResponseException>()
                .Or<TimeoutRejectedException>()
                .WaitAndRetry(3, (c, t) => TimeSpan.FromMilliseconds(100))
                .Wrap(Policy.Timeout(1))
                .Execute(async () =>
                {
                    ClientRequest request = new ClientRequest("172.18.210.6");

                    request.Questions.Add(new Question(DNS.Protocol.Domain.FromString(domain), recordType));
                    request.RecursionDesired = true;           

                    return await request.Resolve();
                });

            return response.AnswerRecords;
        }
    }
}