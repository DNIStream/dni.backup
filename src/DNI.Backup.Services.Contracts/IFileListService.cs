using System.Collections.Generic;
using System.Threading.Tasks;

using DNI.Backup.Model;

namespace DNI.Backup.Services.Contracts {
    public interface IFileListService {
        Task<IEnumerable<string>> GetFilesAsync(IEnumerable<IDirectoryGlob> directoryGlobSettings);
    }
}