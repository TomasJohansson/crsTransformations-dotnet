using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;

namespace Programmerare.CrsTransformations.CompositeTransformations 
{
public abstract class CompositeStrategyTestBase {

    protected CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;

    protected ICrsTransformationAdapter adapterDotSpatial;
    protected ICrsTransformationAdapter adapterMightyLittleGeodesy;
    protected ICrsTransformationAdapter adapterProjNet4GeoAPI;

    protected IList<ICrsTransformationAdapter> allAdapters;
    protected List<CrsCoordinate> allCoordinateResultsForTheDifferentImplementations;

    protected const double wgs84Lat = 59.330231;
    protected const double wgs84Lon = 18.059196;
    protected const double sweref99_Y_expected = 6580822;
    protected const  double sweref99_X_expected = 674032;

    protected CrsCoordinate wgs84coordinate;
    protected CrsCoordinate resultCoordinateDotSpatial;
    protected CrsCoordinate resultCoordinateMightyLittleGeodesy;
    protected CrsCoordinate resultCoordinateProjNet4GeoAPI;

    [SetUp]
    public void SetUp() {
        crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();

        adapterDotSpatial = new CrsTransformationAdapterDotSpatial();
        adapterProjNet4GeoAPI = new CrsTransformationAdapterProjNet4GeoAPI();
        adapterMightyLittleGeodesy = new CrsTransformationAdapterMightyLittleGeodesy();

        allAdapters = new List<ICrsTransformationAdapter>{
            // Regarding the order of the items in the list below:
            // DotSpatial should be the first since it is assumed in the test by the subclass CompositeStrategyFirstSuccessTest
            adapterDotSpatial,
            adapterProjNet4GeoAPI,
            adapterMightyLittleGeodesy
        };

        wgs84coordinate = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(wgs84Lat, wgs84Lon, EpsgNumber.WORLD__WGS_84__4326);

        resultCoordinateDotSpatial = adapterDotSpatial.TransformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        resultCoordinateProjNet4GeoAPI = adapterProjNet4GeoAPI.TransformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        resultCoordinateMightyLittleGeodesy = adapterMightyLittleGeodesy.TransformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        allCoordinateResultsForTheDifferentImplementations = new List<CrsCoordinate>{
            resultCoordinateDotSpatial,
            resultCoordinateMightyLittleGeodesy,
            resultCoordinateProjNet4GeoAPI
        };
    }
}
}