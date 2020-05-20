using System;
using System.IO;
using System.Threading.Tasks;

using FastRsync.Signature;

namespace DNI.Backup.Services.Rsync {
    public class RsyncService : IRsyncService {
        /// <summary>
        ///     Creates a signature for the specified <paramref name="inputFilePath" /> and returns it as a
        ///     <see cref="MemoryStream" />. Intended for use by the destination backup server service to generate a signature for
        ///     destination
        ///     files.
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public async Task<MemoryStream> CreateSignature(string inputFilePath) {
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
        public Task<MemoryStream> CreateDelta(string inputFilePath, Stream signatureStream) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Applies the specified <paramref name="deltaStream" /> to the specified <paramref name="inputFilePath" />. Intended
        ///     for use by the destination backup server to finalise the individual file backup.
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="deltaStream"></param>
        /// <returns></returns>
        public Task<MemoryStream> ApplyDelta(string inputFilePath, Stream deltaStream) {
            throw new NotImplementedException();
        }
    }
}