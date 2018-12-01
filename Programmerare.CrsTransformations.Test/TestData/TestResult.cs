using System.Collections.Generic;

namespace Programmerare.CrsTransformations.TestData
{

public class TestResult {
    // TODO change the public fields to properties  
    public ICrsTransformationAdapter adapter;
    public long totalNumberOfSecondsForAllTransformations;
    public IList<TestResultItem> testResultItems;

    public TestResult(
            ICrsTransformationAdapter adapter,
            long totalNumberOfSecondsForAllTransformations,
            IList<TestResultItem> testResultItems
    ) {
        this.adapter = adapter;
        this.totalNumberOfSecondsForAllTransformations = totalNumberOfSecondsForAllTransformations;
        this.testResultItems = testResultItems;
    }
}
}