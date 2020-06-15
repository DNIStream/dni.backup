using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace DNI.Backup.Services.BackupInitialiser {
    public class ClientBackupInitialiserService : IClientBackupInitialiserService {
        private readonly ILogger<ClientBackupInitialiserService> _logger;

        public ClientBackupInitialiserService(ILogger<ClientBackupInitialiserService> logger) {
            _logger = logger;
            _logger.LogInformation("Created backup service scoped");
        }

        public async Task ProcessBackup(CancellationToken cancellationToken) {
            _logger.LogInformation("ProcessBackup() initialised ");
            //while(!cancellationToken.IsCancellationRequested) {
            // _logger.LogInformation("ProcessBackup() iteration");
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
            // TODO: Await server response and if successful, return true
            await Task.Delay(1000, cancellationToken);
            // throw new NotImplementedException();
            //}
        }
    }
}