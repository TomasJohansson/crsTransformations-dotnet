using Programmerare.CrsTransformations;

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_7;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;

[TestFixture]
public class CrsTransformationAdapterLeafFactoryTest {

    private CrsTransformationAdapterLeafFactory crsTransformationAdapterLeafFactory;

    private const int EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS = CrsTransformationAdapterTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;

    private static IList<string> actualClassNamesForAllKnownImplementations;

    private CrsTransformationAdapterLeafFactory factoryWithOnlyTheTwoLeafsDotSpatialAndProjNet;
    private string classNameDotSpatial, classNameProjNet, classNameMightyLittleGeodesy;
    
    [SetUp]
    public void SetUp() {
        classNameDotSpatial = typeof(CrsTransformationAdapterDotSpatial).FullName;
        classNameProjNet = typeof(CrsTransformationAdapterProjNet).FullName;
        classNameMightyLittleGeodesy = typeof(CrsTransformationAdapterMightyLittleGeodesy).FullName;

        factoryWithOnlyTheTwoLeafsDotSpatialAndProjNet = CrsTransformationAdapterLeafFactory.Create(
            new List<ICrsTransformationAdapter>{
                new CrsTransformationAdapterDotSpatial(),
                new CrsTransformationAdapterProjNet()
            }
        );

        crsTransformationAdapterLeafFactory = CrsTransformationAdapterLeafFactory.Create();
        actualClassNamesForAllKnownImplementations = new List<string> {
			typeof(CrsTransformationAdapterDotSpatial).FullName,
			typeof(CrsTransformationAdapterProjNet).FullName,
			typeof(CrsTransformationAdapterMightyLittleGeodesy).FullName
        };
    }

    [Test]
    public void GetCrsTransformationAdapter_ShouldThrowException_WhenTheParameterIsNotNameOfClassImplementingTheExpectedInterface() {
        string incorrectClassName = "abc";
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            crsTransformationAdapterLeafFactory.GetCrsTransformationAdapter(incorrectClassName);
        });

        string nameOfInterfaceThatShouldBeImplemented = typeof(ICrsTransformationAdapter).FullName;
        Assert.IsNotNull(exception);
        string exceptionMessage = exception.Message;
        Assert.That(exceptionMessage, Does.Contain(nameOfInterfaceThatShouldBeImplemented));
        // Fragile test below but the message string will not change often 
        // and if it does change then it will be very easy to modify the string here
        Assert.That(
            exceptionMessage, 
            Does.StartWith("Failed to return an instance")
        );
    }

    [Test]
    public void ListOfNonClassNamesForAdapters_ShouldNotBeRecognizedAsAdapters() {
        List<string> stringsNotBeingClassNameForAnyAdapter = new List<string>() {
            null,
            "",
            "  ",
            " x ",
            "abc",
            // this test class i.e. the below "this" does not implement 
            // the interface so therefore 'Assert.IsFalse' below
            this.GetType().FullName
        };

        foreach (string stringNotBeingClassNameForAnyAdapter in stringsNotBeingClassNameForAnyAdapter) {
            Assert.IsFalse(
                crsTransformationAdapterLeafFactory.IsCrsTransformationAdapter(
                    stringNotBeingClassNameForAnyAdapter
                )
                ,
                "Should not have been recognized as adapter : " + stringNotBeingClassNameForAnyAdapter
            );
        }
    }

    [Test]
    public void ListOfHardcodedClassnames_ShouldBeCrsTransformationAdapters() {
        IList<string> hardcodedClassNamesForAllKnownImplementations = crsTransformationAdapterLeafFactory.GetClassNamesForAllImplementations();
        foreach (string hardcodedClassNameForKnownImplementation in hardcodedClassNamesForAllKnownImplementations) {
            Assert.IsTrue(
                crsTransformationAdapterLeafFactory.IsCrsTransformationAdapter(
                    hardcodedClassNameForKnownImplementation
                )
                ,
                "Name of failing class: " + hardcodedClassNameForKnownImplementation
            );
        }
    }

    [Test]
    public void ListOfHardcodedClassnames_ShouldBeCreateableAsNonNullCrsTransformationAdapters() {
        IList<String> hardcodedClassNamesForAllKnownImplementations = crsTransformationAdapterLeafFactory.GetClassNamesForAllImplementations();
        foreach (string hardcodedClassNameForKnownImplementation in hardcodedClassNamesForAllKnownImplementations) {
            ICrsTransformationAdapter crsTransformationAdapter = crsTransformationAdapterLeafFactory.GetCrsTransformationAdapter(
                hardcodedClassNameForKnownImplementation
            );
			VerifyThatTheCreatedAdapterIsRealObject(crsTransformationAdapter);
			Assert.That(
                actualClassNamesForAllKnownImplementations, 
                Contains.Item(
                    crsTransformationAdapter.LongNameOfImplementation
                )
            );
        }
    }

    private void VerifyThatTheCreatedAdapterIsRealObject(
        ICrsTransformationAdapter crsTransformationAdapter
    ) {
        Assert.IsNotNull(crsTransformationAdapter);
        // below trying to use the created object to really make sure it works
        CrsCoordinate coordinateWgs84 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(59.330231, 18.059196, EpsgNumber.WORLD__WGS_84__4326);
        CrsTransformationResult resultSweref99 = crsTransformationAdapter.Transform(coordinateWgs84, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.IsNotNull(resultSweref99);
        Assert.IsTrue(resultSweref99.IsSuccess);
    }

    [Test]
    public void ListOfKnownInstances_ShouldOnlyContainNonNullObjectsAndShouldContainAtLeastACertainNumberOfItems() {
        IList<ICrsTransformationAdapter> list = crsTransformationAdapterLeafFactory.GetInstancesOfAllImplementations();
        Assert.That(
            list.Count, 
            Is.GreaterThanOrEqualTo(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS)
        );
        foreach (ICrsTransformationAdapter crsTransformationAdapter in list) {
            VerifyThatTheCreatedAdapterIsRealObject(crsTransformationAdapter);
        }
    }

    [Test]
    public void ListOfHardcodedClassnames_ShouldCorrespondToActualClassNames() {
        IList<string> hardcodedClassNamesForAllKnownImplementations = crsTransformationAdapterLeafFactory.GetClassNamesForAllImplementations();
        Assert.AreEqual(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, hardcodedClassNamesForAllKnownImplementations.Count);
        Assert.AreEqual(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, actualClassNamesForAllKnownImplementations.Count);

        foreach (string actualClassNameForAnImplementation in actualClassNamesForAllKnownImplementations) {
            Assert.That(
                hardcodedClassNamesForAllKnownImplementations, 
                Contains.Item(actualClassNameForAnImplementation)
            );
        }
        foreach (string hardcodedClassNamesForAKnownImplementation in hardcodedClassNamesForAllKnownImplementations) {
            Assert.That(
                actualClassNamesForAllKnownImplementations, 
                Contains.Item(hardcodedClassNamesForAKnownImplementation)
            );
        }
    }

    [Test]
    public void FactoryWithOnlyTwoLeafs_ShouldOnlyContainClassesDotSpatialAndProjNet() {
        IList<string> allClassNames = factoryWithOnlyTheTwoLeafsDotSpatialAndProjNet.GetClassNamesForAllImplementations();
        Assert.AreEqual(2, allClassNames.Count);
        Assert.That(allClassNames, Does.Contain(classNameDotSpatial));
        Assert.That(allClassNames, Does.Contain(classNameProjNet));
        Assert.That(allClassNames, Does.Not.Contain(classNameMightyLittleGeodesy));
    }


    [Test]
    public void FactoryWithOnlyTwoLeafs_ShouldOnlyContainInstancesOfDotSpatialAndProjNet() {
        IList<ICrsTransformationAdapter> allAdapters = factoryWithOnlyTheTwoLeafsDotSpatialAndProjNet.GetInstancesOfAllImplementations();
        Assert.AreEqual(2, allAdapters.Count);
        Assert.That(allAdapters, Does.Contain(new CrsTransformationAdapterDotSpatial()));
        Assert.That(allAdapters, Does.Contain(new CrsTransformationAdapterProjNet()));
        Assert.That(allAdapters, Does.Not.Contain(new CrsTransformationAdapterMightyLittleGeodesy()));
    }

    [Test]
    public void FactoryWithOnlyTwoLeafs_ShouldOnlyRecognizeDotSpatialAndProjNet() {
        Assert.IsTrue(
            factoryWithOnlyTheTwoLeafsDotSpatialAndProjNet.IsCrsTransformationAdapter(
                classNameDotSpatial
            )
        );
        Assert.IsTrue(
            factoryWithOnlyTheTwoLeafsDotSpatialAndProjNet.IsCrsTransformationAdapter(
                classNameProjNet
            )
        );

        Assert.IsFalse(
            factoryWithOnlyTheTwoLeafsDotSpatialAndProjNet.IsCrsTransformationAdapter(
                classNameMightyLittleGeodesy
            )
        );
        // false above for MightyLittleGeodesy
        // for the factory with only two leafs
        // and below just testing the same MightyLittleGeodesy
        // but for the factory with all leafs 
        // to verify it is true
        Assert.IsTrue(
            crsTransformationAdapterLeafFactory.IsCrsTransformationAdapter(
                classNameMightyLittleGeodesy
            )
        );
    }
}