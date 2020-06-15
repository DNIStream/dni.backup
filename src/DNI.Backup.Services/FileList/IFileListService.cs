using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNI.Backup.Services.FileList {
    public interface IFileListService {
        Task<IEnumerable<string>> GetFilesAsync(IEnumerable<IDirectoryGlobSettings> directoryGlobSettings);
    }
}