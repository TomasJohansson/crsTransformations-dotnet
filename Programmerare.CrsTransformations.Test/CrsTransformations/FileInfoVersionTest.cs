using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Programmerare.CrsTransformations
{
    [TestFixture]
    class FileInfoVersionTest {
        [Test]
        public void GetFileInfoVersion_ShouldThrowException_WhenTheTypeIsNotRetrivedFromNuGet() {
            // In the test below, the type of "this" is used, i.e. 
            // this test class which is not going to be deployed 
            // as a NuGet assembly and therefore it should fail.

            Assert.That(
                () => FileInfoVersion.GetFileInfoVersionHelper(this.GetType())
                ,
                Throws.Exception
            );
        }

        [Test]
        public void GetFileInfoVersion_ShouldSuccess_WhenTheTypeIsRetrivedFromNuGet() {
            // In the succesful scenario, a type from an assembly is used,
            // and then the following kind of path will be retrieved:
            // "...nuget/packages/mightylittlegeodesy/1.0.1/lib/net45/MightyLittleGeodesy.dll"
            // and from that kind of path, the version "1.0.1" should become extracted by using 
            // a regular expression.
            // The below test will use NUnit 

            Type typeWithinTheNUnitFramework = typeof(Assert);
            FileInfoVersion fileInfoVersion = FileInfoVersion.GetFileInfoVersionHelper(typeWithinTheNUnitFramework);
            Assert.IsNotNull(fileInfoVersion);
            Assert.AreEqual("nunit.framework.dll", fileInfoVersion.FileName);
            // The above filename will very likely rename the same after an upgrade 
            // but the below will change and there is no need 
            // to assert the below values in regression testing
            //Assert.AreEqual("3.11.0", fileInfoVersion.Version);
            //Assert.AreEqual(380416L, fileInfoVersion.FileSize);

            Console.WriteLine(typeWithinTheNUnitFramework.Assembly.CodeBase);
            // output example from the above code row:
            // ".../.nuget/packages/nunit/3.11.0/lib/netstandard2.0/nunit.framework.dll"
            // and from that kind of path "3.11.0" can be extracted as the version

            // See also the test class CrsTransformationAdapteeTypeTest
            // which is the real use case for the FileInfoVersion,
            // i.e. those tests are used for helping to remember to update 
            // the tested enum if an upgrade has been made.
        }
    }
}
