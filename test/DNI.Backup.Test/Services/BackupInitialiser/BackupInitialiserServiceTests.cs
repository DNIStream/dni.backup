using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Backup.Services.BackupInitialiser;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Test.Services.BackupInitialiser {
    [Trait(TestTraits.TEST_TYPE, TestTraits.UNIT)]
    public class BackupInitialiserServiceTests {
        private readonly ITestOutputHelper _output;

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public BackupInitialiserServiceTests(ITestOutputHelper _output) {
            this._output = _output;
        }

        private IClientBackupInitialiserService GetService() {
            return new ClientBackupInitialiserService();
        }

        [Fact]
        public async Task Test() {
            // Arrange
            var service = GetService();

            // Act

            // Assert
        }
    }
}