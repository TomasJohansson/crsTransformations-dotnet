using NUnit.Framework;
using System.Collections.Generic;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.CompositeTransformations;

// Only one (i.e. the one below) of the adapters are 
// added as depdency to this project:
using Programmerare.CrsTransformations.Adapter.ProjNet;

namespace Programmerare.CrsTransformations.TestWithoutDependeciesToAllAdapters {

    [TestFixture]
    class AdapterFactoryTest {

        // The purpose of this test project is that all adapters 
        // should NOT be added !
        // Code based on reflection should NOT fail with exception 
        // but simply ignore those adapter implementations 
        // which are not availale when trying to create 
        // a composite adapter with the available leaf adapters

        private const int NUMBER_OF_ADAPTERS_WHICH_ARE_DEPENDENCIES_FROM_THIS_PROJECT = 1; // Only "ProjNet"

        [Test]
        public void AdapterCompositeFactory_ShouldNotFail_WhenNotAllAdaptersAreAvailable() {
            var factory = CrsTransformationAdapterCompositeFactory.Create();
            var median = factory.CreateCrsTransformationMedian();
            var coord = CrsCoordinateFactory.LatLon(60.0, 20.0);
            var result = median.Transform(coord, 3006);
            Assert.IsTrue(result.IsSuccess);
            IList<CrsTransformationResult> childrenResults = result.TransformationResultChildren;
            Assert.AreEqual(
                NUMBER_OF_ADAPTERS_WHICH_ARE_DEPENDENCIES_FROM_THIS_PROJECT,
                childrenResults.Count
            );
            var outputCoord = result.OutputCoordinate;
        }
    }
}