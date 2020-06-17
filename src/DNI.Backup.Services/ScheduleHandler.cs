using System;
using System.Threading.Tasks;

using DNI.Backup.Model;
using DNI.Backup.Model.Settings;
using DNI.Backup.Services.Contracts;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNI.Backup.Services {
    /// <summary>
    ///     Handles scheduling
    /// </summary>
    public class ScheduleHandler : IScheduleHandler {
        private readonly ILogger<ClientBackupInitialiserService> _logger;
        private BackupScheduleSettings _scheduleSettings;

        public ScheduleHandler(ILogger<ClientBackupInitialiserService> logger, IOptionsMonitor<BackupScheduleSettings> scheduleSettings) {
            _logger = logger;
            _scheduleSettings = scheduleSettings.CurrentValue;
            scheduleSettings.OnChange(changedSettings => {
                _scheduleSettings = changedSettings;
            });
            _logger.LogInformation("{service} initialised", GetType().Name);
        }

        /// <summary>
        ///     Matches the specified <paramref name="matchDate" /> against a schedule in the configuration and returns that
        ///     schedule.
        /// </summary>
        /// <param name="matchDate"></param>
        /// <returns>A valid schedule or null if a schedule is not matched</returns>
        public async Task<BackupSchedule> MatchAsync(DateTime matchDate) {
            // TODO: Implement cron scheduling and write tests
            // N.B. This needs to be as efficient as possible
            // https://github.com/HangfireIO/Cronos
            return _scheduleSettings.Schedules[0];
        }
    }
}