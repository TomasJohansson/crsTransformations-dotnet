# License Notice
Notice that the "Core" library with the adapter API and general code is released with MIT License.  
However, the adapter implementations libraries are licensed in the same way as the adapted libraries which is specified in separate "LICENSE_NOTICE" files (in the adapter base directories) for each such implementation.

# Information about this Coordinate Reference System Transformations library
This F#/C#/.NET project is intended for transforming coordinates between different coordinate reference systems (CRS).  
The code has been implemented with F# but the tests (and the generated constants in the subproject "Programmerare.CrsTransformations.Constants") are implemented with C#.  
The third-part libraries (the adaptee's below) are .NET libraries.  
.NET versions supported in the upcoming NuGet release: .NET 4.5 and .NET Standard 2.0  

# TODO
The rest of the page will be updated for this .NET project.  
It has been "ported" from the corresponding JVM project '[crsTransformations](https://github.com/TomasJohansson/crsTransformations)' (Kotlin/Java) and the below text has not yet been updated for this .NET version.  



# Usage
The methods for transforming coordinates are defined in the interface *CrsTransformationAdapter*.  
There are nine classes implementing the interface. Five 'leafs' and four 'composites'.  
Each leaf adapter is using some adaptee library for the implementation.  
The four 'composites' are using the leafs like this:
* **Median** (transform with many leafs and use the *median* latitude/longitude result as the aggregated result)
* **Average** (transform with many leafs and use the *average* latitude/longitude result as the aggregated result)
* **Weighted average** (transform with many leafs and use the *weighted* average latitude/longitude result as the aggregated result)
* **First success** (iterate a list of leafs and try to transform until some result seem to have succeeded)
  
Java:
```java
        // The interface with nine implementations as illustrated below
        CrsTransformationAdapter crsTransformationAdapter; 
        // The interface is defined in the library "crs-transformation-adapter-core" with this full name:
        // com.programmerare.crsTransformations.CrsTransformationAdapter        
        
        // The five 'Leaf' implementations:

        // Library "crs-transformation-adapter-impl-proj4j", class:
        // com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
        crsTransformationAdapter = new CrsTransformationAdapterProj4J();

        // Library "crs-transformation-adapter-impl-orbisgis", class:
        // com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;        
        crsTransformationAdapter = new CrsTransformationAdapterOrbisgisCTS();

        // Library "crs-transformation-adapter-impl-nga", class:
        // com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
        crsTransformationAdapter = new CrsTransformationAdapterGeoPackageNGA();

        // Library "crs-transformation-adapter-impl-geotools", class:
        // com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;        
        crsTransformationAdapter = new CrsTransformationAdapterGeoTools();

        // Library "crs-transformation-adapter-impl-goober", class:
        // com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;        
        crsTransformationAdapter = new CrsTransformationAdapterGooberCTL();
        // - - - - - - - - - - - -
        
        // The four 'Composite' implementations below are all located in the library
        // "crs-transformation-adapter-core" and the factory class is:
        // com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory
        
        crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();
        
        crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage();
        
        crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationFirstSuccess();
        
        // All of the above three factory methods without any parameter will try to use as many of the five 'leaf' 
        // implementations as are available at the class path (e.g. are included as dependencies with Gradle or Maven).
        // All above three factory methods are also overloaded with methods taking 
        // a parameter 'List<CrsTransformationAdapter>' if you prefer to explicit define which 'leafs' to use.
        
        // The fourth 'Composite' below does not have any overloaded method without parameter 
        // but if you want to use a result created as a weighted average then the weights need 
        // to be specified per leaf implementation as in the example below.

        crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(Arrays.asList(
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterProj4J(), 1.0),
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterOrbisgisCTS(), 1.0),
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoPackageNGA(), 1.0),
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoTools(), 1.0),
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGooberCTL(), 2.0)
        ));
        // The weight values above illustrates a situation where you (for some reason) want to consider 
        // the transformation result from 'goober' as being 'two times better' than the others.
```
All of the transform methods (defined in the above interface *CrsTransformationAdapter*) need two parameters, one input coordinate and one parameter specifying the target system i.e. to which coordinate reference system the input coordinate will be transformed to.  

The target system can be specified with three data types (i.e. with overloaded methods), either an integer or a string, or a 'CrsIdentifier'.  
If an integer or string is used, then internally an 'CrsIdentifier' will be created, to send it as parameter to the adapter implementations.  
Therefore, if you intend to do many transformation from or to a coordinate reference system, then you may choose to create an instace of CrsIdentifier yourself, but for more convenience you may want to use the overloaded methods with integer as parameter (or maybe string if you already have acess to some "EPSG:"-string, see example below).

A coordinate includes information about the coordinate reference system, i.e. a 'CrsIdentifier' but there are also factory methods (creating coordinate instances) which are overloaded with integer or string parameters.  

There are many factory methods with different names and different order for the two (x/y) position values as illustated in the example below.  
Depending on the desired semantic in your context, you may want to use the different methods (or similarly named accessors in *CrsCoordinate*) like this:  
* x/y for a geocentric or cartesian system
* longitude/latitude for a geodetic or geographic system
* easting/northing for a cartographic or projected system
* xEastingLongitude/yNorthingLatitude for general code handling different types of system
  
Java:
```java
        final int epsgNumber = 4326;
        final String crsCode = "EPSG:" + epsgNumber;
        CrsIdentifier crsIdentifier; // package com.programmerare.crsTransformations.crsIdentifier
        crsIdentifier = CrsIdentifierFactory.createFromEpsgNumber(epsgNumber);
        // Alternative:
        crsIdentifier = CrsIdentifierFactory.createFromCrsCode(crsCode);

        final double latitude = 59.330231;
        final double longitude = 18.059196;

        CrsCoordinate crsCoordinate; // package com.programmerare.crsTransformations.coordinate
        // All the below methods are alternatives for creating the same coordinate 
        // with the above latitude/longitude and coordinate reference system.
        // No class or object is used for the methods below because of the following static import:
        // import static com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory.*;
        crsCoordinate = latLon(latitude, longitude, epsgNumber);
        crsCoordinate = latLon(latitude, longitude, crsCode);
        crsCoordinate = latLon(latitude, longitude, crsIdentifier);
        
        crsCoordinate = lonLat(longitude, latitude, epsgNumber);
        crsCoordinate = lonLat(longitude, latitude, crsCode);
        crsCoordinate = lonLat(longitude, latitude, crsIdentifier);
        
        crsCoordinate = yx(latitude, longitude, epsgNumber);
        crsCoordinate = yx(latitude, longitude, crsCode);
        crsCoordinate = yx(latitude, longitude, crsIdentifier);
        
        crsCoordinate = xy(longitude, latitude, epsgNumber);
        crsCoordinate = xy(longitude, latitude, crsCode);
        crsCoordinate = xy(longitude, latitude, crsIdentifier);

        crsCoordinate = northingEasting(latitude, longitude, epsgNumber);
        crsCoordinate = northingEasting(latitude, longitude, crsCode);
        crsCoordinate = northingEasting(latitude, longitude, crsIdentifier);

        crsCoordinate = eastingNorthing(longitude, latitude, epsgNumber);
        crsCoordinate = eastingNorthing(longitude, latitude, crsCode);
        crsCoordinate = eastingNorthing(longitude, latitude, crsIdentifier);

        crsCoordinate = createFromYNorthingLatitudeAndXEastingLongitude(latitude, longitude, epsgNumber);
        crsCoordinate = createFromYNorthingLatitudeAndXEastingLongitude(latitude, longitude, crsCode);
        crsCoordinate = createFromYNorthingLatitudeAndXEastingLongitude(latitude, longitude, crsIdentifier);

        crsCoordinate = createFromXEastingLongitudeAndYNorthingLatitude(longitude, latitude, epsgNumber);
        crsCoordinate = createFromXEastingLongitudeAndYNorthingLatitude(longitude, latitude, crsCode);
        crsCoordinate = createFromXEastingLongitudeAndYNorthingLatitude(longitude, latitude, crsIdentifier);

        
        CrsIdentifier targetCrs = CrsIdentifierFactory.createFromEpsgNumber(3006);
        CrsTransformationResult crsTransformationResult = crsTransformationAdapter.transform(crsCoordinate, targetCrs);
        // see more example code further down in this webpage
```
    

# Adaptee libraries used by the adapter libraries in the first release
* https://github.com/Proj4J/proj4j
    (version 0.1.0)
* https://github.com/orbisgis/cts/
    (version 1.5.1)
* https://github.com/ngageoint/geopackage-java
    (version 3.1.0)
* https://github.com/geotools/geotools
    (version 20.0)
* https://github.com/goober/coordinate-transformation-library
    (version 1.1)

# Released library versions
The following seven artifacts from this code project have been released/distributed to the Maven "Central Repository" ([Sonatype OSSRH](https://central.sonatype.org/pages/ossrh-guide.html) "Open Source Software Repository Hosting Service"):
* crs-transformation-adapter-**core**
    (version 1.0.0)
* crs-transformation-adapter-*impl*-**proj4j**
    (version 1.0.0)
* crs-transformation-adapter-*impl*-**orbisgis**
    (version 1.0.0)
* crs-transformation-adapter-*impl*-**nga**
    (version 1.0.0)
* crs-transformation-adapter-*impl*-**geotools**
    (version 1.0.0, the adaptee is currently not at Maven Central, see comment below)
* crs-transformation-adapter-*impl*-**goober**
    (version 1.0.0, only used for Swedish CRS, see comment below)
* crs-transformation-*constants*
    (version **9.5.4**)  

The five above libraries which includes "*impl*" in the name are adapter implementations of the above "*core*" library.  
Those five adapters are using the five adaptee libraries for the coordinate transformations.  

The above '*goober*' library is only useful for transformation between WGS84 (which is a very common global CRS) and the Swedish coordinate reference systems (CRS) SWEREF99 (13 versions e.g. "SWEREF99 TM") and RT90 (6 versions e.g. "RT90 2.5 gon V").   

The above '*geotools*' library are using geotools which currently seem to not be distributed to "Maven Central" but can be used by adding an additional repository as in the Gradle and Maven sections of this webpage.  

The above artifact "crs-transformation-*constants*" is actually totally independent from the others.  
It is not depending on anything and nothing depends on it.  
It is a **Java** library (i.e. not even depending on Kotlin) with only one class with a lot of Java constants.  
(the other six artifacts/libraries are implemented with Kotlin and thus have an implicit dependency to a Kotlin artifact)    
The constant class has been generated from the [EPSG database](http://www.epsg-registry.org) version 9.5.4 which is the reason for the version number.

# Gradle configuration
The "core" library is not necessary to include since there is an implicit/transitive dependency from all the "impl" libraries to the "core".  
The "constants" library is not needed but might be interesting if you want to use constants 
for the EPSG numbers rather than hardcoding them or define your own integer constants.  
The "repository" for geotools is only needed if you want to use the library for geotools.

build.gradle
```Groovy
...
repositories {
    maven {
        // this repository can be added if you want to use the implementation 
        // "crs-transformation-adapter-impl-geotools" which uses the "geotools" library     
        url "https://download.osgeo.org/webdav/geotools/"
    }    
    mavenCentral()
}
...

dependencies {
    ...
    implementation "com.programmerare.crs-transformation:crs-transformation-adapter-impl-proj4j:1.0.0"	
    implementation "com.programmerare.crs-transformation:crs-transformation-adapter-impl-orbisgis:1.0.0"
    implementation "com.programmerare.crs-transformation:crs-transformation-adapter-impl-nga:1.0.0"
    implementation "com.programmerare.crs-transformation:crs-transformation-adapter-impl-geotools:1.0.0" // include the above geotools repository
    implementation "com.programmerare.crs-transformation:crs-transformation-adapter-impl-goober:1.0.0" // only swedish CRS	
    
    implementation "com.programmerare.crs-transformation:crs-transformation-constants:9.5.4" // only one class with constants 
    ...
}
```

# Maven configuration
The comments at the above section with Gradle configuration applies for Maven too, e.g. the core library is not necessary to include explicitly.  

pom.xml 
```xml
    ...
    <properties>
        ...
        <crsTransformationVersion>1.0.0</crsTransformationVersion>
    </properties>
    ...
    <dependencies>
        ...    
        <dependency>
            <groupId>com.programmerare.crs-transformation</groupId>
            <artifactId>crs-transformation-adapter-impl-proj4j</artifactId>
            <version>${crsTransformationVersion}</version>
        </dependency>
        
        <dependency>
            <groupId>com.programmerare.crs-transformation</groupId>
            <artifactId>crs-transformation-adapter-impl-orbisgis</artifactId>
            <version>${crsTransformationVersion}</version>    	
        </dependency>
        
        <dependency>
            <groupId>com.programmerare.crs-transformation</groupId>
            <artifactId>crs-transformation-adapter-impl-nga</artifactId>
            <version>${crsTransformationVersion}</version>
        </dependency>
        
        <dependency>
            <!--  if using geotools you should also add a repository as shown further down in this example configuration -->
            <groupId>com.programmerare.crs-transformation</groupId>
            <artifactId>crs-transformation-adapter-impl-geotools</artifactId>
            <version>${crsTransformationVersion}</version>
        </dependency>
        
        <dependency>
            <!-- this is a small library only supporting the global CRS WGS84 and the Swedish coordinate reference systems SWEREF99 and RT90  -->
            <groupId>com.programmerare.crs-transformation</groupId>
            <artifactId>crs-transformation-adapter-impl-goober</artifactId>
            <version>${crsTransformationVersion}</version>      
        </dependency>
        
        <dependency>
            <!-- optional INDEPENDENT artifact with only one class with lots of Java integer constants -->
            <groupId>com.programmerare.crs-transformation</groupId>
            <artifactId>crs-transformation-constants</artifactId>
            <version>9.5.4</version>
        </dependency>
        ...
    </dependencies>        
    ...
    <repositories>
        <repository>
            <!-- osgeo repository for geotools  -->	        
            <id>osgeo.org</id>
            <url>https://download.osgeo.org/webdav/geotools/</url>
        </repository>
    </repositories>
    ...
```

# Kotlin example

Below is a small Kotlin example code working with the current version 1.0.0.  
The example code transforms a coordinate from a global CRS WGS84 (EPSG code 4326) latitude/longitude to
the Swedish CRS SWEREF99TM (EPSG code 3006).

SmallKotlinExample.kt
```kotlin
package smallKotlinExample

import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian
import com.programmerare.crsTransformations.coordinate.latLon

fun main(args: Array<String>) {
    val epsgWgs84 = 4326
    val epsgSweRef = 3006
    // alternative to the above hardcoding: use the library "crs-transformation-constants"
    // and constants EpsgNumber.WORLD__WGS_84__4326 and EpsgNumber.SWEDEN__SWEREF99_TM__3006
    // from the Java class com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber

    val centralStockholmWgs84 = latLon(59.330231, 18.059196, epsgWgs84)

    val crsTransformationAdapter = createCrsTransformationMedian()
    // If the Gradle/Maven configuration includes all five adapter implementations, then the 
    // above created 'Composite' implementation will below use all five 'leaf' implementations 
    // and return a coordinate with a median longitude and a median latitude
    val centralStockholmResultSweRef = crsTransformationAdapter.transform(centralStockholmWgs84, epsgSweRef)
    if (centralStockholmResultSweRef.isSuccess) {
        println(centralStockholmResultSweRef.outputCoordinate)
        // Console output from the above code row: 
        // Coordinate(xEastingLongitude=674032.3573261796, yNorthingLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
    }
}
```

# Java examples
Below is a small Java example code working with the current version 1.0.0.  
The example code transforms a coordinate from a global CRS WGS84 (EPSG code 4326) latitude/longitude to
the Swedish CRS SWEREF99TM (EPSG code 3006).  
```java
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import com.programmerare.crsTransformations.CrsTransformationResult;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;

public class SmallJavaExample {
    public static void main(String[] args) {
        final int epsgWgs84  = 4326;
        final int epsgSweRef = 3006;
        // alternative to the above hardcoding: use the library "crs-transformation-constants"
        // and constants EpsgNumber.WORLD__WGS_84__4326 and EpsgNumber.SWEDEN__SWEREF99_TM__3006
        // from the class com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber
         
        CrsCoordinate centralStockholmWgs84 = CrsCoordinateFactory.latLon(59.330231, 18.059196, epsgWgs84);
        
        CrsTransformationAdapter crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();
        // If the Gradle/Maven configuration includes all five adapter implementations, then the 
        // above created 'Composite' implementation will below use all five 'leaf' implementations 
        // and return a coordinate with a median longitude and a median latitude
        CrsTransformationResult centralStockholmResultSweRef = crsTransformationAdapter.transform(centralStockholmWgs84, epsgSweRef);
        
        if(centralStockholmResultSweRef.isSuccess()) {
            System.out.println(centralStockholmResultSweRef.getOutputCoordinate());
            // Console output from the above code row: 
            // Coordinate(xEastingLongitude=674032.3573261796, yNorthingLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
        }
    }
}
```

Another Java example with some more code and comments: 
```java
import java.util.List;
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
    // The above "EpsgNumber" class with LOTS OF constants (and more constants classes) have been generated, 
    // using "FreeMarker" and database downloaded from EPSG ( http://www.epsg.org or http://www.epsg-registry.org ) 
    
    CrsCoordinate centralStockholmWgs84 = CrsCoordinateFactory.latLon(59.330231, 18.059196, epsgWgs84);
    // https://kartor.eniro.se/m/03Yxp
    // SWEREF99TM coordinates (for WGS84 59.330231, 18.059196) 
    // according to Eniro (above URL): 6580822, 674032 (northing, easting)
    
    CrsTransformationAdapter crsTransformationAdapter; // interface with concrete "leaf" implementation or "composite" implementations
    // This code example is using a "composite" which will use multiple libraries to do the same transformation and then 
    // return a coordinate with the median values (median of the northing values and median of the easting values)  
    crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();
    // The above factory will try to use those known objects which implements the interface i.e. the number 
    // of "leaf" objects will depend on how many you included in for example the maven pom file (five in the above maven example)
    System.out.println("Number of 'leafs' : " + crsTransformationAdapter.getTransformationAdapterChildren().size());
    // Console output from the above row:
    // Number of 'leafs' : 5

    // Transform the WGS84 coordinate to a SWEREF99TM coordinate:
    CrsCoordinate centralStockholmSweRef = crsTransformationAdapter.transformToCoordinate(centralStockholmWgs84, epsgSweRef);
    System.out.println("Median Composite Northing: " + centralStockholmSweRef.getNorthing());        
    System.out.println("Median Composite Easting: " + centralStockholmSweRef.getEasting());      
    // Console output from the above two rows:
    //      Median Composite Northing: 6580821.991123579
    //      Median Composite Easting: 674032.3573261796        
    // (and these can be compared with the 'Eniro' values above i.e. '6580822, 674032 (northing, easting)' )
    
    // The coordinate class provides four methods with different names for the same east-west value and 
    // four methods for the same name each north-south value, as below:
    //      Four EQUIVALENT methods:  getEasting  , getX , getLongitude , getXEastingLongitude
    //      Four EQUIVALENT methods:  getNorthing , getY , getLatitude  , getYNorthingLatitude
    // Regarding the above alternative methods, depending on the desired semantic in your context, you may want to use:
    //      x/y for a geocentric or cartesian system
    //      longitude/latitude for a geodetic or geographic system
    //      easting/northing for a cartographic or projected system
    //      xEastingLongitude/yNorthingLatitude for general code handling different types of system
    
    // If you want more details for the result you can use the following 'transform' method: 
    //  (instead of the method 'transformToCoordinate' used above)
    CrsTransformationResult centralStockholmResultSweRef = crsTransformationAdapter.transform(centralStockholmWgs84, epsgSweRef);
    if(!centralStockholmResultSweRef.isSuccess()) {
        System.out.println("No coordinate result");
    }
    else {
        if(centralStockholmResultSweRef.isReliable(
            4,      //  minimumNumberOfSuccesfulResults, 
            0.01    // maxDeltaValueForXLongitudeAndYLatitude
        )) {
            // at least 4 succesful results and the maximal difference in northing or easting is less than 0.01
            // (and if you want to know the exact difference you can find it in this code example further down the page)
            System.out.println("Reliable result"); // according to your chosen parameters to the method 'isReliable'    
        }
        else {
            System.out.println("Not reliable result");
        }
        System.out.println(centralStockholmResultSweRef.getOutputCoordinate());
        // Console output from the above code row:
        // Coordinate(xEastingLongitude=674032.3573261796, yNorthingLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
        
        // When your code is in a context where you only have the result (but not the adapter object) 
        // (e.g. in a method receiving the result as a parameter)
        // you can get back the object which created the result as below:
        CrsTransformationAdapter crsTransformationAdapterResultSource = centralStockholmResultSweRef.getCrsTransformationAdapterResultSource();
        CrsTransformationAdapteeType adapteeType = crsTransformationAdapterResultSource.getAdapteeType();
        System.out.println("adapteeType: " + adapteeType); // console output: COMPOSITE_MEDIAN
        // The above code row returned an enum which is not really a true adaptee just like the 'composite' is not a true adapter.
        // However, when iterating (as below) the "leaf" results, 
        // it might be more interesting to keep track of from where the different values originated
        List<CrsTransformationResult> transformationResultChildren = centralStockholmResultSweRef.getTransformationResultChildren();
        for (CrsTransformationResult crsTransformationResultLeaf : transformationResultChildren) {
            if(!crsTransformationResultLeaf.isSuccess()) continue; // continue with the next 'leaf'
            
            CrsTransformationAdapter resultAdapter = crsTransformationResultLeaf.getCrsTransformationAdapterResultSource();
            System.out.println(resultAdapter.getAdapteeType());
            // The above code row will output rows like this: "LEAF_GOOBER_1_1" or "LEAF_NGA_GEOPACKAGE_3_1_0" and so on
            if(!crsTransformationResultLeaf.isReliable(
                    2,      //  minimumNumberOfSuccesfulResults, 
                    1000    // maxDeltaValueForXLongitudeAndYLatitude
            )) {
                // The above constraint "at least 2 implementations" will fail because now we are dealing with "leafs"
                // The above delta value constraint has very high tolerance but it does not matter since 
                // the constraint about the number of implementations will fail
                System.out.println("Only 'composites' can have more than one result and this is a 'leaf' and thus does not at least two results");
            }
            System.out.println("Adapter long name: " + resultAdapter.getLongNameOfImplementation()); // full class name including package
            System.out.println("Adapter short name: " + resultAdapter.getShortNameOfImplementation()); // class name suffix i.e. the unique part
            // The above "long" names will be for example:
            //      com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
            //      com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
            // The above "short" names will be for example:
            //      OrbisgisCTS
            //      Proj4J
            System.out.println("adaptee: " + resultAdapter.getAdapteeType());
            // The above row will output for example:
            //      LEAF_ORBISGIS_1_5_1
            //      LEAF_PROJ4J_0_1_0
            // (note that the version number is included for the adaptees)
            System.out.println("isComposite: " + resultAdapter.isComposite()); // "false" since we are iterating "leaf" results
            System.out.println("Coordinate result for " + resultAdapter.getAdapteeType() + " : " + crsTransformationResultLeaf.getOutputCoordinate());
            // The above row will output these rows when doing the iteration:
            //      Coordinate result for LEAF_GOOBER_1_1 : Coordinate(xEastingLongitude=674032.357, yNorthingLatitude=6580821.991, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
            //      Coordinate result for LEAF_NGA_GEOPACKAGE_3_1_0 : Coordinate(xEastingLongitude=674032.357326444, yNorthingLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
            //      Coordinate result for LEAF_GEOTOOLS_20_0 : Coordinate(xEastingLongitude=674032.3571771547, yNorthingLatitude=6580821.994371211, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
            //      Coordinate result for LEAF_ORBISGIS_1_5_1 : Coordinate(xEastingLongitude=674032.3573261796, yNorthingLatitude=6580821.991121078, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
            //      Coordinate result for LEAF_PROJ4J_0_1_0 : Coordinate(xEastingLongitude=674032.357326444, yNorthingLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
            // Note that the median value for "x" is 674032.3573261796 for the above 
            // five x values 674032.357 , 674032.357326444 , 674032.3571771547 , 674032.3573261796 , 674032.357326444 . 
            // That is the same value as was displayed before the iteration of the children/leafs for the median composite.
            // The same applies for the above "y" i.e. the median is 6580821.991123579
            // for the five y values 6580821.991 , 6580821.991123579 , 6580821.994371211 , 6580821.991121078 , 6580821.991123579
        }
        // The result object also provides convenience methods for the results (which you of course otherwise might calculate by iterating the above results)
        CrsTransformationResultStatistic crsTransformationResultStatistic = centralStockholmResultSweRef.getCrsTransformationResultStatistic();
        // Note that the initially created composite was a "median composite" returning the median as the main value, 
        // but you can also create an average composite and regardless you can access both the median and the average with the aggregated statistics object:
        System.out.println("average coordinate: " + crsTransformationResultStatistic.getCoordinateAverage());
        System.out.println("median coordinate: " + crsTransformationResultStatistic.getCoordinateMedian());            
        // Console output from the above two rows:
        // average coordinate: Coordinate(xEastingLongitude=674032.3572312444, yNorthingLatitude=6580821.991747889, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))
        // median coordinate: Coordinate(xEastingLongitude=674032.3573261796, yNorthingLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=true, epsgNumber=3006))            

        System.out.println("MaxDifferenceForXEastingLongitude: " + crsTransformationResultStatistic.getMaxDifferenceForXEastingLongitude());
        System.out.println("MaxDifferenceForYNorthingLatitude: " + crsTransformationResultStatistic.getMaxDifferenceForYNorthingLatitude());
        // Output from the above two rows:
        // MaxDifferenceForXEastingLongitude: 3.264440456405282E-4
        // MaxDifferenceForYNorthingLatitude: 0.0033712107688188553
        // As you can see in the above iteration, the min and max x values are 674032.357 and 674032.357326444 (and the difference is 0.000326444).
        // Similarly the min and max y values are 6580821.991 and 6580821.994371211 (and the difference is 0.003371211).
        // The above two "MaxDifference" methods are used within the implementation of the convenience method 'isReliable' 
        // (also illustrated in this example further above)
    }
```
# The library "crs-transformation-constants" with EPSG integer constants
The integer constants are generated by using the [EPSG database](http://www.epsg-registry.org).  
Each constant is used for defining a coordinate reference system (CRS).  
The name of a constant is a concatenation of the following three parts:
* Name of the area (e.g. country)
* Name of the CRS
* EPSG number (also the integer value for the constant)

![EPSG_SQL](images/epsg_db.png)  
The above three parts are concatenated (in the above order) with two underscores as separator between the parts.  
Spaces and other special characters are replaced with underscores.  
The names are also uppercased.  

Screenshots from Intellij IDEA when using intellisense/autocompletion with the class EpsgNumber:
![epsg_dropdown_usa](images/epsg_dropdown_usa.png)  
![epsg_dropdown_sweden](images/epsg_dropdown_sweden.png)  

Some examples of constant names in the Java class [com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber](https://github.com/TomasJohansson/crsTransformations/blob/master/crs-transformation-constants/src/main/java/com/programmerare/crsConstants/constantsByAreaNameNumber/v9_5_4/EpsgNumber.java):
```java
WORLD__WGS_84__4326  
WORLD__85_S_TO_85_N__WGS_84__PSEUDO_MERCATOR__3857  
NORTH_AMERICA__NAD83__NAD83__4269  
USA__US_NATIONAL_ATLAS_EQUAL_AREA__2163  
CANADA__NAD83__CANADA_ATLAS_LAMBERT__3978  
UK__GREAT_BRITAIN_MAINLAND_ONSHORE__OSGB_1936__BRITISH_NATIONAL_GRID__ODN_HEIGHT__7405  
IRELAND__ONSHORE__TM65__IRISH_GRID__29902  
AUSTRALIA__GDA__GDA94__4283  
NEW_ZEALAND__ONSHORE__NZGD49__NEW_ZEALAND_MAP_GRID__27200  
SWEDEN__SWEREF99_TM__3006  
FINLAND__19_5_E_TO_20_5_E_ONSHORE_NOMINAL__ETRS89__GK20FIN__3874  
DENMARK__ONSHORE_BORNHOLM__ETRS89__KP2000_BORNHOLM__2198  
NORWAY__ONSHORE__NGO_1948__OSLO__4817  
ICELAND__ONSHORE__REYKJAVIK_1900__4657  
NETHERLANDS__ONSHORE__AMERSFOORT__RD_NEW__28992  
BELGIUM__ONSHORE__BELGE_1972__BELGIAN_LAMBERT_72__31370  
GERMANY__WEST_GERMANY__10_5_E_TO_13_5_E__DHDN__3_DEGREE_GAUSS_KRUGER_ZONE_4__31468  
AUSTRIA__ETRS89__AUSTRIA_LAMBERT__3416          
EUROPE__LIECHTENSTEIN_AND_SWITZERLAND__CH1903__LV03__21781  
```
The above examples with constants are just showing a very few of them.  
The actual number of constants in the generated class is 6733.  
This number can easily be retrieved with the Java reflection code 'EpsgNumber.class.getFields().length'.  
The same number can also be verified with the value returned by the SQL statement 'SELECT count(DISTINCT coord_ref_sys_code) FROM epsg_coordinatereferencesystem' (or simply 'SELECT count(*) FROM epsg_coordinatereferencesystem' since 'coord_ref_sys_code' is the primary key).
