package com.programmerare.com.programmerare.testData;

import com.programmerare.crsTransformations.CrsTransformationFacade;

import java.util.List;

public class TestResult {
    // TODO: getters instead of public fields below ...
    public final CrsTransformationFacade facade;
    public final long totalNumberOfSecondsForAllTransformations;
    public final List<TestResultItem> testResultItems;

    TestResult(
        CrsTransformationFacade facade,
        long totalNumberOfSecondsForAllTransformations,
        List<TestResultItem> testResultItems
    ) {
        this.facade = facade;
        this.totalNumberOfSecondsForAllTransformations = totalNumberOfSecondsForAllTransformations;
        this.testResultItems = testResultItems;
    }
}