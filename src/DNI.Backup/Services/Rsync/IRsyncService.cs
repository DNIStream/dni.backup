using System.IO;
using System.Threading.Tasks;

namespace DNI.Backup.Services.Rsync {
    public interface IRsyncService {
        Task<MemoryStream> CreateSignature(string inputFilePath);

        Task<MemoryStream> CreateDelta(string inputFilePath, Stream signatureStream);

        Task<MemoryStream> ApplyDelta(string inputFilePath, Stream deltaStream);
    }
}