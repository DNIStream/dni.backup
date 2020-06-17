namespace DNI.Backup.Model {
    public class BackupSet : DirectoryGlob {
        public string Id { get; set; }

        public bool PreserveTree { get; set; }

        public bool DeleteDestinationFilesNotInSource { get; set; }

        public bool CompressBeforeTransfer { get; set; }
    }
}