using System.Security.Cryptography;
using AliasMailApi.Interfaces;

namespace AliasMailApi.Services
{
    public class SecurityService : ISecurityService
    {
        public byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }
    }
}