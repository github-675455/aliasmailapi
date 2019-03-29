namespace AliasMailApi.Interfaces
{
    public interface ISecurityService
    {
         byte[] HashHMAC(byte[] key, byte[] message);
    }
}