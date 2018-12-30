using System.Collections.Generic;

namespace Programmerare.CrsTransformations.TestData {

public class TestResult {
    public ICrsTransformationAdapter Adapter { get; }
    public long TotalNumberOfSecondsForAllTransformations { get; }
    public IList<TestResultItem> TestResultItems { get; }
    
    public TestResult(
        ICrsTransformationAdapter adapter,
        long totalNumberOfSecondsForAllTransformations,
        IList<TestResultItem> testResultItems
    ) {
        this.Adapter = adapter;
        this.TotalNumberOfSecondsForAllTransformations = totalNumberOfSecondsForAllTransformations;
        this.TestResultItems = testResultItems;
    }
}
}