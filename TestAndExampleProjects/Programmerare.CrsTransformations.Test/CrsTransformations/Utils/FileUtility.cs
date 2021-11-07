using System;
using System.IO;

namespace Programmerare.CrsTransformations.Test.CrsTransformations.Utils {
    
    public class FileUtility {

        private const int DefaultMaxNumberOfDirectoriesToNavigateUp = 8;

        public static string GetPathWithPlatformSeparator(
            string fileOrDirectoryPath
        ) {
            return fileOrDirectoryPath
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
        }

        public static FileInfo FindFile(
            string relativePathBelowSomeFileOrBaseDirectory,
            int maxNumberOfDirectoriesToNavigateUp = DefaultMaxNumberOfDirectoriesToNavigateUp
        ) {
            return FindFile(
                relativePathBelowSomeFileOrBaseDirectory,
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                maxNumberOfDirectoriesToNavigateUp 
            );
        }
        private static FileInfo FindFile(
            string relativePathBelowSomeFileOrBaseDirectory,
            string fullPathToSomeFile,
            int maxNumberOfDirectoriesToNavigateUp = DefaultMaxNumberOfDirectoriesToNavigateUp
        ) {
            return FindFileOrDirectoryHelper(
                relativePathBelowSomeFileOrBaseDirectory,
                fullPathToSomeFile,
                maxNumberOfDirectoriesToNavigateUp,
                path => File.Exists(path),
                path => new FileInfo(path)
            );
        }

        public static DirectoryInfo FindDirectory(
            string relativePathBelowSomeFileOrBaseDirectory,
            int maxNumberOfDirectoriesToNavigateUp = DefaultMaxNumberOfDirectoriesToNavigateUp
        ) {
            return FindDirectory(
                relativePathBelowSomeFileOrBaseDirectory,
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                maxNumberOfDirectoriesToNavigateUp 
            );
        }

        public static DirectoryInfo FindDirectory(
            string relativePathBelowSomeFileOrBaseDirectory,
            string fullPathToSomeDirectory,
            int maxNumberOfDirectoriesToNavigateUp = DefaultMaxNumberOfDirectoriesToNavigateUp
        ) {
            return FindFileOrDirectoryHelper(
                relativePathBelowSomeFileOrBaseDirectory,
                fullPathToSomeDirectory,
                maxNumberOfDirectoriesToNavigateUp,
                path => Directory.Exists(path),
                path => new DirectoryInfo(path)
            );
        }

        private static T FindFileOrDirectoryHelper<T>(
            string relativePathBelowSomeFileOrBaseDirectory,
            string fullPathToSomeDirectory,
            int maxNumberOfDirectoriesToNavigateUp,
            Func<string, bool> Exists,
            Func<string, T> Create
        ) where T: class {
            string relativePath = GetPathWithPlatformSeparator(relativePathBelowSomeFileOrBaseDirectory);
            string fullPathFileOrSubDirectory = GetPathWithPlatformSeparator(fullPathToSomeDirectory);
            int counterForUpNavigations = 0;
            while(counterForUpNavigations < maxNumberOfDirectoriesToNavigateUp) {
                string filePath = Path.Combine(fullPathFileOrSubDirectory, relativePath);
                if(Exists(filePath)) { // e.g. Directory.Exists(filePath)
                    var dir = Create(filePath);// e.g. new DirectoryInfo(filePath);
                    return dir;
                }
                counterForUpNavigations++; // below is the counted "up navigation" (the two dots is used for the parent directory)
                relativePath = ".." + Path.DirectorySeparatorChar + relativePath;
            }
            return null;
        }
    }

}