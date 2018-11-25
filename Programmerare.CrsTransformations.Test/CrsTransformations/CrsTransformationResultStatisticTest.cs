using NUnit.Framework;

namespace Programmerare.CrsTransformations
{
public class CrsTransformationResultStatisticTest : CrsTransformationResultTestBase {

    [Test]
    public void transformResultStatistic_shouldCalculateCorrectAverageAndMeanAndMaxDiffValues() {
        // Both the setup code and the verify/assertion code for this test method 
        // is placed in a base class because it is reused from another test class
        // The keyword "super" is used below to make that more obvious.
        
        CrsTransformationResultStatistic crsTransformationResultStatistic = CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(
            base.listOfSubresultsForStatisticsTest
        );
        
        base.assertCrsTransformationResultStatistic(crsTransformationResultStatistic);
    }
}
}