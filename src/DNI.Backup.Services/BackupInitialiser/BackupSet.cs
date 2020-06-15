using DNI.Backup.Services.FileList;

namespace DNI.Backup.Services.BackupInitialiser {
    public class BackupSet : DirectoryGlobSetting {
        public string Id { get; set; }

        public bool PreserveTree { get; set; }

        public bool DeleteDestinationFilesNotInSource { get; set; }

        public bool CompressBeforeTransfer { get; set; }
    }
}