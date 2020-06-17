using System;
using System.Threading;
using System.Threading.Tasks;

using DNI.Backup.Model.Settings;
using DNI.Backup.Services;
using DNI.Backup.Services.Contracts;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNI.Backup.Client.Service {
    public class Worker : BackgroundService {
        private readonly IServiceProvider _services;
        private readonly ILogger<Worker> _logger;
        private readonly IScheduleHandler _scheduleHandler;
        private readonly IClientBackupInitialiserService _backupInitialiser;
        private SchedulerSettings _schedulerSettings;

        public Worker(IServiceProvider services, ILogger<Worker> logger, IScheduleHandler scheduleHandler,
            IClientBackupInitialiserService backupInitialiser, IOptionsMonitor<SchedulerSettings> schedulerSettings) {
            _services = services;
            _logger = logger;
            _scheduleHandler = scheduleHandler;
            _backupInitialiser = backupInitialiser;

            _schedulerSettings = schedulerSettings.CurrentValue;
            schedulerSettings.OnChange(changedSettings => {
                _schedulerSettings = changedSettings;
            });

            _logger.LogInformation("{service} initialised", GetType().Name);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
            while(!cancellationToken.IsCancellationRequested) {
                _logger.LogInformation("Worker running at: {time} [{interval}ms]", DateTimeOffset.Now, _schedulerSettings.PollingInterval);

                // Check if the current time matches a schedule
                var schedule = await _scheduleHandler.MatchAsync(SystemTime.Now());
                if(schedule != null) {
                    await _backupInitialiser.ProcessBackupAsync(cancellationToken, schedule);
                }

                await Task.Delay(_schedulerSettings.PollingInterval, cancellationToken);
            }
        }
    }
}