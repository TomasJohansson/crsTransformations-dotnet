namespace Programmerare.CrsTransformations.CompositeTransformations {

using System;
using NUnit.Framework;
using System.Collections.Generic;
using Programmerare.CrsTransformations;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;

[TestFixture]
public class CrsTransformationAdapterCompositeFactoryTest {

    private const int EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS = CrsTransformationAdapterTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;

    private IList<ICrsTransformationAdapter> listOfAdaptersWithOneDuplicated, listOfTwoAdaptersWithoutDotSpatial;
    private IList<CrsTransformationAdapterWeight> listOfWeightsWithOneDuplicated;
        

    private CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;
    private CrsTransformationAdapterCompositeFactory compositeFactoryConfiguredWithLeafFactoryOnlyCreatingDotSpatialImplementationAsDefault;

    [SetUp]
    public void SetUp() {
        crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();

        var dotSpatial = new CrsTransformationAdapterDotSpatial();
        var projNet4GeoAPI = new CrsTransformationAdapterProjNet4GeoAPI();
        var mightyLittleGeodesy = new CrsTransformationAdapterMightyLittleGeodesy();
        listOfAdaptersWithOneDuplicated = new List<ICrsTransformationAdapter>{
            dotSpatial,
            projNet4GeoAPI,
            mightyLittleGeodesy,
            // Duplicate added below !
            new CrsTransformationAdapterDotSpatial()
        };

        listOfTwoAdaptersWithoutDotSpatial = new List<ICrsTransformationAdapter>{
            projNet4GeoAPI,
            mightyLittleGeodesy
        };

        listOfWeightsWithOneDuplicated = new List<CrsTransformationAdapterWeight>{
            CrsTransformationAdapterWeight.CreateFromInstance(dotSpatial, 1.0),
            CrsTransformationAdapterWeight.CreateFromInstance(projNet4GeoAPI, 2.0),
            CrsTransformationAdapterWeight.CreateFromInstance(mightyLittleGeodesy, 3.0),
            // Duplicate added below !
            // (Duplicate regarding the class, the weight value is not relevant)
            CrsTransformationAdapterWeight.CreateFromInstance(dotSpatial, 4.0)
        };

        CrsTransformationAdapterLeafFactory leafFactoryOnlyCreatingDotSpatialImplementationAsDefault = CrsTransformationAdapterLeafFactory.Create(new List<ICrsTransformationAdapter>{dotSpatial});
        compositeFactoryConfiguredWithLeafFactoryOnlyCreatingDotSpatialImplementationAsDefault = CrsTransformationAdapterCompositeFactory.Create(leafFactoryOnlyCreatingDotSpatialImplementationAsDefault);
    }


    [Test]
    public void createCrsTransformationAverage_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationAverage = crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationAverage);
    }

    [Test]
    public void createCrsTransformationMedian_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationMedian = crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationMedian);
    }

    [Test]
    public void createCrsTransformationFirstSuccess_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationFirstSuccess = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationFirstSuccess);
    }

    private void assertCompositeNotNullAndAggregatesManyImplementations(CrsTransformationAdapterComposite crsTransformationAdapterComposite) {
        Assert.IsNotNull(crsTransformationAdapterComposite);
        IList<ICrsTransformationAdapter> list = crsTransformationAdapterComposite._GetCompositeStrategy()._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked();
        Assert.AreEqual(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, list.Count);
    }

    private static IList<ICrsTransformationAdapter> emptyListOfCrsTransformationAdapters = new List<ICrsTransformationAdapter>();

    [Test]
    public void createCrsTransformationAverage_shouldThrowException_whenInstantiatingWithEmptyList()
    {
        helper_shouldThrowException_whenInstantiatingWithEmptyList(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage
        );
    }

    [Test]
    public void createCrsTransformationMedian_shouldThrowException_whenInstantiatingWithEmptyList() {
        helper_shouldThrowException_whenInstantiatingWithEmptyList(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian
        );
    }

    [Test]
    public void createCrsTransformationFirstSuccess_shouldThrowException_whenInstantiatingWithEmptyList() {
        helper_shouldThrowException_whenInstantiatingWithEmptyList(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess
        );
    }

    private void helper_shouldThrowException_whenInstantiatingWithEmptyList(
        Func<IList<ICrsTransformationAdapter>, CrsTransformationAdapterComposite> compositeCreator
    )
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            compositeCreator(emptyListOfCrsTransformationAdapters);
        });
        Assert.IsNotNull(exception);
        // the exception message might be something like:
        // 'Composite' adapter can not be created with an empty list of 'leaf' adapters
        // At least it should contain the word "empty" (is the assumption in the below test)
        Assert.That(exception.Message, Does.Contain("empty"));
    }

    [Test]
    public void CompositeFactory_ShouldThrowException_WhenDuplicateInListAndCreatingCompositeAverage() {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage(listOfAdaptersWithOneDuplicated);
        });        
    }

    [Test]
    public void CompositeFactory_ShouldThrowException_WhenDuplicateInListAndCreatingCompositeMedian() {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian(listOfAdaptersWithOneDuplicated);
        });        
    }

    [Test]
    public void CompositeFactory_ShouldThrowException_WhenDuplicateInListAndCreatingCompositeFirstSuccess() {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(listOfAdaptersWithOneDuplicated);
        });        
    }

    [Test]
    public void CompositeFactory_ShouldThrowException_WhenDuplicateInListAndCreatingCompositeWeightedAverage() {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(listOfWeightsWithOneDuplicated);
        });        
    }

    [Test]
    public void CompositeFactoryParameterLessMethod_ShouldOnlyReturnLeafsItIsConfiguredToKnowAbout() {
        var composite = compositeFactoryConfiguredWithLeafFactoryOnlyCreatingDotSpatialImplementationAsDefault.CreateCrsTransformationAverage();
        var children = composite.GetTransformationAdapterChildren();
        Assert.AreEqual(1, children.Count);
        Assert.AreEqual(
            typeof(CrsTransformationAdapterDotSpatial).FullName,
            children[0].GetType().FullName
        );
    }

    [Test]
    public void CompositeFactoryWithParameter_ShouldReturnCompositesWithLeafsInParameterListRegardlessOfTheListUsedAsDefault() {
        var composite = compositeFactoryConfiguredWithLeafFactoryOnlyCreatingDotSpatialImplementationAsDefault.CreateCrsTransformationAverage(
            listOfTwoAdaptersWithoutDotSpatial
        );
        var children = composite.GetTransformationAdapterChildren();
        Assert.AreEqual(2, children.Count);
        var listWithTheTwoExpectedClassNames = new List<string>{
            typeof(CrsTransformationAdapterProjNet4GeoAPI).FullName,
            typeof(CrsTransformationAdapterMightyLittleGeodesy).FullName
        };
        string classForChild1 = children[0].GetType().FullName;
        string classForChild2 = children[1].GetType().FullName;
        // The below tests verifies that BOTH (i.e. one of each)
        // above implementations are returned but is NOT 
        // depending on the order since a list is created above 
        // and then using 'Does.Contain below
        Assert.That(
            listWithTheTwoExpectedClassNames,
            Does.Contain(classForChild1)
        );
        Assert.That(
            listWithTheTwoExpectedClassNames,
            Does.Contain(classForChild2)
        );
        // The below test is just for asserting that 
        // we above did not get two of one and zero for the other 
        // i.e. we want to assert DIFFERENT implementations
        Assert.AreNotEqual(
            classForChild1,
            classForChild2
        );

    }



} // class ends
} // namespace ends