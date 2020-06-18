using System;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using Castle.Core;

using DNI.Backup.Model;
using DNI.Backup.Model.Settings;
using DNI.Backup.Services.Contracts;
using DNI.Backup.TestHelpers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Services.Tests {
    [Trait(TestTraits.TEST_TYPE, TestTraits.UNIT)]
    public class BackupSetResolverTests {
        private readonly ITestOutputHelper _output;

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        private readonly Mock<ILogger<BackupSetResolver>> _loggerMock;
        private readonly Mock<IOptionsMonitor<BackupSetSettings>> _backupSetSettingsMock;

        public BackupSetResolverTests(ITestOutputHelper _output) {
            this._output = _output;

            _loggerMock = Mock.Get(_fixture.Create<ILogger<BackupSetResolver>>());

            _backupSetSettingsMock = Mock.Get(_fixture.Create<IOptionsMonitor<BackupSetSettings>>());
        }

        private IBackupSetResolver GetService() {
            return new BackupSetResolver(_loggerMock.Object, _backupSetSettingsMock.Object);
        }

        [Fact]
        public async Task ThrowsException_WhenSetIdsAreNull() {
            // Arrange
            var setIds = (string[])null;

            var service = GetService();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => service.ResolveAsync(setIds));

            Assert.Equal("setIds", result.ParamName);
        }

        [Fact]
        public async Task ThrowsException_WhenSetIdsIsEmpty() {
            // Arrange
            var setIds = new string[0];

            var service = GetService();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentException>(() => service.ResolveAsync(setIds));

            Assert.Equal("setIds", result.ParamName);
        }

        [Fact]
        public async Task ReturnsNull_WhenIdsNotPresentInOptionsSet() {
            // Arrange
            var backupSettings = _fixture.Create<BackupSetSettings>();
            var setIds = _fixture.CreateMany<string>().ToArray();
            _backupSetSettingsMock
                .SetupGet(x => x.CurrentValue)
                .Returns(() => backupSettings);

            var service = GetService();

            // Act
            var result = await service.ResolveAsync(setIds);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnsSingleBackupSet_WhenOnlyOneSetIdExistsInConfig() {
            // Arrange
            var expectedId = _fixture.Create<string>();
            var backupSettings = _fixture.Create<BackupSetSettings>();
            var expectedSet = backupSettings.Sets[0];
            expectedSet.Id = expectedId;
            
            var setIds = _fixture.CreateMany<string>()
                .ToArray();
            setIds[2] = expectedId;

            _backupSetSettingsMock
                .SetupGet(x => x.CurrentValue)
                .Returns(() => backupSettings);

            var service = GetService();

            // Act
            var result = (await service.ResolveAsync(setIds))
                .ToArray();

            // Assert
            Assert.Single(result);
            Assert.Equal(expectedSet, result.First());
        }
    }
}