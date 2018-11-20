﻿using System;
using System.Collections.Generic;
using System.Text;
using com.programmerare.crsTransformations;
using com.programmerare.crsTransformations.Adapter.ProjNet4GeoAPI;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Test.crsTransformations.implementations
{
    [TestFixture]
    class ProjNet4GeoAPITest
    {
        // TODO instead of adding lots of test to this class:
        // Refactor MightyLittleGeodesyTest to reuse those tests !

        private CrsTransformationAdapter crsTransformationAdapter;

        [SetUp]
        public void SetUp()
        {
            crsTransformationAdapter = new CrsTransformationAdapterProjNet4GeoAPI();
        }

        [Test]
        public void AdapteeTypeTest() {
            Assert.IsNotNull(crsTransformationAdapter.AdapteeType);
            Assert.AreEqual(
                CrsTransformationAdapteeType.LEAF_PROJ_NET_4_GEO_API_1_4_1, 
                crsTransformationAdapter.AdapteeType
            );
        }
    }
}