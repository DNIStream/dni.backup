using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNI.Backup.Services.FileList {
    public interface IFileListService {
        IAsyncEnumerable<string> GetFilesAsync(IEnumerable<BackupDirectorySetting> backupDirectorySettings);
    }
}