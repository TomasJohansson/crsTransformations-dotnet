using System.Collections.Generic;

namespace Programmerare.CrsTransformations.TestData
{

public class TestResult {
    // TODO: getters instead of public fields below ...
    public ICrsTransformationAdapter adapter;
    public long totalNumberOfSecondsForAllTransformations;
    public List<TestResultItem> testResultItems;

    public TestResult(
            ICrsTransformationAdapter adapter,
            long totalNumberOfSecondsForAllTransformations,
            List<TestResultItem> testResultItems
    ) {
        this.adapter = adapter;
        this.totalNumberOfSecondsForAllTransformations = totalNumberOfSecondsForAllTransformations;
        this.testResultItems = testResultItems;
    }
}
}