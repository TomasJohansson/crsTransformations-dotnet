using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_7;
using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using GeoAPI.CoordinateSystems; // ICoordinateSystem
using System.Collections.Generic;

namespace Programmerare.CrsTransformations.Test.Implementations {

    [TestFixture]
    class SridReaderTest {
        // The purpose of the below five fields is to use them 
        // in test methods for Equals and HashCode
        private SridReader readerForFilePath1, readerForFilePath2;
        private SridReader readerForEmbeddedResource1, readerForEmbeddedResource2, readerForEmbeddedResource3;

        [SetUp]
        public void SetUp() {
            // These files (path 1 and 2 below) do not have to exist regarding the tests they are used.
            readerForFilePath1 = new SridReader(@"C:\temp\file1.csv");
            readerForFilePath2 = new SridReader(@"C:\temp\file2.csv");
            readerForEmbeddedResource1 = new SridReader(new List<EmbeddedResourceFileWithCRSdefinitions>{EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI});
            readerForEmbeddedResource2 = new SridReader(new List<EmbeddedResourceFileWithCRSdefinitions>{EmbeddedResourceFileWithCRSdefinitions.SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml});
            readerForEmbeddedResource3 = new SridReader(new List<EmbeddedResourceFileWithCRSdefinitions>{EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI, EmbeddedResourceFileWithCRSdefinitions.SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml});
        }

        [Test]
        public void WORLD__WGS_84__4326() {
            // The test is based on the fact that the file 
            // "SRID.csv" contains a row like this:
            // 4326;GEOGCS["WGS 84",DATUM["WGS_1984",SPHEROID["WGS 84",6378137,298.257223563,AUTHORITY["EPSG","7030"]],AUTHORITY["EPSG","6326"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.01745329251994328,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4326"]]
            TestSridReaderForEpsgNumber(
                EpsgNumber.WORLD__WGS_84__4326,
                "GEOGCS[\"WGS 84"
            );
        }

        [Test]
        public void SWEDEN__SWEREF99_TM__3006() {
            // The test is based on the fact that the file 
            // "SRID.csv" contains a row like this:
            // 3006;PROJCS["SWEREF99 TM",GEOGCS["SWEREF99",DATUM["SWEREF99",SPHEROID["GRS 1980",6378137,298.257222101,AUTHORITY["EPSG","7019"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY["EPSG","6619"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.01745329251994328,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4619"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",15],PARAMETER["scale_factor",0.9996],PARAMETER["false_easting",500000],PARAMETER["false_northing",0],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AUTHORITY["EPSG","3006"]]
            TestSridReaderForEpsgNumber(
                EpsgNumber.SWEDEN__SWEREF99_TM__3006,
                "PROJCS[\"SWEREF99 TM"
            );
        }

        [Test]
        public void SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021() {
            // The test is based on the fact that the file 
            // "SRID.csv" contains a row like this:
            // 3021;PROJCS["RT90 2.5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.01745329251994328,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",15.80827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AUTHORITY["EPSG","3021"]]
            TestSridReaderForEpsgNumber(
                EpsgNumber.SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021,
                "PROJCS[\"RT90 2.5 gon V"
            );
        }

        [Test]
        public void EdgeCaseFirstRowInTheCsvFile() {
            // Testing the very first row in the file.

            // The test is based on the fact that the file 
            // "SRID.csv" contains a row (the first row) like this:
            // 2000;PROJCS["Anguilla 1957 / British West Indies Grid",GEOGCS["Anguilla 1957",DATUM["Anguilla_1957",SPHEROID["Clarke 1880 (RGS)",6378249.145,293.465,AUTHORITY["EPSG","7012"]],AUTHORITY["EPSG","6600"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.01745329251994328,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4600"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",-62],PARAMETER["scale_factor",0.9995],PARAMETER["false_easting",400000],PARAMETER["false_northing",0],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AUTHORITY["EPSG","2000"]]
            TestSridReaderForEpsgNumber(
                EpsgNumber.ANGUILLA__ONSHORE__ANGUILLA_1957__BRITISH_WEST_INDIES_GRID__2000,
                "PROJCS[\"Anguilla 1957 / British West Indies Grid"
            );
        }

        [Test]
        public void EdgeCaseLastRowInTheCsvFile() {
            // Testing the very last row in the file.

            // The test is based on the fact that the file 
            // "SRID.csv" contains a row (the last row) like this:
            // 32766;PROJCS["WGS 84 / TM 36 SE",GEOGCS["WGS 84",DATUM["WGS_1984",SPHEROID["WGS 84",6378137,298.257223563,AUTHORITY["EPSG","7030"]],AUTHORITY["EPSG","6326"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.01745329251994328,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4326"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",36],PARAMETER["scale_factor",0.9996],PARAMETER["false_easting",500000],PARAMETER["false_northing",10000000],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AUTHORITY["EPSG","32766"]]
            TestSridReaderForEpsgNumber(
                EpsgNumber.MOZAMBIQUE__OFFSHORE__WGS_84__TM_36_SE__32766,
                "PROJCS[\"WGS 84 / TM 36 SE"
            );
        }

        private void TestSridReaderForEpsgNumber(
            int epsgNumber, 
            string expectedInitialPartOfWellKnownTextString
        ) {
            var listWithOnlyTheStandardFile = new List<EmbeddedResourceFileWithCRSdefinitions>{EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI};
            var sridReader = new SridReader(listWithOnlyTheStandardFile);
            //sridReader = new SridReader(@"PATH_TO_FILE\crsTransformations-dotnet\Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI\SRID_ShippedWithProjNet4GeoAPI_1_4_1.csv");
            ICoordinateSystem crs = sridReader.GetCSbyID(epsgNumber);
            Assert.IsNotNull(crs);
            Assert.AreEqual(
                epsgNumber,
                crs.AuthorityCode
            );
            Assert.That(
                crs.WKT, 
                Does.StartWith(expectedInitialPartOfWellKnownTextString)
            );
        }

        [Test]
        public void Equals_ShouldReturnTrue_WhenTheSameFilePathOrTheSameListOfAssemblyResources() {
            Assert.AreEqual(readerForFilePath1, readerForFilePath1);
            Assert.AreEqual(readerForFilePath2, readerForFilePath2);
            Assert.AreEqual(readerForEmbeddedResource1, readerForEmbeddedResource1);
            Assert.AreEqual(readerForEmbeddedResource2, readerForEmbeddedResource2);
            Assert.AreEqual(readerForEmbeddedResource3, readerForEmbeddedResource3);

            Assert.AreNotEqual(readerForFilePath1, readerForFilePath2);
            Assert.AreNotEqual(readerForFilePath1, readerForEmbeddedResource1);
            Assert.AreNotEqual(readerForEmbeddedResource1, readerForEmbeddedResource2);
            Assert.AreNotEqual(readerForEmbeddedResource1, readerForEmbeddedResource3);
            Assert.AreNotEqual(readerForEmbeddedResource2, readerForEmbeddedResource3);
        }

        [Test]
        public void NotEquals_ShouldReturnTrue_WhenDifferentFilePathOrNotTheSameListOfAssemblyResources() {
            Assert.AreNotEqual(readerForFilePath1, readerForFilePath2);
            Assert.AreNotEqual(readerForFilePath1, readerForEmbeddedResource1);
            Assert.AreNotEqual(readerForEmbeddedResource1, readerForEmbeddedResource2);
            Assert.AreNotEqual(readerForEmbeddedResource1, readerForEmbeddedResource3);
            Assert.AreNotEqual(readerForEmbeddedResource2, readerForEmbeddedResource3);
        }

        [Test]
        public void GetHashCode_ShouldReturnTrue_WhenTheSameFilePathOrTheSameListOfAssemblyResources() {
            Assert.AreEqual(readerForFilePath1.GetHashCode(), readerForFilePath1.GetHashCode());
            Assert.AreEqual(readerForFilePath2.GetHashCode(), readerForFilePath2.GetHashCode());
            Assert.AreEqual(readerForEmbeddedResource1.GetHashCode(), readerForEmbeddedResource1.GetHashCode());
            Assert.AreEqual(readerForEmbeddedResource2.GetHashCode(), readerForEmbeddedResource2.GetHashCode());
            Assert.AreEqual(readerForEmbeddedResource3.GetHashCode(), readerForEmbeddedResource3.GetHashCode());
        }

        [Test]
        public void GetStringForEqualityComparison_ShouldReturnStringWithPathOrEnumValuesForThePredefinedEmbeddedResources() {
            // This method is a bit fragile and it is not really essential to 
            // do regressing testing for this code but it may be somewhat interesting to 
            // see that the generated strings (used in Equals and HashCode)
            // seem properly implemented i.e. that they return a string (as below) in such a way that 
            // different file paths or different list of embedded resource files return different values
            Assert.AreEqual(@"file:C:\temp\file1.csv", readerForFilePath1._GetStringForEqualityComparison());
            Assert.AreEqual("embedded:STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI", readerForEmbeddedResource1._GetStringForEqualityComparison());
            Assert.AreEqual("embedded:SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml", readerForEmbeddedResource2._GetStringForEqualityComparison());
            Assert.AreEqual("embedded:STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI,SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml", readerForEmbeddedResource3._GetStringForEqualityComparison());
        }
    }
}