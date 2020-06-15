using System;

namespace DNI.Backup.Services.FileList {
    public class DirectoryGlobSettingsValidationException : Exception {
        public DirectoryGlobSettingsValidationException(string message, Exception innerException = null)
            : base(message, innerException) {
        }
    }
}