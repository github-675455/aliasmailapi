namespace AliasMailApi.Services
{
    public interface ISecurityService
    {
         byte[] HashHMAC(byte[] key, byte[] message);
    }
}