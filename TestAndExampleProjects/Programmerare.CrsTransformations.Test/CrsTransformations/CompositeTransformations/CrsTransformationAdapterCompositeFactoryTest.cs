namespace Programmerare.CrsTransformations.Core.CompositeTransformations {

using System;
using NUnit.Framework;
using System.Collections.Generic;
using Programmerare.CrsTransformations;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.CompositeTransformations;

    [TestFixture]
public class CrsTransformationAdapterCompositeFactoryTest {

    private const int EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS = CrsTransformationAdapterTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;

    private IList<ICrsTransformationAdapter> listOfAdaptersWithOneDuplicated, listOfTwoAdaptersWithoutDotSpatial;
    private IList<CrsTransformationAdapterWeight> listOfWeightsWithOneDuplicated;
        

    private CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;
    private CrsTransformationAdapterCompositeFactory compositeFactoryConfiguredWithLeafFactoryOnlyCreatingDotSpatialImplementationAsDefault;
    private CrsTransformationAdapterWeightFactory weightFactory;

    [SetUp]
    public void SetUp() {
        weightFactory = CrsTransformationAdapterWeightFactory.Create();
        crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();

        var dotSpatial = new CrsTransformationAdapterDotSpatial();
        var ProjNet = new CrsTransformationAdapterProjNet();
        var mightyLittleGeodesy = new CrsTransformationAdapterMightyLittleGeodesy();
        listOfAdaptersWithOneDuplicated = new List<ICrsTransformationAdapter>{
            dotSpatial,
            ProjNet,
            mightyLittleGeodesy,
            // Duplicate added below !
            new CrsTransformationAdapterDotSpatial()
        };

        listOfTwoAdaptersWithoutDotSpatial = new List<ICrsTransformationAdapter>{
            ProjNet,
            mightyLittleGeodesy
        };

        listOfWeightsWithOneDuplicated = new List<CrsTransformationAdapterWeight>{
            weightFactory.CreateFromInstance(dotSpatial, 1.0),
            weightFactory.CreateFromInstance(ProjNet, 2.0),
            weightFactory.CreateFromInstance(mightyLittleGeodesy, 3.0),
            // Duplicate added below !
            // (Duplicate regarding the class, the weight value is not relevant)
            weightFactory.CreateFromInstance(dotSpatial, 4.0)
        };

        CrsTransformationAdapterLeafFactory leafFactoryOnlyCreatingDotSpatialImplementationAsDefault = CrsTransformationAdapterLeafFactory.Create(new List<ICrsTransformationAdapter>{dotSpatial});
        compositeFactoryConfiguredWithLeafFactoryOnlyCreatingDotSpatialImplementationAsDefault = CrsTransformationAdapterCompositeFactory.Create(leafFactoryOnlyCreatingDotSpatialImplementationAsDefault);
    }


    [Test]
    public void CreateCrsTransformationAverage_ShouldBeCreatedWithManyImplementations_WhenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationAverage = crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage();
        AssertCompositeNotNullAndAggregatesManyImplementations(crsTransformationAverage);
    }

    [Test]
    public void CreateCrsTransformationMedian_ShouldBeCreatedWithManyImplementations_WhenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationMedian = crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian();
        AssertCompositeNotNullAndAggregatesManyImplementations(crsTransformationMedian);
    }

    [Test]
    public void CreateCrsTransformationFirstSuccess_ShouldBeCreatedWithManyImplementations_WhenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationFirstSuccess = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess();
        AssertCompositeNotNullAndAggregatesManyImplementations(crsTransformationFirstSuccess);
    }

    private void AssertCompositeNotNullAndAggregatesManyImplementations(CrsTransformationAdapterComposite crsTransformationAdapterComposite) {
        Assert.IsNotNull(crsTransformationAdapterComposite);
        IList<ICrsTransformationAdapter> list = crsTransformationAdapterComposite._GetCompositeStrategy()._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked();
        Assert.AreEqual(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, list.Count);
    }

    private static IList<ICrsTransformationAdapter> emptyListOfCrsTransformationAdapters = new List<ICrsTransformationAdapter>();

    [Test]
    public void CreateCrsTransformationAverage_ShouldThrowException_WhenInstantiatingWithEmptyList() {
        Helper_ShouldThrowException_WhenInstantiatingWithEmptyList(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage
        );
    }

    [Test]
    public void CreateCrsTransformationMedian_ShouldThrowException_WhenInstantiatingWithEmptyList() {
        Helper_ShouldThrowException_WhenInstantiatingWithEmptyList(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian
        );
    }

    [Test]
    public void CreateCrsTransformationFirstSuccess_ShouldThrowException_WhenInstantiatingWithEmptyList() {
        Helper_ShouldThrowException_WhenInstantiatingWithEmptyList(
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess
        );
    }

    private void Helper_ShouldThrowException_WhenInstantiatingWithEmptyList(
        Func<IList<ICrsTransformationAdapter>, CrsTransformationAdapterComposite> compositeCreator
    ) {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => {
                compositeCreator(emptyListOfCrsTransformationAdapters);
            }
        );
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
        var children = composite.TransformationAdapterChildren;
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
        var children = composite.TransformationAdapterChildren;
        Assert.AreEqual(2, children.Count);
        var listWithTheTwoExpectedClassNames = new List<string>{
            typeof(CrsTransformationAdapterProjNet).FullName,
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