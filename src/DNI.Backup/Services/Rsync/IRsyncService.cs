using System.IO;
using System.Threading.Tasks;

namespace DNI.Backup.Services.Rsync {
    public interface IRsyncService {
        Task<MemoryStream> CreateSignatureAsync(string inputFilePath);

        Task<MemoryStream> CreateDeltaAsync(string inputFilePath, Stream signatureStream);

        Task<bool> ApplyDeltaAsync(string inputFilePath, Stream deltaStream);
    }
}