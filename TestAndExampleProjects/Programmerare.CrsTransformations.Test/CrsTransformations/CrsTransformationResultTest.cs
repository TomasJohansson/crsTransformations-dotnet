using System;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;

namespace Programmerare.CrsTransformations.Core {

[TestFixture]
public class CrsTransformationResultTest : CrsTransformationResultTestBase {

    private CrsTransformationResult transformResultWithSuccessFalse;
    private CrsCoordinate inputCoordinate;
    private CrsCoordinate outputCoordinate;

    [SetUp]
    public void SetUp() {
        inputCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(0.0, 0.0, 1234);
        outputCoordinate = inputCoordinate;
    }

    // Lots of method in this test class creates a 
    // CrsTransformationResult with three of the six 
    // parameters being exactly the same.
    // For that reason this helper method is used,
    // i.e. to not duplicate the three 
    // parameters almost always being the same.
    private CrsTransformationResult CreateCrsTransformationResult(
        //CrsCoordinate inputCoordinate: ,
        CrsCoordinate outputCoordinate,
        Exception exceptionOrNull,
        bool isSuccess
        //crsTransformationAdapterResultSource: ICrsTransformationAdapter,
        //nullableCrsTransformationResultStatistic: CrsTransformationResultStatistic// = null
    ) {
        var transformResult = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinate,
            outputCoordinate,
            exceptionOrNull,
            isSuccess,
            base.compositeAdapterForResultTest,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
        );
        return transformResult;
    }


    [Test]
    public void TransformResult_ShouldNotReturnSuccess_WhenSuccessParameterIsFalse() {
        transformResultWithSuccessFalse = CreateCrsTransformationResult(
            null,
            null,
            false // parameter success = false !
        );
        Assert.IsFalse(transformResultWithSuccessFalse.IsSuccess); // because of success parameter false
    }

    [Test]
    public void TransformResult_ShouldReturnSuccess_WhenSuccessParameterIsTrue() {
        transformResultWithSuccessFalse = CreateCrsTransformationResult(
            outputCoordinate,
            null,
            true
        );
        Assert.IsTrue(transformResultWithSuccessFalse.IsSuccess);
    }

    [Test]
    public void TransformResultConstruction_ShouldThrowException_WhenParametersSuccessFalseAndOutputCoordinateNotNull() {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => {
                //"unvalid TransformResult object construction should throw exception when success false is combined with output coordinate not being null",
                CreateCrsTransformationResult(
                    outputCoordinate, // not null (which it should be when success false as below)
                    null,
                    false
                );           
            }
            ,
            "unvalid TransformResult object construction should throw exception when success false is combined with output coordinate not being null"
        );
    }

    [Test]
    public void TransformResultConstruction_ShouldThrowException_WhenParametersSuccessTrueAndOutputCoordinateNull() {
        CrsCoordinate outputCoordinateNull = null;
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => {
                CreateCrsTransformationResult(
                    outputCoordinateNull, // outputCoordinate = null, then success should be false !
                    null,
                    true
                );
            }
            ,
            "unvalid TransformResult object construction should throw exception when success true is combined with null as output coordinate"
        );
    }

    [Test]
    public void TransformResultConstruction_ShouldThrowException_WhenParametersExceptionIsNotNullAndOutputCoordinateNotNull() {
        Exception someException = new Exception("this is an exception"); 
        CrsCoordinate outputCoordinateNotNull = this.outputCoordinate;
        Assert.IsNotNull(outputCoordinateNotNull); // just to assert what the name claims i.e. not null
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => {
                CreateCrsTransformationResult(
                    outputCoordinateNotNull,
                    // when there is an exception as below then the above coordinate SHOULD BE null !
                    someException,
                    false
                );
            }
            ,
            "unvalid TransformResult object construction should throw exception when an exception parameter is combined with non-null as output coordinate"
        );
    }

    [Test]
    public void TransformResultConstruction_ShouldThrowException_WhenParametersExceptionIsNotNullAndSuccessIsTrue() {
        Exception someException = new Exception("this is an exception");
        CrsCoordinate outputCoordinateNull = null;
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => {
                CreateCrsTransformationResult(
                    outputCoordinateNull,
                    someException,
                    // when there is an exception as above then the below success SHOULD BE false !
                    true
                );
            }
            ,
            "unvalid TransformResult object construction should throw exception when an exception parameter is combined with success true"
        );
    }


    [Test]
    public void TransformResult_ShouldThrowException_WhenTryingToGetCoordinateWhenSuccessIsFalse() {
        outputCoordinate = null;
        transformResultWithSuccessFalse = CreateCrsTransformationResult(
            outputCoordinate,
            null,
            false
        );
        InvalidOperationException e = Assert.Throws<InvalidOperationException>(
            () => {
                var coord = transformResultWithSuccessFalse.OutputCoordinate;
            }
            ,
            "Should not try to get output coordinate unless the result was a success"
        );
        string exceptionMessage = e.Message.ToLower().Replace("-", "");
        // the purpose of the above row is to make the test less sensitive to the exact message
        // e.g. will match text containing "Pre-condition" (including hyphen and uppercased first letter)
        // The exception message should be something like "Precondition violated ..."
        Assert.That(exceptionMessage, Does.Contain("precondition"));
    }

    [Test]
    public void TransformResult_ShouldReturnStatisticsObjectWithCorrectAverageAndMeanAndMaxDiffValues_WhenCreatingResultWithListOfSubresults() {
        // Both the setup code and the verify/assertion code for this test method 
        // is placed in a base class because it is reused from another test class.
        // The keyword "super" is used below to make that more obvious.
        
        CrsTransformationResult transformResult = CrsTransformationResult._CreateCrsTransformationResult(
            base.inputCoordinateNotUsedInStatisticsTest,
            base.outputCoordinateNotUsedInStatisticsTest,
            null,
            true,
            base.compositeAdapterForResultTest,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
        );
        CrsTransformationResultStatistic crsTransformationResultStatistic = transformResult.CrsTransformationResultStatistic;
        base.AssertCrsTransformationResultStatistic(crsTransformationResultStatistic);
    }

    // [Test] Not intended for test execution, just want to verify compilation, see comment in the method
    public void TheInternalMethodsShouldBeAvailableFromTestProject() {
        CrsTransformationResult._CreateCrsTransformationResult(null,null,null,true,null,null);
        // The above "internal" factory method works from this test project 
            // because of this configuration of the proj file in the F# core project:
              //<ItemGroup>
              //  <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
              //    <_Parameter1>Programmerare.CrsTransformations.Test</_Parameter1>
              //  </AssemblyAttribute>
              //</ItemGroup>
        // Note that another test project have been created 
        // without the above kind of configuration and from 
        // there the internal method does indeed (as expected) not work
        // i.e. the code does not even compile if trying to use it.
    }
} // the test class ends here
} // namespace ends here