using System;

namespace DNI.Backup.Model.Validators {
    public class DirectoryGlobSettingsValidationException : Exception {
        public DirectoryGlobSettingsValidationException(string message, Exception innerException = null)
            : base(message, innerException) {
        }
    }
}