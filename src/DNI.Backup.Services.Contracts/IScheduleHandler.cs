using System;
using System.Threading.Tasks;

using DNI.Backup.Model;

namespace DNI.Backup.Services.Contracts {
    public interface IScheduleHandler {
        Task<BackupSchedule> MatchAsync(DateTime matchDate);
    }
}