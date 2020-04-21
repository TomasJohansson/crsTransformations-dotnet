using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.CompositeTransformations;
using System; // Console

namespace Programmerare.CrsTransformations.TestClient {
    
class SmallCSharpeExample {
    public void method() {
        Console.WriteLine("SmallCSharpeExample starts");
        int epsgWgs84  = 4326;
        int epsgSweRef = 3006;
        // alternative to the above two hardcodings: use the library "Programmerare.CrsTransformations.Constants"
        // and constants EpsgNumber.WORLD__WGS_84__4326 and EpsgNumber.SWEDEN__SWEREF99_TM__3006
        // from the class Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_8_9.EpsgNumber
         
        CrsCoordinate centralStockholmWgs84 = CrsCoordinateFactory.LatLon(59.330231, 18.059196, epsgWgs84);
        
        ICrsTransformationAdapter crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.Create().CreateCrsTransformationMedian();
        // If the NuGet configuration includes all (currently three) adapter implementations, then the 
        // above created 'Composite' implementation will below use all three 'leaf' implementations 
        // and return a coordinate with a median longitude and a median latitude
        CrsTransformationResult centralStockholmResultSweRef = crsTransformationAdapter.Transform(centralStockholmWgs84, epsgSweRef);
        
        if(centralStockholmResultSweRef.IsSuccess) {
            Console.WriteLine(centralStockholmResultSweRef.OutputCoordinate);
            // Console output from the above code row: 
            // CrsCoordinate(xEastingLongitude=674032.357177155, yNorthingLatitude=6580821.99121561, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))
        }
        Console.WriteLine("SmallCSharpeExample ends");
        Console.ReadLine();
    }
}
}