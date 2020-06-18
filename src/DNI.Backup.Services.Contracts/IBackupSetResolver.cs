using System.Collections.Generic;
using System.Threading.Tasks;

using DNI.Backup.Model;

namespace DNI.Backup.Services.Contracts {
    public interface IBackupSetResolver {
        Task<BackupSet[]> ResolveAsync(string[] setIds);
    }
}