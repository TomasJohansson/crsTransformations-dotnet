# License Notice
Notice that the "core" library with the API and general code is released with MIT License.
However, the libraries which are implementing adapters/facades are licensed in the same way as the adapted libraries which is specified in separate "LICENSE_NOTICE" files for each such implementation.

# Release information
This code project is work in progress.
Currently no artifact has been distributed to the "Central Repository" ([Sonatype OSSRH](https://central.sonatype.org/pages/ossrh-guide.html) "Open Source Software Repository Hosting Service").
However, it still possible to use through [jitpack](https://jitpack.io)  (the [build based on git commit 0628bbc6f1f2506f75c02313432fc8e249c9901d](https://jitpack.io/com/github/TomasJohansson/crsTransformations/0628bbc6f1f2506f75c02313432fc8e249c9901d/build.log)).
In the maven pom.xml you can add jitpack as below:
```xml
	<repositories>
		<repository>
		<id>jitpack.io</id>
		<url>https://jitpack.io</url>
		</repository>
	</repositories>
```

Then you can add the following dependencies to retrieve the jar files based on [git commit 0628bbc6f1f2506f75c02313432fc8e249c9901d](https://github.com/TomasJohansson/crsTransformations/commit/0628bbc6f1f2506f75c02313432fc8e249c9901d).

```xml
	<dependencies>
		<dependency>
		    <groupId>com.github.TomasJohansson.crsTransformations</groupId>
		    <artifactId>crs-transformation-constants</artifactId>
		    <version>2e48782a9b65ec41ddba0d58165c5c0140071a15</version>
		</dependency> 	 
	  	<dependency>
			<groupId>com.github.TomasJohansson.crsTransformations</groupId>
			<artifactId>crs-transformation-adapter-impl-goober</artifactId>
			<version>2e48782a9b65ec41ddba0d58165c5c0140071a15</version>    	
	  	</dependency>
	  	<dependency>
			<groupId>com.github.TomasJohansson.crsTransformations</groupId>
			<artifactId>crs-transformation-adapter-impl-orbisgis</artifactId>
			<version>2e48782a9b65ec41ddba0d58165c5c0140071a15</version>
	  	</dependency>		
	  	<dependency>
			<groupId>com.github.TomasJohansson.crsTransformations</groupId>
			<artifactId>crs-transformation-adapter-impl-proj4j</artifactId>
			<version>2e48782a9b65ec41ddba0d58165c5c0140071a15</version>
	  	</dependency>
	  	<dependency>
			<groupId>com.github.TomasJohansson.crsTransformations</groupId>
			<artifactId>crs-transformation-adapter-impl-nga</artifactId>
			<version>2e48782a9b65ec41ddba0d58165c5c0140071a15</version>
	  	</dependency>	  	
	</dependencies>
```
Java example code working with the above "jitpack release":
```java
import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.CrsTransformationAdapteeType;
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import com.programmerare.crsTransformations.CrsTransformationResult;
import com.programmerare.crsTransformations.CrsTransformationResultStatistic;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;

...
    // Some terminology regarding the names used in the below code example:
    // "CRS" = Coordinate Reference System
    // "WGS84" is the most frequently used coordinate system (e.g. the coordinates usually used in a GPS)    
    // "SWEREF99TM" is the official coordinate system used by authorities in Sweden
    // "EPSG" = "European Petroleum Survey Group" was (but the EPSG name is still often used) 
    //           an organization defining CRS with integer numbers e.g.  4326 for WGS84 or 3006 for SWEREF99TM
    int epsgWgs84  = EpsgNumber.WORLD__WGS_84__4326;
    int epsgSweRef = EpsgNumber.SWEDEN__SWEREF99_TM__3006;
    // The above "EpsgNumber" class with LOTS OF constants have been generated, using "FreeMarker" and database from EPSG ( http://www.epsg.org or http://www.epsg-registry.org ) 
    
    CrsCoordinate centralStockholmWgs84 = CrsCoordinateFactory.latLon(59.330231, 18.059196, epsgWgs84); // https://kartor.eniro.se/m/03Yxp
    // SWEREF99TM coordinates according to Eniro (above URL): 6580822, 674032 (northing, easting)
    
    CrsTransformationAdapter crsTransformationAdapter; // interface with concrete "leaf" implementation or "composite" implementations
    // This example is using a "composite" which will use multiple libraries to do the same transformation and then 
    // return a coordinate with the median values (median of the northing values and median of the easting values)  
    crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();

    // Transform the WGS84 coordinate to a SWEREF99TM coordinate:
    CrsCoordinate centralStockholmSweRef = crsTransformationAdapter.transformToCoordinate(centralStockholmWgs84, epsgSweRef);
    System.out.println("Median Composite Easting: " + centralStockholmSweRef.getEasting());      // Median Composite Easting: 674032.3573263118
    System.out.println("Median Composite Northing: " + centralStockholmSweRef.getNorthing());    // Median Composite Northing: 6580821.991122329
    // The coordinate class provides four methods with different names for the same east-west value and 
    // four methods for the same name each north-south value.
    // Equivalent methods:  getEasting  , getX , getLongitude , getXEastingLongitude
    // Equivalent methods:  getNorthing , getY , getLatitude  , getYNorthingLatitude
    // You may want to use:
    //  x/y for a geocentric or cartesian system
    //  longitude/latitude for a geodetic or geographic system
    //  easting/northing for a cartographic or projected system
    //  xEastingLongitude/yNorthingLatitude for general code handling different types of system
    
    // If you want more details for the result you can use the following 'transform' method instead of the above 'transformToCoordinate':
    CrsTransformationResult centralStockholmResultSweRef = crsTransformationAdapter.transform(centralStockholmWgs84, epsgSweRef);
    if(!centralStockholmResultSweRef.isSuccess()) {
        System.out.println("No coordinate result");
    }
    else {
        if(centralStockholmResultSweRef.isReliable(
            4,      //  minimumNumberOfSuccesfulResults, 
            0.01    // maxDeltaValueForXLongitudeAndYLatitude
        )) {
            // at least 4 results and the maximal difference in northing or easting is less than 0.01
            System.out.println("Reliable result");  
        }
        else {
            System.out.println("Not reliable result");
        }
        System.out.println(centralStockholmResultSweRef.getOutputCoordinate());
        // Console output from the above code row:
        // Coordinate(xEastingLongitude=674032.3573263118, yNorthingLatitude=6580821.991122329, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
        
        // When your code is in a context where you only have the result (e.g. in a method receiving the result as a parameter)
        // you can get back the object which created the result)
        CrsTransformationAdapter crsTransformationAdapterResultSource = centralStockholmResultSweRef.getCrsTransformationAdapterResultSource();
        CrsTransformationAdapteeType adapteeType = crsTransformationAdapterResultSource.getAdapteeType();
        System.out.println("adapteeType: " + adapteeType); // console output: COMPOSITE_MEDIAN
        // The above code row return an enum which is not really a true adaptee just like the composite is not a true adapter.
        // However, when iterating (as below) the "leaf" results, it might be more interesting to keep track of from where the different values originated
        List<CrsTransformationResult> transformationResultChildren = centralStockholmResultSweRef.getTransformationResultChildren();
        for (CrsTransformationResult crsTransformationResultLeaf : transformationResultChildren) {
            if(crsTransformationResultLeaf.isSuccess()) {
                CrsTransformationAdapter resultAdapter = crsTransformationResultLeaf.getCrsTransformationAdapterResultSource();
                System.out.println(resultAdapter.getAdapteeType());
                // The above code row till output rows like this: "LEAF_GOOBER_1_1" or "LEAF_NGA_GEOPACKAGE_3_1_0" and so on
                if(!crsTransformationResultLeaf.isReliable(2, 1000)) { // at least 2 implementations will fail because now we are dealing with "leafs"
                    System.out.println("Only 'composites' can have more than one result and this is a 'leaf' and thus does not at least two results");
                }
                System.out.println("Adapter long name: " + resultAdapter.getLongNameOfImplementation()); // full class name including package
                System.out.println("Adapter short name: " + resultAdapter.getShortNameOfImplementation()); // class name suffix i.e. the unique part
                // The above "long" names will be for example:
                //      com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
                //      com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
                // The above "long" names will be for example:
                //      OrbisgisCTS
                //      Proj4J
                System.out.println("adaptee: " + resultAdapter.getAdapteeType());
                // The above row will output for example:
                //      LEAF_ORBISGIS_1_5_1
                //      LEAF_PROJ4J_0_1_0
                // (note that the version number is included for the adaptees)
                System.out.println("isComposite: " + resultAdapter.isComposite()); // false since we are iteraing "leaf" results
                System.out.println("Coordinate result for " + resultAdapter.getAdapteeType() + " : " + crsTransformationResultLeaf.getOutputCoordinate());
                // The above row will output these rows when doing the iteration:
                //      Coordinate result for LEAF_GOOBER_1_1 : Coordinate(xEastingLongitude=674032.357, yNorthingLatitude=6580821.991, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
                //      Coordinate result for LEAF_NGA_GEOPACKAGE_3_1_0 : Coordinate(xEastingLongitude=674032.357326444, yNorthingLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
                //      Coordinate result for LEAF_ORBISGIS_1_5_1 : Coordinate(xEastingLongitude=674032.3573261796, yNorthingLatitude=6580821.991121078, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
                //      Coordinate result for LEAF_PROJ4J_0_1_0 : Coordinate(xEastingLongitude=674032.357326444, yNorthingLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
                // Note that the median value for "x" is 674032.3573263118 for the above values 674032.357 , 674032.357326444 , 674032.3573261796 , 674032.357326444 
                // (since it is the average of the two middle values 674032.357326444 and 674032.3573261796)
                // That is also the same as was displayed before the iteration of the children/leafs for the median composite
                // The same applies for the above "y" i.e. the median is 6580821.991122329 (average of two middle values 6580821.991123579 and 6580821.991121078)
                // for the four y values 6580821.991 , 6580821.991123579 , 6580821.991121078 , 6580821.991123579
            }
        }
        // The result object also provides convenience methods for the results (which you of course otherwise might calculate by iterating the above results)
        CrsTransformationResultStatistic crsTransformationResultStatistic = centralStockholmResultSweRef.getCrsTransformationResultStatistic();
        // Note that the initially created composite was a "median composite" returning the median as the main value, but 
        // you can also create an average composite but regardless, you can access both the median and the average with the aggregated statistics object:
        System.out.println("average coordinate: " + crsTransformationResultStatistic.getCoordinateAverage());
        System.out.println("median coordinate: " + crsTransformationResultStatistic.getCoordinateMedian());
        // Console output from the above two rows:
        //  average coordinate: Coordinate(xEastingLongitude=674032.357244767, yNorthingLatitude=6580821.99109206, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
        //  median coordinate: Coordinate(xEastingLongitude=674032.3573263118, yNorthingLatitude=6580821.991122329, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))

        System.out.println("MaxDiffXLongitude: " + crsTransformationResultStatistic.getMaxDiffXLongitude());
        System.out.println("MaxDiffYLatitude: " + crsTransformationResultStatistic.getMaxDiffYLatitude());
        // Output from the above two rows:
        //      MaxDiffXLongitude: 3.264440456405282E-4
        //      MaxDiffYLatitude: 1.2357905507087708E-4
        // As you can see in the above iteration, the min and max x values are 674032.357 and 674032.357326444 (and the difference is 0.000326444).
        // Similarly the min and max y values are 6580821.991 and 6580821.991123579 (and the difference is 0.000123579).
        // The above "maxDiff" methods are used within the implementation of the convenience method 'isReliable' (also illustrated in this example further above)

```
