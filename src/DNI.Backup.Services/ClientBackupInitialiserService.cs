using System.Threading;
using System.Threading.Tasks;

using DNI.Backup.Model;
using DNI.Backup.Services.Contracts;

using Microsoft.Extensions.Logging;

namespace DNI.Backup.Services {
    public class ClientBackupInitialiserService : IClientBackupInitialiserService {
        private readonly ILogger<ClientBackupInitialiserService> _logger;
        private readonly IBackupSetResolver _backupSetResolver;

        public ClientBackupInitialiserService(ILogger<ClientBackupInitialiserService> logger, IBackupSetResolver backupSetResolver, IFileListService fileListService) {
            _logger = logger;
            _backupSetResolver = backupSetResolver;
            _logger.LogInformation("{service} initialised", GetType().Name);
        }

        public async Task ProcessBackupAsync(CancellationToken cancellationToken, BackupSchedule schedule) {
            _logger.LogInformation("ProcessBackupAsync() called");
            // TODO: Iterate through configs
            // TODO: Validate / read configs against backupConfigs in client configuration file (.Net Core configuration reader?)
            // TODO: Enumerate list of local files to be backed up
            // TODO: Send file list to server (compress?)
            // TODO: Server responds with file list status and / or deltas
            //      new (doesn't exist on server)
            //      exists (server sends file signature back)
            //      deleted from source (exists on server, but not on client)

            // TODO: For each file in server response
            //      if new, send file
            //      if exists, generate delta and send to server -> Server then applies delta patch
            //      if deleted, tell server to delete if DeleteDestinationFilesNotInSource set to true
        }
    }
}