using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_8_9;
using Programmerare.CrsTransformations.CompositeTransformations;
using Programmerare.CrsTransformations.Coordinate;
using System;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations.TestClient {
    class LargerCSharpeExample {
public void method() {
    Console.WriteLine("LargerCSharpeExample starts");

    // Some terminology regarding the names used in the below code example:
    // "CRS" = Coordinate Reference System
    // "WGS84" is the most frequently used coordinate system (e.g. the coordinates usually used in a GPS)    
    // "SWEREF99TM" is the official coordinate system used by authorities in Sweden
    // "EPSG" = "European Petroleum Survey Group" was (but the EPSG name is still often used) 
    //           an organization defining CRS with integer numbers e.g.  4326 for WGS84 or 3006 for SWEREF99TM
    int epsgWgs84  = EpsgNumber.WORLD__WGS_84__4326;
    int epsgSweRef = EpsgNumber.SWEDEN__SWEREF99_TM__3006;
    // The above "EpsgNumber" class with LOTS OF constants (and more constants classes) have been generated, 
    // using "FreeMarker" and database downloaded from EPSG ( http://www.epsg.org or http://www.epsg-registry.org ) 
    // from "crs-transformation-code-generation" in the project https://github.com/TomasJohansson/crsTransformations

    CrsCoordinate centralStockholmWgs84 = CrsCoordinateFactory.LatLon(59.330231, 18.059196, epsgWgs84);
    // https://kartor.eniro.se/m/03Yxp
    // SWEREF99TM coordinates (for WGS84 59.330231, 18.059196) 
    // according to Eniro (above URL): 6580822, 674032 (northing, easting)
    
    ICrsTransformationAdapter crsTransformationAdapter; // interface with concrete "leaf" implementation or "composite" implementations
    // This code example is using a "composite" which will use multiple libraries to do the same transformation and then 
    // return a coordinate with the median values (median of the northing values and median of the easting values)  
    crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.Create().CreateCrsTransformationMedian();
    // The above factory will try to use those known objects which implements the interface i.e. the number 
    // of "leaf" objects will depend on how many you included as for example NuGet dependencies (three in the above NuGet example)
    Console.WriteLine("Number of 'leafs' : " + crsTransformationAdapter.TransformationAdapterChildren.Count);
    // Console output from the above row:
    // Number of 'leafs' : 3

    // Transform the WGS84 coordinate to a SWEREF99TM coordinate:
    CrsCoordinate centralStockholmSweRef = crsTransformationAdapter.TransformToCoordinate(centralStockholmWgs84, epsgSweRef);
    Console.WriteLine("Median Composite Northing: " + centralStockholmSweRef.Northing);
    Console.WriteLine("Median Composite Easting: " + centralStockholmSweRef.Easting);
    // Console output from the above two rows:
    //      Median Composite Northing: 6580821.99121561
    //      Median Composite Easting: 674032.357177155
    // (and these can be compared with the 'Eniro' values above i.e. '6580822, 674032 (northing, easting)' )
    
    // The coordinate class provides four properties with different names for the same east-west value and 
    // four properties for the same name each north-south value, as below:
    //      Four EQUIVALENT properties:  Easting  , X , Longitude , XEastingLongitude
    //      Four EQUIVALENT properties:  Northing , Y , Latitude  , YNorthingLatitude
    // Regarding the above alternative methods, depending on the desired semantic in your context, you may want to use:
    //      X/Y for a geocentric or cartesian system
    //      Longitude/Latitude for a geodetic or geographic system
    //      Easting/Northing for a cartographic or projected system
    //      xEastingLongitude/yNorthingLatitude for general code handling different types of system
    
    // If you want more details for the result you can use the following 'Transform' method: 
    //  (instead of the method 'TransformToCoordinate' used above)
    CrsTransformationResult centralStockholmResultSweRef = crsTransformationAdapter.Transform(centralStockholmWgs84, epsgSweRef);
    if(!centralStockholmResultSweRef.IsSuccess) {
        Console.WriteLine("No coordinate result");
    }
    else {
        if(centralStockholmResultSweRef.IsReliable(
            2,      // minimumNumberOfSuccesfulResults
            0.01    // maxDeltaValueForXLongitudeAndYLatitude
        )) {
            // at least 2 succesful results and the maximal difference in northing or easting is less than 0.01
            // (and if you want to know the exact difference you can find it in this code example further down the page)
            Console.WriteLine("Reliable result"); // according to your chosen parameters to the method 'isReliable'    
        }
        else {
            Console.WriteLine("Not reliable result");
        }
        Console.WriteLine(centralStockholmResultSweRef.OutputCoordinate);
        // Console output from the above code row:
        // CrsCoordinate(xEastingLongitude=674032.357177155, yNorthingLatitude=6580821.99121561, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))
        
        // When your code is in a context where you only have the result (but not the adapter object) 
        // (e.g. in a method receiving the result as a parameter)
        // you can get back the object which created the result as below:
        ICrsTransformationAdapter crsTransformationAdapterResultSource = centralStockholmResultSweRef.CrsTransformationAdapterResultSource;
        CrsTransformationAdapteeType adapteeType = crsTransformationAdapterResultSource.AdapteeType;
        Console.WriteLine("adapteeType: " + adapteeType); // console output: COMPOSITE_MEDIAN
        // The above code row returned an enum which is not really a true adaptee just like the 'composite' is not a true adapter.
        // However, when iterating (as below) the "leaf" results, 
        // it might be more interesting to keep track of from where the different values originated
        IList<CrsTransformationResult> transformationResultChildren = centralStockholmResultSweRef.TransformationResultChildren;
        foreach (CrsTransformationResult crsTransformationResultLeaf in transformationResultChildren) {
            if(!crsTransformationResultLeaf.IsSuccess) continue; // continue with the next 'leaf'
            
            ICrsTransformationAdapter resultAdapter = crsTransformationResultLeaf.CrsTransformationAdapterResultSource;
            Console.WriteLine(resultAdapter.AdapteeType);
            // The above code row will output rows like this: 
            // "LEAF_PROJ_NET_4_GEO_API_1_4_1" or "LEAF_MIGHTY_LITTLE_GEODESY_1_0_1" and so on
            if(!crsTransformationResultLeaf.IsReliable(
                    2,      // minimumNumberOfSuccesfulResults
                    1000    // maxDeltaValueForXLongitudeAndYLatitude
            )) {
                // The above constraint "at least 2 implementations" will always fail because now we are dealing with "leafs"
                // The above delta value constraint has very high tolerance but it does not matter since 
                // the constraint about the number of implementations will fail
                Console.WriteLine("Only 'composites' can have more than one result and this is a 'leaf' and thus does not have at least two results");
            }
            Console.WriteLine("Adapter long name: " + resultAdapter.LongNameOfImplementation); // full class name including package
            Console.WriteLine("Adapter short name: " + resultAdapter.ShortNameOfImplementation); // class name suffix i.e. the unique part
            // The above "long" names will be for example:
            //      Programmerare.CrsTransformations.Adapter.DotSpatial.CrsTransformationAdapterDotSpatial
            //      Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy.CrsTransformationAdapterMightyLittleGeodesy
            // The above "short" names will be for example:
            //      DotSpatial
            //      MightyLittleGeodesy
            Console.WriteLine("adaptee: " + resultAdapter.AdapteeType);
            // The above row will output for example:
            //      LEAF_DOT_SPATIAL_2_0_0_RC1
            //      LEAF_MIGHTY_LITTLE_GEODESY_1_0_1
            // (note that the version number is included for the adaptees)
            Console.WriteLine("isComposite: " + resultAdapter.IsComposite); // "False" since we are iterating "leaf" results
            Console.WriteLine("Coordinate result for " + resultAdapter.AdapteeType + " : " + crsTransformationResultLeaf.OutputCoordinate);
            // The above row will output these rows when doing the iteration:
            //      Coordinate result for LEAF_DOT_SPATIAL_2_0_0_RC1 : CrsCoordinate(xEastingLongitude=674032.357322213, yNorthingLatitude=6580821.99121561, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))
            //      Coordinate result for LEAF_PROJ_NET_4_GEO_API_1_4_1 : CrsCoordinate(xEastingLongitude=674032.357177155, yNorthingLatitude=6580821.99437121, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))
            //      Coordinate result for LEAF_MIGHTY_LITTLE_GEODESY_1_0_1 : CrsCoordinate(xEastingLongitude=674032.357, yNorthingLatitude=6580821.991, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))
            // Note that the median value for "x" is 674032.357177155 for the above 
            // three values 674032.357 , 674032.357177155 , 674032.357322213 . 
            // That is the same value as was displayed before the iteration of the children/leafs for the median composite.
            // The same applies for the above "y" i.e. the median is 6580821.99121561
            // for the three y values 6580821.991 , 6580821.99121561 , 6580821.99437121
        }
        // The result object also provides convenience methods for the results (which you of course otherwise might calculate by iterating the above results)
        CrsTransformationResultStatistic crsTransformationResultStatistic = centralStockholmResultSweRef.CrsTransformationResultStatistic;
        // Note that the initially created composite was a "median composite" returning the median as the main value, 
        // but you can also create an average composite and regardless you can access both the median and the average with the aggregated statistics object:
        Console.WriteLine("average coordinate: " + crsTransformationResultStatistic.CoordinateAverage);
        Console.WriteLine("median coordinate: " + crsTransformationResultStatistic.CoordinateMedian);
        // Console output from the above two rows:
        // average coordinate: CrsCoordinate(xEastingLongitude=674032.357166456, yNorthingLatitude=6580821.99219561, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))
        // median coordinate: CrsCoordinate(xEastingLongitude=674032.357177155, yNorthingLatitude=6580821.99121561, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))

        Console.WriteLine("MaxDifferenceForXEastingLongitude: " + crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude);
        Console.WriteLine("MaxDifferenceForYNorthingLatitude: " + crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude);
        // Output from the above two rows:
        // MaxDifferenceForXEastingLongitude: 0.000322213280014694
        // MaxDifferenceForYNorthingLatitude: 0.00337121076881886
        // As you can see in the above iteration, the min and max x values are 674032.357 and 674032.357322213 (and the difference is 0.000322213).
        // Similarly the min and max y values are 6580821.991 and 6580821.99437121 (and the difference is 0.00337121).
        // The above two "MaxDifference" methods are used within the implementation of the convenience method 'isReliable' 
        // (also illustrated in this example further above)
    
    } // else statement ends

    Console.WriteLine("LargerCSharpeExample ends");
    Console.ReadLine();
} // method ends
} // class ends
} // namespace ends