using System;
using System.Threading;
using System.Threading.Tasks;

using DNI.Backup.Services.BackupInitialiser;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNI.Backup {
    public class Worker : BackgroundService {
        private readonly IServiceProvider _services;
        private readonly ILogger<Worker> _logger;
        private BackupScheduleSettings _scheduleSettings;

        public Worker(IServiceProvider services, ILogger<Worker> logger, IOptionsMonitor<BackupScheduleSettings> scheduleSettings) {
            _services = services;
            _logger = logger;

            _scheduleSettings = scheduleSettings.CurrentValue;
            scheduleSettings.OnChange(changedSchedules => {
                _scheduleSettings = changedSchedules;
            });

            _logger.LogInformation("start");
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
            while(!cancellationToken.IsCancellationRequested) {
                _logger.LogInformation(_scheduleSettings.Schedules[0].Schedule);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // If schedule is hit
                if((DateTime.Now.Second % 5) == 0) {
                    using(var scope = _services.CreateScope()) {
                        var backupInitialiser = scope.ServiceProvider.GetRequiredService<IClientBackupInitialiserService>();
                        await backupInitialiser.ProcessBackup(cancellationToken);
                    }
                }

                // TODO: Poll every 60 seconds?
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}