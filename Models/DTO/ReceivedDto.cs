using System;
using System.Net;

namespace aliasmailapi.Models.DTO
{
    public class ReceivedDto
    {
        public string Host;
        public IPAddress Ip;
        public DateTimeOffset Received;
    }
}