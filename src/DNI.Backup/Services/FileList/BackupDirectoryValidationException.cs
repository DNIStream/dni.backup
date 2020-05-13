using System;

namespace DNI.Backup.Services.FileList {
    public class BackupDirectoryValidationException : Exception {
        public BackupDirectoryValidationException(string message, Exception innerException = null)
            : base(message, innerException) {
        }
    }
}