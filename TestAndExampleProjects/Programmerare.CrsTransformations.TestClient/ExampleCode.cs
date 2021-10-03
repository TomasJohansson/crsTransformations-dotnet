using System.Collections.Generic; // IList
using Programmerare.CrsTransformations; // ICrsTransformationAdapter
using Programmerare.CrsTransformations.CompositeTransformations; // CrsTransformationAdapterCompositeFactory
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;


using Programmerare.CrsTransformations.Identifier; // CrsIdentifier
using Programmerare.CrsTransformations.Coordinate; // CrsCoordinate
using static Programmerare.CrsTransformations.Coordinate.CrsCoordinateFactory;
// The above row with "using static" enables many factory methods:
// LatLon , LonLat , YX , XY , ... and so on (see the example code below)

namespace Programmerare.CrsTransformations.TestClient {
class ExampleCode {
    ICrsTransformationAdapter crsTransformationAdapter;

public void method() {
    // ...

    // The interface with seven implementations as illustrated below
    ICrsTransformationAdapter crsTransformationAdapter;
    // The interface is defined in the library "Programmerare.CrsTransformations.Core" with this full name:
    // Programmerare.CrsTransformations.ICrsTransformationAdapter

    // The three 'Leaf' implementations:

    // Library "Programmerare.CrsTransformations.Adapter.DotSpatial", class:
    // Programmerare.CrsTransformations.Adapter.DotSpatial.CrsTransformationAdapterDotSpatial
    crsTransformationAdapter = new CrsTransformationAdapterDotSpatial();

    // Library "Programmerare.CrsTransformations.Adapter.ProjNet", class:
    // Programmerare.CrsTransformations.Adapter.ProjNet.CrsTransformationAdapterProjNet
    crsTransformationAdapter = new CrsTransformationAdapterProjNet();

    // Library "Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy", class:
    // Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy.CrsTransformationAdapterMightyLittleGeodesy
    crsTransformationAdapter = new CrsTransformationAdapterMightyLittleGeodesy();

    // - - - - - - - - - - - -

    // The four 'Composite' implementations below are all located in the library
    // "Programmerare.CrsTransformations.Core" and the factory class is:
    // Programmerare.CrsTransformations.CompositeTransformations.CrsTransformationAdapterCompositeFactory
    var crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();

    crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian();

    crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage();

    crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess();

    // All of the above three factory methods without any parameter will try to use as many of 
    // the three (currently) 'leaf' implementations as are available in runtime 
    // (e.g. are included as NuGet dependencies).
    // If you want to specify explicitly which ones to be used, you can provide 
    // a parameter 'IList<ICrsTransformationAdapter>' to the Create method like this:
    crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create(
        new List<ICrsTransformationAdapter>{
            new CrsTransformationAdapterDotSpatial(),
            new CrsTransformationAdapterProjNet(),
            new CrsTransformationAdapterMightyLittleGeodesy(),
        }
    );

    // The fourth 'Composite' below does not use any implicit implementations  
    // but if you want to use a result created as a weighted average then the weights need 
    // to be specified explicitly per leaf implementation as in the example below.
    var weightFactory = CrsTransformationAdapterWeightFactory.Create();
    crsTransformationAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(
        new List<CrsTransformationAdapterWeight> {
            weightFactory.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), 1.0),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterProjNet(), 1.0),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), 2.0),
        }
    );
    // The weight values above illustrates a situation where you (for some reason) want to consider 
    // the transformation results from 'MightyLittleGeodesy' as being 'two times better' than the others.
    }
public void method2() {
    int epsgNumber = 4326;
    string crsCode = "EPSG:" + epsgNumber;
    CrsIdentifier crsIdentifier; // namespace Programmerare.CrsTransformations.Identifier
    crsIdentifier = CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumber);
    // Alternative:
    crsIdentifier = CrsIdentifierFactory.CreateFromCrsCode(crsCode);

    double latitude = 59.330231;
    double longitude = 18.059196;

    CrsCoordinate crsCoordinate; // namespace Programmerare.CrsTransformations.Coordinate
    // All the below methods are alternatives for creating the same coordinate 
    // with the above latitude/longitude and coordinate reference system.
    // No class or object is used for the methods below because of the following static import:
    // using static Programmerare.CrsTransformations.Coordinate.CrsCoordinateFactory;
    crsCoordinate = LatLon(latitude, longitude, epsgNumber);
    crsCoordinate = LatLon(latitude, longitude, crsCode);
    crsCoordinate = LatLon(latitude, longitude, crsIdentifier);

    crsCoordinate = LonLat(longitude, latitude, epsgNumber);
    crsCoordinate = LonLat(longitude, latitude, crsCode);
    crsCoordinate = LonLat(longitude, latitude, crsIdentifier);

    crsCoordinate = YX(latitude, longitude, epsgNumber);
    crsCoordinate = YX(latitude, longitude, crsCode);
    crsCoordinate = YX(latitude, longitude, crsIdentifier);

    crsCoordinate = XY(longitude, latitude, epsgNumber);
    crsCoordinate = XY(longitude, latitude, crsCode);
    crsCoordinate = XY(longitude, latitude, crsIdentifier);

    crsCoordinate = NorthingEasting(latitude, longitude, epsgNumber);
    crsCoordinate = NorthingEasting(latitude, longitude, crsCode);
    crsCoordinate = NorthingEasting(latitude, longitude, crsIdentifier);

    crsCoordinate = EastingNorthing(longitude, latitude, epsgNumber);
    crsCoordinate = EastingNorthing(longitude, latitude, crsCode);
    crsCoordinate = EastingNorthing(longitude, latitude, crsIdentifier);

    crsCoordinate = CreateFromYNorthingLatitudeAndXEastingLongitude(latitude, longitude, epsgNumber);
    crsCoordinate = CreateFromYNorthingLatitudeAndXEastingLongitude(latitude, longitude, crsCode);
    crsCoordinate = CreateFromYNorthingLatitudeAndXEastingLongitude(latitude, longitude, crsIdentifier);

    crsCoordinate = CreateFromXEastingLongitudeAndYNorthingLatitude(longitude, latitude, epsgNumber);
    crsCoordinate = CreateFromXEastingLongitudeAndYNorthingLatitude(longitude, latitude, crsCode);
    crsCoordinate = CreateFromXEastingLongitudeAndYNorthingLatitude(longitude, latitude, crsIdentifier);

    CrsIdentifier targetCrs = CrsIdentifierFactory.CreateFromEpsgNumber(3006);
    ICrsTransformationAdapter crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.Create().CreateCrsTransformationAverage();
    CrsTransformationResult crsTransformationResult = crsTransformationAdapter.Transform(crsCoordinate, targetCrs);
    // see more example code further down in this webpage
        }

    }
}
