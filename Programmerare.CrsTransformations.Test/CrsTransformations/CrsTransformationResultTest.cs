using System;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;

// TODO: refactor the methods in this test class 
// regarding duplicated code for construction 
// of result within Assert.Throws 

namespace Programmerare.CrsTransformations
{
public class CrsTransformationResultTest : CrsTransformationResultTestBase {

    private CrsTransformationResult transformResultWithSuccessFalse;
    private CrsCoordinate inputCoordinate;
    private CrsCoordinate outputCoordinate;

    [SetUp]
    public void SetUp() {
        inputCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(0.0, 0.0, 1234);
        outputCoordinate = inputCoordinate;
    }

    [Test]
    public void transformResult_shouldNotReturnSuccess_whenSuccessParameterIsFalse() {
        transformResultWithSuccessFalse = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinate,
            null,
            null,
            false, // parameter success = false !
            base.compositeAdapterForResultTest,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
        );
        Assert.IsFalse(transformResultWithSuccessFalse.IsSuccess); // because of success parameter false
    }

    [Test]
    public void transformResult_shouldReturnSuccess_whenSuccessParameterIsTrue() {
        transformResultWithSuccessFalse = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinate,
            outputCoordinate,
            null,
            true,
            base.compositeAdapterForResultTest,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
        );
        Assert.IsTrue(transformResultWithSuccessFalse.IsSuccess);
    }

    [Test]
    public void transformResultConstruction_shouldThrowException_whenParametersSuccessFalseAndOutputCoordinateNotNull() {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            {
                //"unvalid TransformResult object construction should throw exception when success false is combined with output coordinate not being null",
                CrsTransformationResult._CreateCrsTransformationResult(
                    inputCoordinate, 
                    outputCoordinate, // not null (which it should be when success false as below)
                    null,
                    false,
                    base.compositeAdapterForResultTest,
                    CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
                );           
            },
            "unvalid TransformResult object construction should throw exception when success false is combined with output coordinate not being null"
        );
    }

    [Test]
    public void transformResultConstruction_shouldThrowException_whenParametersSuccessTrueAndOutputCoordinateNull() {
        CrsCoordinate outputCoordinateNull = null;
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            {
                CrsTransformationResult._CreateCrsTransformationResult(
                    inputCoordinate,
                    outputCoordinateNull, // outputCoordinate = null, then success should be false !
                    null,
                    true,
                    base.compositeAdapterForResultTest,
                    CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
                );
            },
            "unvalid TransformResult object construction should throw exception when success true is combined with null as output coordinate"
        );
    }

    [Test]
    public void transformResultConstruction_shouldThrowException_whenParametersExceptionIsNotNullAndOutputCoordinateNotNull() {
        Exception someException = new Exception("this is an exception"); 
        CrsCoordinate outputCoordinateNotNull = this.outputCoordinate;
        Assert.IsNotNull(outputCoordinateNotNull); // just to assert what the name claims i.e. not null
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            {
                CrsTransformationResult._CreateCrsTransformationResult(
                    inputCoordinate,
                    outputCoordinateNotNull,
                    // when there is an exception as below then the above coordinate SHOULD BE null !
                    someException,
                    false,
                    base.compositeAdapterForResultTest,
                    CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
                );
            },
            "unvalid TransformResult object construction should throw exception when an exception parameter is combined with non-null as output coordinate"
        );
    }


    [Test]
    public void transformResultConstruction_shouldThrowException_whenParametersExceptionIsNotNullAndSuccessIsTrue() {
        Exception someException = new Exception("this is an exception");
        CrsCoordinate outputCoordinateNull = null;
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            {
                CrsTransformationResult._CreateCrsTransformationResult(
                inputCoordinate,
                outputCoordinateNull,
                someException,
                // when there is an exception as above then the below success SHOULD BE false !
                true,
                base.compositeAdapterForResultTest,
                CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
            );
            },
            "unvalid TransformResult object construction should throw exception when an exception parameter is combined with success true"
        );

    }

    [Test]
    public void transformResult_shouldThrowException_whenTryingToGetCoordinateWhenSuccessIsFalse() {
        outputCoordinate = null;
        transformResultWithSuccessFalse = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinate,
            outputCoordinate,
            null,
            false,
            base.compositeAdapterForResultTest,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
        );
        InvalidOperationException e = Assert.Throws<InvalidOperationException>(() =>
            {
                var coord = transformResultWithSuccessFalse.OutputCoordinate;
            },
            "Should not try to get output coordinate unless the result was a success"
        );
        string exceptionMessage = e.Message.ToLower().Replace("-", "");
        // the purpose of the above row is to make the test less sensitive to the exact message
        // e.g. will match text containing "Pre-condition" (including hyphen and uppercased first letter)
        // The exception message should be something like "Precondition violated ..."
        Assert.That(exceptionMessage, Does.Contain("precondition"));
    }

    [Test]
    public void transformResult_shouldReturnStatisticsObjectWithCorrectAverageAndMeanAndMaxDiffValues_whenCreatingResultWithListOfSubresults() {

        // Both the setup code and the verify/assertion code for this test method 
        // is placed in a base class because it is reused from another test class.
        // The keyword "super" is used below to make that more obvious.
        
        CrsTransformationResult transformResult = CrsTransformationResult._CreateCrsTransformationResult(
            base.inputCoordinateNotUsedInStatisticsTest,
            base.outputCoordinateNotUsedInStatisticsTest,
            null,
            true,
            base.compositeAdapterForResultTest,

            //base.listOfSubresultsForStatisticsTest,
            //nullTransformationResultStatistic
            // the above parameter with the statictics object is null 
            // i.e. it is not precalculated but will become created
            // (which this test method is testing further down below)                

            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(base.listOfSubresultsForStatisticsTest)
        );
        CrsTransformationResultStatistic crsTransformationResultStatistic = transformResult.CrsTransformationResultStatistic;
        
        base.assertCrsTransformationResultStatistic(crsTransformationResultStatistic);
    }

}
}