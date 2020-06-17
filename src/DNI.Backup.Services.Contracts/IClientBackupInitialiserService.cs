using System.Threading;
using System.Threading.Tasks;

using DNI.Backup.Model;

namespace DNI.Backup.Services.Contracts {
    public interface IClientBackupInitialiserService {
        Task ProcessBackupAsync(CancellationToken cancellationToken, BackupSchedule schedule);
    }
}