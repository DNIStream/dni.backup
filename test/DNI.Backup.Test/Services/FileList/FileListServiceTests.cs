using System;
using System.Threading.Tasks;

using DNI.Backup.Services;
using DNI.Backup.Services.FileList;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Test.Services {
    public class FileListServiceTests {
        private readonly ITestOutputHelper _output;

        public FileListServiceTests(ITestOutputHelper _output) {
            this._output = _output;
        }

        private IFileListService GetService() {
            return new FileListService();
        }

        [Fact]
        public async Task GetFiles_ThrowsException_WhenBackupDirectorySettings_IsNull() {
            // Arrange
            var service = GetService();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetFilesAsync(null));
            Assert.Equal("backupDirectorySettings", result.ParamName);
        }

        [Fact]
        public async Task GetFiles_ThrowsException_WhenBackupDirectorySettings_ContainsEmptyEnumerable() {
            // Arrange
            var service = GetService();
            var settings = new BackupDirectorySetting[0];

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentException>(() => service.GetFilesAsync(settings));
            Assert.Equal("backupDirectorySettings", result.ParamName);
        }

    }
}