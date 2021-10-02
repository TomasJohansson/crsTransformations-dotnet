using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v10_036;

using NUnit.Framework;

namespace Programmerare.CrsConstants.ConstantsByAreaNameNumber {
    [TestFixture]
    class EpsgNumberTest {
        
        [Test]
        public void WGS_84() {
            Assert.AreEqual(
                4326,
                EpsgNumber.WORLD__WGS_84__4326
            );
        }

        [Test]
        public void SWEREF99_TM() {
            Assert.AreEqual(
                3006,
                EpsgNumber.SWEDEN__SWEREF99_TM__3006
            );
        }

        [Test]
        public void RT90_2_5_GON_V() {
            Assert.AreEqual(
                3021,
                EpsgNumber.SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021
            );
        }

    }
}