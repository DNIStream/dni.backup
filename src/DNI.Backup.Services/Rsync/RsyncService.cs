using System;
using System.IO;
using System.Threading.Tasks;

using FastRsync.Core;
using FastRsync.Delta;
using FastRsync.Diagnostics;
using FastRsync.Signature;

namespace DNI.Backup.Services.Rsync {
    public class RsyncService : IRsyncService {
        /// <summary>
        ///     Creates a signature for the specified <paramref name="inputFilePath" /> and returns it as a
        ///     <see cref="MemoryStream" />. Intended for use by the destination backup server service to generate a signature for
        ///     destination files.
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public async Task<MemoryStream> CreateSignatureAsync(string inputFilePath) {
            if(string.IsNullOrWhiteSpace(inputFilePath)) {
                throw new ArgumentNullException(nameof(inputFilePath));
            }

            var signatureBuilder = new SignatureBuilder();
            await using(var inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var signatureStream = new MemoryStream();
                await signatureBuilder.BuildAsync(inputFileStream, new SignatureWriter(signatureStream));
                signatureStream.Seek(0, SeekOrigin.Begin);
                return signatureStream;
            }
        }

        /// <summary>
        ///     Creates a delta file for the specified <paramref name="inputFilePath" />, using the signature supplied by the
        ///     requester. Intended for use by the source workstation client to generate delta files to be sent to the server for
        ///     patching.
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="signatureStream"></param>
        /// <returns></returns>
        public async Task<MemoryStream> CreateDeltaAsync(string inputFilePath, Stream signatureStream) {
            if(string.IsNullOrWhiteSpace(inputFilePath)) {
                throw new ArgumentNullException(nameof(inputFilePath));
            }

            if(signatureStream == null) {
                throw new ArgumentNullException(nameof(signatureStream));
            }

            var deltaBuilder = new DeltaBuilder {
                ProgressReport = new ConsoleProgressReporter()
            };
            await using(var inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var deltaStream = new MemoryStream();
                deltaBuilder.BuildDelta(inputFileStream, new SignatureReader(signatureStream, deltaBuilder.ProgressReport),
                    new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
                deltaStream.Seek(0, SeekOrigin.Begin);
                return deltaStream;
            }
        }

        /// <summary>
        ///     Applies the specified <paramref name="deltaStream" /> to the specified <paramref name="inputFilePath" />. Intended
        ///     for use by the destination backup server to finalise the individual file backup.
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="deltaStream"></param>
        /// <returns></returns>
        public async Task<bool> ApplyDeltaAsync(string inputFilePath, Stream deltaStream) {
            if(string.IsNullOrWhiteSpace(inputFilePath)) {
                throw new ArgumentNullException(nameof(inputFilePath));
            }

            if(deltaStream == null) {
                throw new ArgumentNullException(nameof(deltaStream));
            }

            var deltaApplier = new DeltaApplier {
                SkipHashCheck = false
            };

            var tempFileName = string.Concat(Path.GetFileName(inputFilePath), ".transfer");
            var tempDestinationFilePath = Path.Join(Path.GetDirectoryName(inputFilePath), tempFileName);
            await using(var destinationFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                await using(var newDestinationFileStream =
                    new FileStream(tempDestinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
                    await deltaApplier.ApplyAsync(destinationFileStream, new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter()),
                        newDestinationFileStream);
                }
            }

            // Overwrite the destination file with the newly patched file
            File.Copy(tempDestinationFilePath, inputFilePath, true);

            // Delete the temp file
            File.Delete(tempDestinationFilePath);

            return true;
        }
    }
}