using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Test.Services {
    public class BackupServiceTests {
        private readonly ITestOutputHelper _output;

        public BackupServiceTests(ITestOutputHelper _output) {
            this._output = _output;
        }

        //[Fact]
        [Fact(Skip = "TBC")]
        public async Task BackupTest() {
            // Arrange
            var backupService = new BackupService();

            // Act
            backupService.Test();

            // Assert
        }
    }
}