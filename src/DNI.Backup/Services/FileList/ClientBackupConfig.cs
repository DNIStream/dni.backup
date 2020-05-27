namespace DNI.Backup.Services.FileList {
    public class ClientBackupConfig : DirectoryGlobSettings {
        public string Id { get; set; }

        public bool PreserveTree { get; set; }

        public bool DeleteDestinationFilesNotInSource { get; set; }

        public bool CompressBeforeTransfer { get; set; }
    }
}