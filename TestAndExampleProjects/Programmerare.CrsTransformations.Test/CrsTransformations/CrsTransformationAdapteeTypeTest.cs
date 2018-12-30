using NUnit.Framework;
using Programmerare.CrsTransformations.CompositeTransformations;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations {
[TestFixture]
class CrsTransformationAdapteeTypeTest {

    private CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;
    private CrsTransformationAdapterWeightFactory weightFactory;

    [SetUp]
    public void SetUp() {
        weightFactory = CrsTransformationAdapterWeightFactory.Create();
        crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();
    }

    [Test]
    public void ProjNet4GeoAPIAdapter_ShouldMatchExpectedEnumAndAssemblyNameWithVersion() {
        var expectedFileInfoVersion = new FileInfoVersion(
            fileName: "projnet.dll",
            fileSize: 102912L, // netstandard2.0 filesize
            version: "1.4.1"
        );
        VerifyExpectedEnumAndAssemblyVersion(
            new CrsTransformationAdapterProjNet4GeoAPI(),
            expectedFileInfoVersion,
            CrsTransformationAdapteeType.LEAF_PROJ_NET_4_GEO_API_1_4_1
        );
    }
    
    [Test]
    public void DotSpatialAdapter_ShouldMatchExpectedEnumAndAssemblyNameWithVersion() {
        var expectedFileInfoVersion = new FileInfoVersion(
            fileName: "dotspatial.projections.dll",
            fileSize: 1538048L,
            version: "2.0.0-rc1"
        );
        VerifyExpectedEnumAndAssemblyVersion(
            new CrsTransformationAdapterDotSpatial(),
            expectedFileInfoVersion,
            CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_2_0_0_RC1
        );
    }


    [Test]
    public void MightyLittleGeodesyAdapter_ShouldMatchExpectedEnumAndAssemblyNameWithVersion() {
        FileInfoVersion expectedFileInfoVersion = new FileInfoVersion(
            fileName: "mightylittlegeodesy.dll", // .nuget\packages\mightylittlegeodesy\1.0.1\lib\net45
            fileSize: 15872L, // net45 version
            version: "1.0.1"
        );
        VerifyExpectedEnumAndAssemblyVersion(
            new CrsTransformationAdapterMightyLittleGeodesy(),
            expectedFileInfoVersion,
            CrsTransformationAdapteeType.LEAF_MIGHTY_LITTLE_GEODESY_1_0_1
        );
    }

    private void VerifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
        CrsTransformationAdapterBase crsTransformationAdapter,
        CrsTransformationAdapteeType expectedAdaptee
    ) {
        VerifyExpectedEnumAndAssemblyVersion(
            crsTransformationAdapter,
            FileInfoVersion.FileInfoVersionNOTrepresentingThirdPartLibrary,
            expectedAdaptee
        );
    }

    private void VerifyExpectedEnumAndAssemblyVersion(
        CrsTransformationAdapterBase crsTransformationAdapter,
        FileInfoVersion expectedFileInfoVersion,
        CrsTransformationAdapteeType expectedEnumWithMatchingNameInlcudingVersionNumber
    ) {
        Assert.AreEqual(
            expectedEnumWithMatchingNameInlcudingVersionNumber, 
            crsTransformationAdapter.AdapteeType
        );
        FileInfoVersion fileInfoVersion = crsTransformationAdapter._GetFileInfoVersion();
        if(expectedFileInfoVersion.IsRepresentingThirdPartLibrary()) {
            Assert.That(
                fileInfoVersion.FileName, Does.EndWith(expectedFileInfoVersion.FileName),
                "Likely failure reason: You have upgraded a version. If so, then upgrade both the enum value and the filename"
            );
            Assert.AreEqual(
                // This test is actually quite fragile for example 
                // regarding the .NET version 
                // (e.g. "net45" or "netstandard2.0")
                // but remember that the purpose is to detect 
                // that a library (i.e. the binary file e.g. file size)
                // has been modified and then maybe the enum specifying 
                // the version should be updated
                expectedFileInfoVersion.FileSize,
                fileInfoVersion.FileSize
            );

            Assert.AreEqual(
                expectedFileInfoVersion.Version,
                fileInfoVersion.Version
            );
        }
    }

    [Test]
    public void TestCompositeAverage() {
        VerifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage(),
            CrsTransformationAdapteeType.COMPOSITE_AVERAGE
        );
    }

    [Test]
    public void TestCompositeMedian() {
        VerifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian(),
            CrsTransformationAdapteeType.COMPOSITE_MEDIAN
        );
    }

    [Test]
    public void TestCompositeFirstSuccess() {
        VerifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(),
            CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS
        );
    }

    [Test]
    public void TestCompositeWeightedAverage() {
        VerifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(new List<CrsTransformationAdapterWeight>{
                weightFactory.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), 1.0),
                weightFactory.CreateFromInstance(new CrsTransformationAdapterProjNet4GeoAPI(), 2.0),
                weightFactory.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), 3.0)
            }),
            CrsTransformationAdapteeType.COMPOSITE_WEIGHTED_AVERAGE
        );
    }

}
}