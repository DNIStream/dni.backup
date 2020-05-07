using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using FastRsync.Core;
using FastRsync.Delta;
using FastRsync.Diagnostics;
using FastRsync.Signature;

namespace DNI.Backup {
    // https://github.com/GrzegorzBlok/FastRsyncNet

    public class BackupService {
        public void Test() {
            // Data we need to store for each file: Full file path, last signature
            var sourceFilePath = @"D:\MediaProjects\obs\DNI-2020-04-08_12-57-08.mkv";
            var destinationFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\Game-2020-04-23_18-48-49.mkv";
            //var sourceFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test.txt";
            //var destinationFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test-diff.txt";
            var deltaFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test.delta.txt";
            var signatureFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test.sig.txt";

            // Check if "new file" exists, if not, just copy it
            if(!File.Exists(destinationFilePath)) {
                File.Copy(sourceFilePath, destinationFilePath);
                return;
            }

            // Machine A = Build the signature of the incoming file
            var signatureBuilder = new SignatureBuilder();
            using(var sourceFileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using(var signatureStream = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                    signatureBuilder.Build(sourceFileStream, new SignatureWriter(signatureStream));
                }
            }

            // Machine B - Produce the delta
            var deltaBuilder = new DeltaBuilder {
                ProgressReport = new ConsoleProgressReporter()
            };
            using(var sourceFileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using(var signatureStream = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using(var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                        deltaBuilder.BuildDelta(sourceFileStream, new SignatureReader(signatureStream, deltaBuilder.ProgressReport),
                            new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
                    }
                }
            }

            // Machine A = Patch remote file
            var deltaApplier = new DeltaApplier {
                SkipHashCheck = false
            };
            using(var sourceFileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using(var deltaStream = new FileStream(deltaFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using(var destinationFileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
                        deltaApplier.Apply(sourceFileStream, new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter()),
                            destinationFileStream);
                    }
                }
            }
        }
    }
}