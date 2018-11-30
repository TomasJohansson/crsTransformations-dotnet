using System;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations;

namespace Programmerare.CrsTransformations.TestClient
{
    public class InternalMethodsShouldNOTbeAvailable
    {
        public void test()
        {
            // The following method should NOT be possible to compile from this project

            //CrsTransformationResult._CreateCrsTransformationResult(null,null,null,true,null,null);

            // However, the exact same code above DOES indeed work
            // from the test project's 
            // method 'CrsTransformationResultTest.internalMethodShouldBeAvailableFromTestProject'

            // Just for verifying that the following 
            // does NOT compile from this project,
            // because it is an "internal" method.
            // However, it does work from the test project because 
            // of this configuration of the proj file in the F# core project:
              //<ItemGroup>
              //  <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
              //    <_Parameter1>Programmerare.CrsTransformations.Test</_Parameter1>
              //  </AssemblyAttribute>
              //</ItemGroup>
        }
    }
}
