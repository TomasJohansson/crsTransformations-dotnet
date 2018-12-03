using Programmerare.CrsTransformations;

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;

[TestFixture]
public class CrsTransformationAdapterLeafFactoryTest {

    private CrsTransformationAdapterLeafFactory crsTransformationAdapterLeafFactory;

    private const int EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS = CrsTransformationAdapterTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;

    private static IList<string> actualClassNamesForAllKnownImplementations;
    
    [SetUp]
    public void SetUp() {
        crsTransformationAdapterLeafFactory = new CrsTransformationAdapterLeafFactory();
        actualClassNamesForAllKnownImplementations = new List<string> {
			typeof(CrsTransformationAdapterDotSpatial).FullName,
			typeof(CrsTransformationAdapterProjNet4GeoAPI).FullName,
			typeof(CrsTransformationAdapterMightyLittleGeodesy).FullName
        };
    }

    [Test]
    public void createCrsTransformationAdapter_shouldThrowException_whenTheParameterIsNotNameOfClassImplementingTheExpectedInterface() {
        string incorrectClassName = "abc";
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            crsTransformationAdapterLeafFactory.CreateCrsTransformationAdapter(incorrectClassName);
        });

        string nameOfInterfaceThatShouldBeImplemented = typeof(ICrsTransformationAdapter).FullName;
        Assert.IsNotNull(exception);
        string exceptionMessage = exception.Message;
        Assert.That(exceptionMessage, Does.Contain(nameOfInterfaceThatShouldBeImplemented));
        Assert.That(exceptionMessage, Does.StartWith("Failed to instantiate")); // Fragile but the message string will not change often and if it does change then it will be very easy to modify the string here
    }

    [Test]
    public void listOfNonClassNamesForAdapters_shouldNotBeRecognizedAsAdapters()
    {
        List<string> stringsNotBeingClassNameForAnyAdapter = new List<string>() {
            null,
            "",
            "  ",
            " x ",
            "abc",
            // this test class i.e. the below "this" does not imlpement the interface so therefore assertFalse below
            this.GetType().FullName
        };

        foreach (string stringNotBeingClassNameForAnyAdapter in stringsNotBeingClassNameForAnyAdapter)
        {
            Assert.IsFalse(
                crsTransformationAdapterLeafFactory.IsCrsTransformationAdapter(stringNotBeingClassNameForAnyAdapter),
                "Should not have been recognized as adapter : " + stringNotBeingClassNameForAnyAdapter
            );
        }
    }

    [Test]
    public void listOfHardcodedClassnames_shouldBeCrsTransformationAdapters()
    {
        IList<string> hardcodedClassNamesForAllKnownImplementations = crsTransformationAdapterLeafFactory.GetClassNamesForAllKnownImplementations();
        foreach (string hardcodedClassNameForKnownImplementation in hardcodedClassNamesForAllKnownImplementations)
        {
            Assert.IsTrue(
                crsTransformationAdapterLeafFactory.IsCrsTransformationAdapter(hardcodedClassNameForKnownImplementation),
                "Name of failing class: " + hardcodedClassNameForKnownImplementation
            );
        }
    }

    [Test]
    public void listOfHardcodedClassnames_shouldBeCreateableAsNonNullCrsTransformationAdapters()
    {
        IList<String> hardcodedClassNamesForAllKnownImplementations = crsTransformationAdapterLeafFactory.GetClassNamesForAllKnownImplementations();
        foreach (string hardcodedClassNameForKnownImplementation in hardcodedClassNamesForAllKnownImplementations)
        {
            ICrsTransformationAdapter crsTransformationAdapter = crsTransformationAdapterLeafFactory.CreateCrsTransformationAdapter(hardcodedClassNameForKnownImplementation);
			VerifyThatTheCreatedAdapterIsRealObject(crsTransformationAdapter);
			Assert.That(actualClassNamesForAllKnownImplementations, Contains.Item(crsTransformationAdapter.LongNameOfImplementation));
        }
    }

    private void VerifyThatTheCreatedAdapterIsRealObject(ICrsTransformationAdapter crsTransformationAdapter)
    {
        Assert.IsNotNull(crsTransformationAdapter);
        // below trying to use the created object to really make sure it works
        CrsCoordinate coordinateWgs84 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(59.330231, 18.059196, EpsgNumber.WORLD__WGS_84__4326);
        CrsTransformationResult resultSweref99 = crsTransformationAdapter.Transform(coordinateWgs84, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.IsNotNull(resultSweref99);
        Assert.IsTrue(resultSweref99.IsSuccess);
    }

    [Test]
    public void listOfKnownInstances_shouldOnlyContainNonNullObjectsAndTheNumberOfItemsShouldBeAtLeastFive()
    {
        IList<ICrsTransformationAdapter> list = crsTransformationAdapterLeafFactory.GetInstancesOfAllKnownAvailableImplementations();
        Assert.That(list.Count, Is.GreaterThanOrEqualTo(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS));
        foreach (ICrsTransformationAdapter crsTransformationAdapter in list)
        {
            VerifyThatTheCreatedAdapterIsRealObject(crsTransformationAdapter);
        }
    }

    [Test]
    public void listOfHardcodedClassnames_shouldCorrespondToActualClassNames()
    {
        IList<string> hardcodedClassNamesForAllKnownImplementations = crsTransformationAdapterLeafFactory.GetClassNamesForAllKnownImplementations();
        Assert.AreEqual(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, hardcodedClassNamesForAllKnownImplementations.Count);
        Assert.AreEqual(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, actualClassNamesForAllKnownImplementations.Count);

        foreach (string actualClassNameForAnImplementation in actualClassNamesForAllKnownImplementations)
        {
            Assert.That(hardcodedClassNamesForAllKnownImplementations, Contains.Item(actualClassNameForAnImplementation));
        }
        foreach (string hardcodedClassNamesForAllKnownImplementation in hardcodedClassNamesForAllKnownImplementations)
        {
            Assert.That(actualClassNamesForAllKnownImplementations, Contains.Item(hardcodedClassNamesForAllKnownImplementation));
        }
    }
}