using NUnit.Framework;
using Programmerare.CrsTransformations.CompositeTransformations;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations
{
[TestFixture]
class CrsTransformationAdapteeTypeTest {

    private CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;

    [SetUp]
    public void SetUp() {
        crsTransformationAdapterCompositeFactory = new CrsTransformationAdapterCompositeFactory();
    }

    [Test]
    public void ProjNet4GeoAPIAdapter_shouldMatchExpectedEnumAndAssemblyNameWithVersion() {
        var expectedFileInfoVersion = new FileInfoVersion(
            fileName: "projnet.dll",
            fileSize: 102912L, // netstandard2.0
            version: "1.4.1"
        );
        verifyExpectedEnumAndAssemblyVersion(
            new CrsTransformationAdapterProjNet4GeoAPI(),
            expectedFileInfoVersion,
            CrsTransformationAdapteeType.LEAF_PROJ_NET_4_GEO_API_1_4_1
        );
    }
    
    [Test]
    public void DotSpatialAdapter_shouldMatchExpectedEnumAndAssemblyNameWithVersion() {
        var expectedFileInfoVersion = new FileInfoVersion(
            fileName: "dotspatial.projections.dll",
            fileSize: 1538048L,
            version: "2.0.0-rc1"
        );
        verifyExpectedEnumAndAssemblyVersion(
            new CrsTransformationAdapterDotSpatial(),
            expectedFileInfoVersion,
            CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_2_0_0_RC1
        );
    }


    [Test]
    public void MightyLittleGeodesyAdapter_shouldMatchExpectedEnumAndAssemblyNameWithVersion() {
        FileInfoVersion expectedFileInfoVersion = new FileInfoVersion(
            fileName: "mightylittlegeodesy.dll", // .nuget\packages\mightylittlegeodesy\1.0.1\lib\net45
            fileSize: 15872L, // net45 version
            version: "1.0.1"
        );
        verifyExpectedEnumAndAssemblyVersion(
            new CrsTransformationAdapterMightyLittleGeodesy(),
            expectedFileInfoVersion,
            CrsTransformationAdapteeType.LEAF_SWEDISH_CRS_MLG_1_0_1
        );
    }

    private void verifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
        CrsTransformationAdapterBase crsTransformationAdapter,
        CrsTransformationAdapteeType expectedAdaptee
    ) {
        verifyExpectedEnumAndAssemblyVersion(
            crsTransformationAdapter,
            FileInfoVersion.FileInfoVersionNOTrepresentingThirdPartLibrary,
            expectedAdaptee
        );
    }

    private void verifyExpectedEnumAndAssemblyVersion(
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
    public void testCompositeAverage() {
        verifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage(),
            CrsTransformationAdapteeType.COMPOSITE_AVERAGE
        );
    }

    [Test]
    public void testCompositeMedian() {
        verifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian(),
            CrsTransformationAdapteeType.COMPOSITE_MEDIAN
        );
    }

    [Test]
    public void testCompositeFirstSuccess() {
        verifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(),
            CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS
        );
    }

    [Test]
    public void testCompositeWeightedAverage() {
        verifyExpectedEnumWhenNotRepresentingThirdPartLibrary(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(new List<CrsTransformationAdapterWeight>{
                CrsTransformationAdapterWeight.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), 1.0),
                CrsTransformationAdapterWeight.CreateFromInstance(new CrsTransformationAdapterProjNet4GeoAPI(), 2.0),
                CrsTransformationAdapterWeight.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), 3.0)
            }),
            CrsTransformationAdapteeType.COMPOSITE_WEIGHTED_AVERAGE
        );
    }

}
}