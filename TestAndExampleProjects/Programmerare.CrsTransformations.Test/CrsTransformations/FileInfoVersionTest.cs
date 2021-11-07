using System;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Core {

    [TestFixture]
    class FileInfoVersionTest {

        [Test]
        public void GetFileInfoVersionHelper() {
            // In the test below, the type of "this" is used, i.e. 
            // this test class which is not going to be deployed/released, 
            // and therefore no reason to change it to anything else but 1.0.0.0 in the project file <AssemblyVersion>1.0.0.0</AssemblyVersion>
            var fileInfoVersion = FileInfoVersion.GetFileInfoVersionHelper(this.GetType());
            Assert.AreEqual("Programmerare.CrsTransformations.Test.dll", fileInfoVersion.FileName);
            Assert.AreEqual("1.0.0.0", fileInfoVersion.Version);
        }

        [Test]
        public void GetFileInfoVersion_ShouldSuccess_WhenTheTypeIsRetrivedFromNuGet() {
            // In the succesful scenario, a type from an assembly is used,
            // and then the following kind of path will be retrieved:
            // "...nuget/packages/mightylittlegeodesy/1.0.2/lib/net45/MightyLittleGeodesy.dll"
            // and from that kind of path, the version "1.0.2" should become extracted by using 
            // a regular expression.
            // The below test will use NUnit 

            Type typeWithinTheNUnitFramework = typeof(Assert);
            FileInfoVersion fileInfoVersion = FileInfoVersion.GetFileInfoVersionHelper(typeWithinTheNUnitFramework);
            Assert.IsNotNull(fileInfoVersion);
            Assert.AreEqual("nunit.framework.dll", fileInfoVersion.FileName);
            // The above filename will very likely remain the same after an upgrade 
            // but the code disabled below would change and there is no need 
            // to assert the below values in regression testing
            //Assert.AreEqual("3.11.0", fileInfoVersion.Version);
            //Assert.AreEqual(380416L, fileInfoVersion.FileSize);

            //Console.WriteLine(typeWithinTheNUnitFramework.Assembly.CodeBase);
            // output example from the above code row:
            // ".../.nuget/packages/nunit/3.11.0/lib/netstandard2.0/nunit.framework.dll"
            // and from that kind of path "3.11.0" can be extracted as the version

            // See also the test class CrsTransformationAdapteeTypeTest
            // which is the real use case for the FileInfoVersion,
            // i.e. those tests are used for helping to remember to update 
            // the tested enum if an upgrade has been made.
        }

        [Test]
        public void FileInfoVersion_ShouldBeEqual_WhenAllPropertiesAreEqual() {
            var f1 = new FileInfoVersion("", -1L, "");
            var f2 = new FileInfoVersion("", -1L, "");
            // The above two should be equal but the rest below should be different
            var f3 = new FileInfoVersion("a", -1L, "");
            var f4 = new FileInfoVersion("", -1L, "a");
            var f5 = new FileInfoVersion("", -2L, "");
            var f6 = new FileInfoVersion("", 0, "");
            var f7 = new FileInfoVersion("", 1, "");
            var f8 = new FileInfoVersion(" ", -1L, ""); // space
            var f9 = new FileInfoVersion("", -1L, " "); // space
            
            Assert.AreEqual(f1, f2);
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());

            Assert.AreNotEqual(f1, f3);
            Assert.AreNotEqual(f1, f4);
            Assert.AreNotEqual(f1, f5);
            Assert.AreNotEqual(f1, f6);
            Assert.AreNotEqual(f1, f7);
            Assert.AreNotEqual(f1, f8);
            Assert.AreNotEqual(f1, f9);
        }

        [Test]
        public void IsRepresentingThirdPartLibrary_ShouldBeTrue_WhenUsingInstanceRepresentingNonThirdPartLibrary() {
            var instanceRepresentingNonThirdPartLibrary = FileInfoVersion.FileInfoVersionNOTrepresentingThirdPartLibrary;
            Assert.IsFalse(instanceRepresentingNonThirdPartLibrary.IsRepresentingThirdPartLibrary());

            var someOtherInstanceWithDifferentPropertyValues = new FileInfoVersion("a", 123, "b");
            Assert.IsTrue(someOtherInstanceWithDifferentPropertyValues.IsRepresentingThirdPartLibrary());
        }
    }
}