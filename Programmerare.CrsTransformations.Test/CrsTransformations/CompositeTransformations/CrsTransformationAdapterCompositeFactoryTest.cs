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

    private IList<ICrsTransformationAdapter> listOfAdaptersWithOneDuplicated;
    private IList<CrsTransformationAdapterWeight> listOfWeightsWithOneDuplicated;

    private CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;

    [SetUp]
    public void SetUp() {
        crsTransformationAdapterCompositeFactory = new CrsTransformationAdapterCompositeFactory();

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

        listOfWeightsWithOneDuplicated = new List<CrsTransformationAdapterWeight>{
            CrsTransformationAdapterWeight.CreateFromInstance(dotSpatial, 1.0),
            CrsTransformationAdapterWeight.CreateFromInstance(projNet4GeoAPI, 2.0),
            CrsTransformationAdapterWeight.CreateFromInstance(mightyLittleGeodesy, 3.0),
            // Duplicate added below !
            // (Duplicate regarding the class, the weight value is not relevant)
            CrsTransformationAdapterWeight.CreateFromInstance(dotSpatial, 4.0)
        };
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

        //




} // class ends
} // namespace ends