using System;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations;
using System.Collections.Generic;

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

            
            CrsTransformationAdapterBase c = null;
            // the below methods was previously exposed
            // but should NOT be so anymore
            //c._GetFileInfoVersion();
            //var res = c._TransformHook(null, null);
            //var coord = c._TransformToCoordinateHook(null, null);
            // Actually the above methods have now been renamed 
            // to "Strategy" instead of "Hook" because of the 
            // design pattern being used now after some refactoring 
            // that prevented exposing of methods considering 
            // the fact that "protected" can not be used in F#
            // and "internal" was not appropriate since 
            // other assemblies are implemening the base type.

            CrsTransformationAdapterBaseLeaf cc = null;
            // the below method was previously exposed
            // but should NOT be so anymore i.e. should no longer even be possible to compile:
            //var coordinate = cc._TransformToCoordinateHookLeaf(null, null);

            // The below types was previously exposed as result types for the Create methods
            //CrsTransformationAdapterLeafFactoryWithHardcodedImplementations c1 = CrsTransformationAdapterLeafFactory.Create();
            //CrsTransformationAdapterLeafFactoryWithConfiguredImplementations c2 = CrsTransformationAdapterLeafFactory.Create(new List<ICrsTransformationAdapter>());
            // Now instead both types are instead only exposed through the base type as below
            CrsTransformationAdapterLeafFactory c1 = CrsTransformationAdapterLeafFactory.Create();
            CrsTransformationAdapterLeafFactory c2 = CrsTransformationAdapterLeafFactory.Create(new List<ICrsTransformationAdapter>());
        }
    }
}
