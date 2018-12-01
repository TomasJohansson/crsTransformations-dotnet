namespace Programmerare.CrsTransformations.CompositeTransformations {

using System;
using NUnit.Framework;
using System.Collections.Generic;
using Programmerare.CrsTransformations;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;

[TestFixture]
public class CrsTransformationAdapterCompositeFactoryTest {

    private const int EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS = CrsTransformationAdapterTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;

    [Test]
    public void createCrsTransformationAverage_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationAverage = CrsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationAverage);
    }

    [Test]
    public void createCrsTransformationMedian_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationMedian = CrsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationMedian);
    }

    [Test]
    public void createCrsTransformationFirstSuccess_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationFirstSuccess = CrsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess();
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
            CrsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage
        );
    }

    [Test]
    public void createCrsTransformationMedian_shouldThrowException_whenInstantiatingWithEmptyList() {
        helper_shouldThrowException_whenInstantiatingWithEmptyList(
            CrsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian
        );
    }

    [Test]
    public void createCrsTransformationFirstSuccess_shouldThrowException_whenInstantiatingWithEmptyList() {
        helper_shouldThrowException_whenInstantiatingWithEmptyList(
            CrsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess
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

} // class ends
} // namespace ends