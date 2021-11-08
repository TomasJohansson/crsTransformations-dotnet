using NUnit.Framework;
using Programmerare.CrsTransformations.Identifier;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v10_036;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.CompositeTransformations;
using System;
using System.Collections.Generic;

using DotSpatial.Projections;

using ProjNet.CoordinateSystems; // CoordinateSystemFactory
using ProjNet.CoordinateSystems.Transformations; // CoordinateTransformationFactory


namespace Programmerare.CrsTransformations.Test.CrsTransformations.WktTest {

// This test class is using some WKT strings copied from rows in the file 
// TestAndExampleProjects\Programmerare.CrsTransformations.Test\resources\wkt_transformation_results\results_sorted_with_best_results_first.csv
// One purpose of this test class is to illustrate that some WKT strings do produce very similar results (and thus quite likely correct)
// for different implementations (e.g. currently these two: DotSpatial and ProjNet)
// while others produce more different results, and then at least one of them is not very correct.
// Another purpose is to show that the results are the same as the results provided by the implementation
// libraries (currently DotSpatial and ProjNet) with some test methods below using those libraries directly 
// and showing that the same differences are retrieved.

// This row below is copied from close to the top of the sorted above mentioned csv :
// 4757|103.81115456377452;1.3086446620725247|T;2.842170943040401E-14;4.440892098500626E-16|T;2.842170943040401E-14;2.220446049250313E-16|EPSG;ProjNet;T;103.81115456377452;1.3086446620725247;T;103.81115456377452;1.3086446620725247;|WKT;ProjNet;T;103.81115456377452;1.3086446620725247;T;103.81115456377452;1.3086446620725247;|EPSG;DotSpatial;T;103.81115456377451;1.3086446620725245;T;103.8111545637745;1.3086446620725245;|WKT;DotSpatial;T;103.8111545637745;1.3086446620725243;T;103.81115456377451;1.3086446620725245;|GEOGCS["SVY21",DATUM["SVY21",SPHEROID["WGS 84",6378137,298.257223563,AUTHORITY["EPSG","7030"]],AUTHORITY["EPSG","6757"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4757"]]
// This row below is copied from much further down in the sorted file:
// 5638|-18.501248008832658;64.7513247740907|T;446635.368730213;158477.93336611055|T;2.1316282072803006E-14;4.1501635905660805E-08|EPSG;ProjNet;T;3000051.9223146066;4902498.1999974735;T;-18.50124800883266;64.7513248114909;|WKT;ProjNet;T;3000051.9223146066;4902498.1999974735;T;-18.50124800883266;64.7513248114909;|EPSG;DotSpatial;T;3000051.921390519;4902498.198035028;T;-18.50124800883265;64.75132476998927;|WKT;DotSpatial;T;3446687.290120732;4744020.266631363;T;-18.50124800883264;64.75132476998934;|PROJCS["ISN2004 / LAEA Europe",GEOGCS["ISN2004",DATUM["Islands_Net_2004",SPHEROID["GRS 1980",6378137,298.257222101,AUTHORITY["EPSG","7019"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY["EPSG","1060"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","5324"]],PROJECTION["Lambert_Azimuthal_Equal_Area"],PARAMETER["latitude_of_center",52],PARAMETER["longitude_of_center",10],PARAMETER["false_easting",4321000],PARAMETER["false_northing",3210000],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AUTHORITY["EPSG","5638"]]

// The WKT-CRS is at the end of the above rows, and they are pasted into variables named "wkt_4757" and "wkt_5638"

[TestFixture]
public class CrsTransformationAdapterCompositeWktTest {

    // the WKT variables below are named after their corresponding EPSG number (see comments above the class declaration in this file above)
    private const string wkt_4757 = "GEOGCS[\"SVY21\",DATUM[\"SVY21\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6757\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4757\"]]";
    private const string wkt_5638 = "PROJCS[\"ISN2004 / LAEA Europe\",GEOGCS[\"ISN2004\",DATUM[\"Islands_Net_2004\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"1060\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"5324\"]],PROJECTION[\"Lambert_Azimuthal_Equal_Area\"],PARAMETER[\"latitude_of_center\",52],PARAMETER[\"longitude_of_center\",10],PARAMETER[\"false_easting\",4321000],PARAMETER[\"false_northing\",3210000],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AUTHORITY[\"EPSG\",\"5638\"]]";

    // Copied from further up in this file:
    // 4757|103.81115456377452;1.3086446620725247|...
    private const double wgs84_4757_Longitude = 103.81115456377452;
    private const double wgs84_4757_Latitude  = 1.3086446620725247;

    // Copied from further up in this file:
    // 5638|-18.501248008832658;64.7513247740907|...
    private const double wgs84_5638_Longitude = -18.501248008832658;
    private const double wgs84_5638_Latitude  = 64.7513247740907;

    // These below delta values are roughly the differences in the results for DotSpatial and ProjNet
    // when using the WKT strings för CRS with EPSG 4757 and 5638.
    // You can also some tests in this class which use DotSpatial and ProjNet directly, 
    // which are also using these delta values in the tests.
    private const double delta_4757 = 0.0000000000001;
    private const double delta_5638 = 446636.0;

    private void AssertReliable(
        CrsTransformationResult result,
        double delta,
        bool isExpectedToBeReliable
    ) {
        string messageIfFailing = "Max Lat diff: " + result.crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude + " , Max Long diff: " + result.crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude;
        if(isExpectedToBeReliable) {
            Assert.IsTrue(result.IsReliable(2, delta), messageIfFailing);
        }
        else {
            Assert.IsFalse(result.IsReliable(2, delta), messageIfFailing);
        }
    }
        

    [Test]
    public void IsReliableTest() {
        var allCrsAdapters = new List<ICrsTransformationAdapter>(){
            new CrsTransformationAdapterProjNet(),
            new CrsTransformationAdapterDotSpatial()
        };
        var crsAdapter = CrsTransformationAdapterCompositeFactory.Create().CreateCrsTransformationMedian(allCrsAdapters);

        CrsIdentifier crs_4757 = CrsIdentifierFactory.CreateFromWktCrs(wkt_4757);
        CrsIdentifier crs_5638 = CrsIdentifierFactory.CreateFromWktCrs(wkt_5638);

        // The first part of the rows (see comments above at the top of this file) provides coordinates 
        // which should be the centroid (defined with WGS84) coordinates for the EPSG area:
        // 4757|103.81115456377452;1.3086446620725247|
        // 5638|-18.501248008832658;64.7513247740907
        CrsCoordinate coordinate_wgs84_4757 = CrsCoordinateFactory.CreateFromLongitudeLatitude(
            wgs84_4757_Longitude, wgs84_4757_Latitude
        );
        CrsCoordinate coordinate_wgs84_5638 =  CrsCoordinateFactory.CreateFromLongitudeLatitude(
            wgs84_5638_Longitude, wgs84_5638_Latitude
        );

        var result_4757 = crsAdapter.Transform(coordinate_wgs84_4757, crs_4757);
        var result_5638 = crsAdapter.Transform(coordinate_wgs84_5638, crs_5638);
        Assert.IsTrue(result_4757.isSuccess);
        Assert.IsTrue(result_5638.isSuccess);

        // Please note that the below assertion delta values are not chosen because 
        // it *should* work with those values, 
        // but it is just illustrating how it works, i.e. that the differences can be large values
        // (as for EPSG 5638) or small values (as for EPSG 4757)
        // Also, it shows, with other test methods in this class, that it just works the same 
        // as in the used libraries.

        // Max Lat diff: 4.440892098500626E-16 , Max Long diff: 2.842170943040401E-14
        this.AssertReliable(result_4757, delta_4757, true); 
            // above: both CRS adapters (DotSpatial and ProjNet, used in the Composite) got very similar results
            // below: if trying an even smaller delta then it fails (but false is asserted below to not make the test fail)
        this.AssertReliable(result_4757, (delta_4757 / 10.0), false);
        
        // Max Lat diff: 158477.93336611055 , Max Long diff: 446635.36780612543
        this.AssertReliable(result_5638, delta_5638, true);
        this.AssertReliable(result_5638, (delta_5638 - 1.0), false);
    }


    [Test]
    public void TestWithNativeCode_4757() {
        TestWithNativeCode(
            wkt_4757,
            wgs84_4757_Longitude,
            wgs84_4757_Latitude,
            deltaLower: delta_4757 / 10.0,
            deltaUpper: delta_4757
        );
    }

    [Test]
    public void TestWithNativeCode_5638() {
        TestWithNativeCode(
            wkt_5638,
            wgs84_5638_Longitude,
            wgs84_5638_Latitude,
            deltaLower: delta_5638 - 1,
            deltaUpper: delta_5638
        );
    }
        
    private void TestWithNativeCode(
        string wkt,
        double longitude,
        double latitude,
        double deltaLower,
        double deltaUpper
    ) {
        double[] resultDotSpatial, resultProjNet;
        resultDotSpatial    = Transform_with_DotSpatial(wkt, longitude, latitude);
        resultProjNet       = Transform_with_ProjNet(wkt, longitude, latitude);
        double diffX        = Math.Abs(resultDotSpatial[0] - resultProjNet[0]);
        double diffY        = Math.Abs(resultDotSpatial[1] - resultProjNet[1]);
        double maxDiff      = Math.Max(diffX, diffY);
        // A note regarding the below 'GreaterThan' assertion:
        // Normally, you would just want the difference to be as small as possible,
        // i.e. there is not really any strong reason for asserting that it is greater than a lower 
        // value, but the purpose here is just to make it clear that the actual delta values is 
        // indeed quite close to the delta values provided in the constants of this class.
        // (which are used as parameters for this method)
        Assert.That(maxDiff, Is.GreaterThan(deltaLower).And.LessThan(deltaUpper));
    }

    private double[] Transform_with_DotSpatial(string wkt, double longitude, double latitude) {
        // The code here below in this method is using similar code as in the below F# file:
        // crstransformations-dotnet\Programmerare.CrsTransformations.Adapter.DotSpatial\CrsTransformationAdapterDotSpatial.fs
        ProjectionInfo projInfoSourceCrs = ProjectionInfo.FromEpsgCode(EpsgNumber.WORLD__WGS_84__4326);
        ProjectionInfo projInfoTargetCrs = ProjectionInfo.FromEsriString(wkt);
        var xy = new double[]{longitude, latitude};
        Reproject.ReprojectPoints(xy, null, projInfoSourceCrs, projInfoTargetCrs, 0, 1);
        return xy;
    }

    private double[] Transform_with_ProjNet(string wkt, double longitude, double latitude) {
        // The code here below in this method is using similar code as in the below F# file:
        // crstransformations-dotnet\Programmerare.CrsTransformations.Adapter.ProjNet\CrsTransformationAdapterProjNet.fs
        var coordinateSystemFactory = new CoordinateSystemFactory();
        CoordinateSystem sourceCrs = GeographicCoordinateSystem.WGS84;
        CoordinateSystem targetCrs = coordinateSystemFactory.CreateFromWkt(wkt);
        var ctFact = new CoordinateTransformationFactory();
        var xy = new double[]{longitude, latitude};
        ICoordinateTransformation trans = ctFact.CreateFromCoordinateSystems(sourceCrs, targetCrs);
        double[] result = trans.MathTransform.Transform(xy);
        return result;
    }


} // class ends here
} // namespace ends here