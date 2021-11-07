using NUnit.Framework;
using System.IO;

namespace Programmerare.CrsTransformations.Test.CrsTransformations.Utils {

    [TestFixture]
    public class FileUtilityTest {

        [Test]
        public void FindFile() {
            string relativePathBelowSomeBaseDirectory = "Programmerare.CrsTransformations.Core/CrsTransformations/Coordinate/CrsCoordinate.fs";
            var file = FileUtility.FindFile(relativePathBelowSomeBaseDirectory, 8); // with the current directory structure: 6 would be enough
            Assert.IsNotNull(file);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(file.FullName.EndsWith("CrsCoordinate.fs"));
        }


        [Test]
        public void FindDirectory() {
            string fullPathToSomeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string relativePathBelowSomeFileOrBaseDirectory = @"Programmerare.CrsTransformations.Core\CrsTransformations\Coordinate";
            var directory = FileUtility.FindDirectory(relativePathBelowSomeFileOrBaseDirectory, fullPathToSomeDirectory, 8); // with the current directory structure: 6 would be enough
            Assert.IsNotNull(directory);
            Assert.IsTrue(directory.Exists);
            Assert.IsTrue(directory.FullName.EndsWith("Coordinate"));
        }        
        
        [Test]
        public void GetPathWithPlatformSeparator() {
            char Separator = Path.DirectorySeparatorChar;
            string expected = Separator + "abc" + Separator + "def";
            Assert.AreEqual(
                expected,
                FileUtility.GetPathWithPlatformSeparator("/abc/def")
            );
            Assert.AreEqual(
                expected,
                FileUtility.GetPathWithPlatformSeparator(@"\abc\def")
            );
        }
    }
}
