package com.programmerare.com.programmerare.testData;

import com.programmerare.crsTransformations.CrsTransformationAdapter;

import java.util.List;

public class TestResult {
    // TODO: getters instead of public fields below ...
    public final CrsTransformationAdapter adapter;
    public final long totalNumberOfSecondsForAllTransformations;
    public final List<TestResultItem> testResultItems;

    TestResult(
            CrsTransformationAdapter adapter,
            long totalNumberOfSecondsForAllTransformations,
            List<TestResultItem> testResultItems
    ) {
        this.adapter = adapter;
        this.totalNumberOfSecondsForAllTransformations = totalNumberOfSecondsForAllTransformations;
        this.testResultItems = testResultItems;
    }
}