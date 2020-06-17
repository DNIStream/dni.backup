using DNI.Backup.Model;
using DNI.Backup.Model.Settings;
using DNI.Backup.Model.Validators;
using DNI.Backup.Services;
using DNI.Backup.Services.Contracts;

using FluentValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DNI.Backup.Client.Service {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => {
                    config.Sources.Clear();
                    config
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("client-settings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    if(args != null) {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<Worker>();

                    // Configuration
                    services.Configure<BackupServerSettings>(hostContext.Configuration.GetSection("backupSettings:backupServer"));
                    services.Configure<BackupScheduleSettings>(hostContext.Configuration.GetSection("backupSettings:backupSchedules"));
                    services.Configure<BackupSetSettings>(hostContext.Configuration.GetSection("backupSettings:backupSets"));
                    services.Configure<SchedulerSettings>(hostContext.Configuration.GetSection("backupSettings:schedulerSettings"));

                    // Validators
                    services.AddTransient<IValidator<DirectoryGlob>, DirectoryGlobValidator>();

                    // Services
                    services.AddSingleton<IScheduleHandler, ScheduleHandler>();
                    services.AddTransient<IClientBackupInitialiserService, ClientBackupInitialiserService>();
                    services.AddTransient<IBackupSetResolver, BackupSetResolver>();
                    services.AddTransient<IRsyncService, RsyncService>();
                    services.AddTransient<IFileListService, FileListService>();
                });
    }
}