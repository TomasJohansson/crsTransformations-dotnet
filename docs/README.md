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
		    <version>0628bbc6f1f2506f75c02313432fc8e249c9901d</version>
		</dependency> 	 
	  	<dependency>
			<groupId>com.github.TomasJohansson.crsTransformations</groupId>
			<artifactId>crs-transformation-adapter-impl-goober</artifactId>
			<version>0628bbc6f1f2506f75c02313432fc8e249c9901d</version>    	
	  	</dependency>
	  	<dependency>
			<groupId>com.github.TomasJohansson.crsTransformations</groupId>
			<artifactId>crs-transformation-adapter-impl-orbisgis</artifactId>
			<version>0628bbc6f1f2506f75c02313432fc8e249c9901d</version>
	  	</dependency>		
	  	<dependency>
			<groupId>com.github.TomasJohansson.crsTransformations</groupId>
			<artifactId>crs-transformation-adapter-impl-proj4j</artifactId>
			<version>0628bbc6f1f2506f75c02313432fc8e249c9901d</version>
	  	</dependency>
	  	<dependency>
			<groupId>com.github.TomasJohansson.crsTransformations</groupId>
			<artifactId>crs-transformation-adapter-impl-nga</artifactId>
			<version>0628bbc6f1f2506f75c02313432fc8e249c9901d</version>
	  	</dependency>	  	
	</dependencies>
```
Java example code working with the above "jitpack release":
```java
import java.util.Arrays;
import java.util.List;
import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import com.programmerare.crsTransformations.ResultsStatistic;
import com.programmerare.crsTransformations.TransformResult;
import com.programmerare.crsTransformations.compositeTransformations.AdapterWeight;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory;
import com.programmerare.crsTransformations.coordinate.Coordinate;
import com.programmerare.crsTransformations.coordinate.CoordinateFactory;
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier;
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifierFactory;

...
        double inputLatitude, inputLongitude;
        int inputCoordinateReferenceSystem, outputCoordinateReferenceSystem;
        Coordinate inputCoordinate;
        CrsTransformationAdapter crsTransformationAdapter; // interface implemented by "Leaf" (concrete) implementations
                                                           // and "Composite" aggregating implementations

        inputLatitude = 500;
        inputLongitude = 500;
        inputCoordinateReferenceSystem = EpsgNumber.WORLD__WGS_84__4326;
        outputCoordinateReferenceSystem = EpsgNumber.SWEDEN__SWEREF99_TM__3006;
        inputCoordinate = CoordinateFactory.createFromYLatitudeXLongitude(inputLatitude, inputLongitude, inputCoordinateReferenceSystem);

        // crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage(); // alternative "Composite" implementation
        crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();
        transformCoordinate(inputCoordinate, outputCoordinateReferenceSystem, crsTransformationAdapter);
        // Currently there are two more "Composite" implementations:
        // "ChainOfResponsibility" normally not using all implementations but only until some implementation seem to have handled the lookup
        // "WeightedAverage" using all but if you consider some implementation to be more reliable you can specify the weights
        crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationChainOfResponsibility();
        crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(
            Arrays.asList(
                // The weights are RELATIVE values which do NOT need to sum up to a certain value such as 1 or 100
                // and those relative weights are applied for those that claim a "successful" transformation
                // i.e. those implementations that did not throw exception or indicated failure in some other way such as return NaN as the value.
                AdapterWeight.createFromInstance(new CrsTransformationAdapterGooberCTL(), 1),
                AdapterWeight.createFromInstance(new CrsTransformationAdapterOrbisgisCTS(), 3),
                AdapterWeight.createFromInstance(new CrsTransformationAdapterProj4J(), 7),
                AdapterWeight.createFromInstance(new CrsTransformationAdapterGeoPackageNGA(), 10)
            )
        );
        transformCoordinate(inputCoordinate, outputCoordinateReferenceSystem, crsTransformationAdapter);

        

        crsTransformationAdapter = new CrsTransformationAdapterGooberCTL();
        transformCoordinate(inputCoordinate, outputCoordinateReferenceSystem, crsTransformationAdapter);
        
        inputLatitude = 55;
        inputLongitude = 30;
        inputCoordinate = CoordinateFactory.createFromYLatitudeXLongitude(inputLatitude, inputLongitude, inputCoordinateReferenceSystem);        
        transformCoordinate(inputCoordinate, outputCoordinateReferenceSystem, crsTransformationAdapter);        
        
        crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage();
        transformCoordinate(inputCoordinate, outputCoordinateReferenceSystem, crsTransformationAdapter);        
        
...
    private static void transformCoordinate(Coordinate inputCoordinate, int outputCoordinateReferenceSystem,
        CrsTransformationAdapter crsTransformationAdapter // interface with both "Leaf" and "Composite" implementations
    ) {
        System.out.println("------------------------------------------------------");
        TransformResult transformResult = crsTransformationAdapter.transform(inputCoordinate,
                outputCoordinateReferenceSystem);
        System.out.println(crsTransformationAdapter.getNameOfImplementation());
        if (!transformResult.isSuccess()) {
            System.out.println("Transformation failed");
            return;
        }
        Coordinate outputCoordinate = transformResult.getOutputCoordinate();
        System.out.println(outputCoordinate);
        if (!transformResult.isReliable(2, // minimumNumberOfSuccesfulResults
                1 // maxDeltaValueForXLongitudeAndYLatitude
        )) {
            System.out.println("Unreliable result");
            ResultsStatistic resultsStatistic = transformResult.getResultsStatistic();
            System.out.println("numberOfResults: " + resultsStatistic.getNumberOfResults());
            System.out.println("MaxDiffXLongitude: " + resultsStatistic.getMaxDiffXLongitude());
            System.out.println("MaxDiffYLatitude: " + resultsStatistic.getMaxDiffYLatitude());
            System.out.println("CoordinateAverage: " + resultsStatistic.getCoordinateAverage());
            System.out.println("CoordinateAverage: " + resultsStatistic.getCoordinateMedian());
            List<TransformResult> transformResults = transformResult.getSubResults();
            for (TransformResult subresult : transformResults) {
                if (subresult.isSuccess()) {
                    System.out.println(subresult.getOutputCoordinate() + " , "
                            + subresult.getCrsTransformationAdapterThatCreatedTheResult().getNameOfImplementation());
                } else {
                    System.out.println("Failed: "
                            + subresult.getCrsTransformationAdapterThatCreatedTheResult().getNameOfImplementation());
                }
            }

        }
    }
```
