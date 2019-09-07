namespace Programmerare.CrsTransformations.Core.CompositeTransformations {

using System;
using NUnit.Framework;
using Programmerare.CrsTransformations;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
    using Programmerare.CrsTransformations.CompositeTransformations;

    [TestFixture]
class CrsTransformationAdapterWeightTest {

    private ICrsTransformationAdapter crsTransformationAdapterInstanceNotNull;
    private CrsTransformationAdapterWeightFactory weightFactory;
        
    [SetUp]
    public void Setup() {
        weightFactory = CrsTransformationAdapterWeightFactory.Create();
        crsTransformationAdapterInstanceNotNull = new CrsTransformationAdapterMightyLittleGeodesy();
    }
    
    [Test]
    public void CreateFromInstance_ShouldThrowException_WhenAdapterParameterIsNull() {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => {
            weightFactory.CreateFromInstance(
                null, // adapter
                123 // weight
            );
        });
    }

    [Test]
    public void CreateFromInstance_ShouldThrowException_WhenWeightParameterIsNegative() {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            weightFactory.CreateFromInstance(
                crsTransformationAdapterInstanceNotNull,
                -1 // weight.  null weight leads to compiler error so that it imposslble
            );
        });
    }

    [Test]
    public void CreateFromInstance_ShouldThrowException_WhenWeightParameterIsZero() {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            weightFactory.CreateFromInstance(
                crsTransformationAdapterInstanceNotNull,
                0.0 // weight.  null weight leads to compiler error so that it imposslble
            );
        });
    }

    [Test]
    public void CreateFromStringWithFullClassNameForImplementation_ShouldThrowException_WhenTheClassIsNotImplementingTheExpectedInterface() {
        string nameOfClassNotImplementingTheInterfaceCrsTransformationAdapter = this.GetType().FullName;
        ArgumentException exception = Assert.Throws<ArgumentException>(() => {
            weightFactory.CreateFromStringWithFullClassNameForImplementation(
                nameOfClassNotImplementingTheInterfaceCrsTransformationAdapter,
                123 // weight
            );
        });
    }

    [Test]
    public void CreateFromStringWithFullClassNameForImplementation_ShouldSucceed_WhenTheClassIsImplementingTheExpectedInterface() {
        string nameOfClassImplementingTheInterfaceCrsTransformationAdapter = crsTransformationAdapterInstanceNotNull.GetType().FullName;
        double weightValue = 123;
        CrsTransformationAdapterWeight crsTransformationAdapterWeight = weightFactory.CreateFromStringWithFullClassNameForImplementation(
            nameOfClassImplementingTheInterfaceCrsTransformationAdapter,
            weightValue
        );
        Assert.IsNotNull(crsTransformationAdapterWeight);
        Assert.AreEqual(
            nameOfClassImplementingTheInterfaceCrsTransformationAdapter,  
            crsTransformationAdapterWeight.CrsTransformationAdapter.GetType().FullName
        );
        Assert.AreEqual(
            weightValue, // expected
            crsTransformationAdapterWeight.Weight
        );
    }

}
}