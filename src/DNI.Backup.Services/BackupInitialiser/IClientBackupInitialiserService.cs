using System.Threading;
using System.Threading.Tasks;

namespace DNI.Backup.Services.BackupInitialiser {
    public interface IClientBackupInitialiserService {
        Task ProcessBackup(CancellationToken cancellationToken);
    }
}