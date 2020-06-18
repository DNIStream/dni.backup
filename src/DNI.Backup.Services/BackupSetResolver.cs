using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DNI.Backup.Model;
using DNI.Backup.Model.Settings;
using DNI.Backup.Services.Contracts;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNI.Backup.Services {
    public class BackupSetResolver : IBackupSetResolver {
        private readonly ILogger<BackupSetResolver> _logger;
        private BackupSetSettings _backupSetSettings;

        public BackupSetResolver(ILogger<BackupSetResolver> logger, IOptionsMonitor<BackupSetSettings> backupSetSettings) {
            _logger = logger;
            _backupSetSettings = backupSetSettings.CurrentValue;
            backupSetSettings.OnChange(changedSettings => {
                _backupSetSettings = changedSettings;
            });
            _logger.LogInformation("{service} initialised", GetType().Name);
        }

        public async Task<BackupSet[]> ResolveAsync(string[] setIds) {
            if(setIds == null) {
                throw new ArgumentNullException(nameof(setIds));
            }

            if(setIds.Length == 0) {
                throw new ArgumentException("setsIds must have at least one entry", nameof(setIds));
            }

            var sets = await Task.Run(() => {
                return setIds
                    .Select(id => _backupSetSettings.Sets.FirstOrDefault(x => x.Id == id))
                    .Where(set => set != null)
                    .ToArray();
            });

            return !sets.Any() ? null : sets;
        }
    }
}