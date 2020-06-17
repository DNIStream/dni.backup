using System;
using System.Threading.Tasks;

using DNI.Backup.Model;
using DNI.Backup.Model.Settings;
using DNI.Backup.Services.Contracts;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNI.Backup.Services {
    public class BackupSetResolver : IBackupSetResolver {
        private readonly ILogger<ClientBackupInitialiserService> _logger;
        private BackupSetSettings _backupSetSettings;

        public BackupSetResolver(ILogger<ClientBackupInitialiserService> logger, IOptionsMonitor<BackupSetSettings> backupSetSettings) {
            _logger = logger;
            _backupSetSettings = backupSetSettings.CurrentValue;
            backupSetSettings.OnChange(changedSettings => {
                _backupSetSettings = changedSettings;
            });
            _logger.LogInformation("{service} initialised", GetType().Name);
        }

        public async Task<BackupSet[]> ResolveAsync(string[] setIds) {
            throw new NotImplementedException();
        }
    }
}