using System.Collections.Generic;
using System.Threading.Tasks;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;

namespace aliasmailapi.Interfaces
{
    public interface IDNSManager
    {
         Task<IList<IResourceRecord>> Resolve(string domain, RecordType recordType);
    }
}