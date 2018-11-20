using System;
using System.Collections.Generic;
using System.Text;
using com.programmerare.crsTransformations;
using com.programmerare.crsTransformations.Adapter.DotSpatial;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Test.crsTransformations.implementations
{
    [TestFixture]
    class DotSpatialTest
    {
        // TODO instead of adding lots of test to this class:
        // Refactor MightyLittleGeodesyTest to reuse those tests !

        private CrsTransformationAdapter crsTransformationAdapter;

        [SetUp]
        public void SetUp()
        {
            crsTransformationAdapter = new CrsTransformationAdapterDotSpatial();
        }

        [Test]
        public void AdapteeTypeTest() {
            Assert.IsNotNull(crsTransformationAdapter.AdapteeType);
            Assert.AreEqual(
                CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_1_9_0, 
                crsTransformationAdapter.AdapteeType
            );
        }
    }
}
