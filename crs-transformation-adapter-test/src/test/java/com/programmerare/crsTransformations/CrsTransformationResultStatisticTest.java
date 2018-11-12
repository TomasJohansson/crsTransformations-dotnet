package com.programmerare.crsTransformations;

import org.junit.jupiter.api.Test;

public class CrsTransformationResultStatisticTest extends CrsTransformationResultTestBase {

    @Test
    void transformResultStatistic_shouldCalculateCorrectAverageAndMeanAndMaxDiffValues() {
        // Both the setup code and the verify/assertion code for this test method 
        // is placed in a base class because it is reused from another test class
        // The keyword "super" is used below to make that more obvious.
        
        final CrsTransformationResultStatistic crsTransformationResultStatistic = CrsTransformationResultStatistic._createCrsTransformationResultStatistic(
            super.listOfSubresultsForStatisticsTest
        );
        
        super.assertCrsTransformationResultStatistic(crsTransformationResultStatistic);
    }
}