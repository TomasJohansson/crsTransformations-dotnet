using NUnit.Framework;

namespace Programmerare.CrsTransformations {

[TestFixture]
public class CrsTransformationResultStatisticTest : CrsTransformationResultTestBase {

    [Test]
    public void TransformResultStatistic_ShouldCalculateCorrectAverageAndMeanAndMaxDiffValues() {
        // Both the setup code and the verify/assertion code for this test method 
        // is placed in a base class because it is reused from another test class
        // The keyword "base" is not needed but used below to make that more obvious.
        
        CrsTransformationResultStatistic crsTransformationResultStatistic = CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(
            base.listOfSubresultsForStatisticsTest
        );
        // the above factory method is "internal" in the F# project 
        // but still can be used from this test project 
        // because of the following configuration in the F# core project:
        //<ItemGroup>
        //  <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
        //    <_Parameter1>Programmerare.CrsTransformations.Test</_Parameter1>
        //  </AssemblyAttribute>
        //</ItemGroup>
        
        base.AssertCrsTransformationResultStatistic(crsTransformationResultStatistic);
    }
} // the test class ends here
} // namespace ends here