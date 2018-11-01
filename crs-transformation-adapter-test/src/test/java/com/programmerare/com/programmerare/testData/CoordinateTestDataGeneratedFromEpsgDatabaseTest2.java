package com.programmerare.com.programmerare.testData;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsIdentifier;
import com.programmerare.crsTransformations.ResultsStatistic;
import com.programmerare.crsTransformations.TransformResult;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterComposite;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory;
import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Tag;
import org.junit.jupiter.api.Test;

import java.io.IOException;
import java.io.OutputStream;
import java.io.PrintStream;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeUnit;

import static com.programmerare.com.programmerare.testData.CoordinateTestDataGeneratedFromEpsgDatabaseTest.getCoordinatesFromGeneratedCsvFile;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

@Disabled // you may want to temporary change this line if you want to run the "tests"
public class CoordinateTestDataGeneratedFromEpsgDatabaseTest2 { // TODO better class name

    // Note that there are currently two methods 'findPotentialBuggyImplementations'
    // (one in this class and one in the class 'CoordinateTestDataGeneratedFromEpsgDatabaseTest')
    // Which are essentially trying to do the same thing i.e. transform back and forth
    // with lots of coordinates from a csv file.
    // However, the first will be based on having down those transformations and generated
    // output to different files, and then comparing those files.
    // This method below will instead use the normal composite adapter to do the transformations
    // with all leaf implementations and use methods of the result statistics object
    // to find problematic transformations with large differences.

    @Test // currently not a real test with assertions but printing console output with differences
    @Tag(TestCategory.SideEffectPrintingConsoleOutput)
    @Tag(TestCategory.SlowTest) // about 725 seconds iterating all 6435 rows (using the delta value 'double deltaDiff = 0.01' finding 80 results with big diffs)
    void findPotentialBuggyImplementations() {
        final CrsTransformationAdapterComposite crsTransformationComposite = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();
        verifyFiveImplementations(crsTransformationComposite); // to make sure that the above factory really creates an object which will use five implementations

        final List<TransformResult> transformResultsWithLargeDifferences = new ArrayList<TransformResult>();

        final CrsIdentifier wgs84 = CrsIdentifier.createFromEpsgNumber(EpsgNumber.WORLD__WGS_84__4326);

        final double deltaDiff = 0.01; // fairly large value to only find the large differences (potential large problems/bugs)
        // when the delta value above is 0.01 then about 80 EPSG codes are found i.e. there
        // are than many transformation (from and back that EPSG code) which will create
        // a bigger difference than 0.01 in either the latitude or longitude (or both)
        // between at least two of the implementations.

        final PrintStream nullStream = getNullStreamToAvoidOutputFromSystemOutAndSystemErr();
        // System.setOut/System.setErr will be used with the above but later restore with the two below
        final PrintStream outStream = System.out;
        final PrintStream errStream = System.err;

        long startTime = System.nanoTime();

        final List<EpsgCrsAndAreaCodeWithCoordinates> coordinatesFromGeneratedCsvFile = getCoordinatesFromGeneratedCsvFile();
        System.out.println("number of rows to iterate: " + coordinatesFromGeneratedCsvFile.size());
        for (int i = 0; i <coordinatesFromGeneratedCsvFile.size() ; i++) {
            System.setOut(outStream);
            System.setErr(errStream);
            if(i % 100 == 0) {
                System.out.println("i: " + i);
                long elapsedNanos = System.nanoTime() - startTime;
                long totalNumberOfSeconds = TimeUnit.NANOSECONDS.toSeconds(elapsedNanos);
                System.out.println("Number of seconds so far: " + totalNumberOfSeconds);
                // if(i > 500) break;
            }
            // now tries to avoid lots of output from the implementations (third part libraries used)
            System.setOut(nullStream);
            System.setErr(nullStream);

            final EpsgCrsAndAreaCodeWithCoordinates epsgCrsAndAreaCodeWithCoordinates = coordinatesFromGeneratedCsvFile.get(i);
            final Coordinate coordinateInputWgs84 = CoordinateFactory.createFromYLatitudeXLongitude(epsgCrsAndAreaCodeWithCoordinates.centroidY, epsgCrsAndAreaCodeWithCoordinates.centroidX, wgs84);

            final TransformResult resultOutputFromWgs4 = crsTransformationComposite.transform(coordinateInputWgs84, epsgCrsAndAreaCodeWithCoordinates.epsgCrsCode);
            if(!resultOutputFromWgs4.isSuccess()) continue;

            final TransformResult resultWhenTransformedBackToWgs84 = crsTransformationComposite.transform(resultOutputFromWgs4.getOutputCoordinate(), wgs84);
            if(!resultWhenTransformedBackToWgs84.isSuccess()) continue;

            final ResultsStatistic resultsStatistic = resultWhenTransformedBackToWgs84.getResultsStatistic();
            assertNotNull(resultsStatistic);
            assertTrue(resultsStatistic.isStatisticsAvailable());
            if(
                resultsStatistic.getMaxDiffXLongitude() > deltaDiff
                ||
                resultsStatistic.getMaxDiffYLatitude() > deltaDiff
            ) {
                transformResultsWithLargeDifferences.add(resultWhenTransformedBackToWgs84);
            }
        }
        System.setOut(outStream);
        System.setErr(errStream);
        System.out.println("Number of iterated rows/coordinates: " + coordinatesFromGeneratedCsvFile.size());

        System.out.println("Number of results with 'large' differences: " + transformResultsWithLargeDifferences.size());
        for (int i = 0; i <transformResultsWithLargeDifferences.size() ; i++) {
            final TransformResult transformResult = transformResultsWithLargeDifferences.get(i);
            System.out.println("----------------------------------------");
            System.out.println("epsg " + transformResult.getInputCoordinate().getCrsIdentifier().getCrsCode());
            System.out.println("MaxDiffYLatitude : " + transformResult.getResultsStatistic().getMaxDiffYLatitude());
            System.out.println("MaxDiffYLongitude: " + transformResult.getResultsStatistic().getMaxDiffXLongitude());
            final List<TransformResult> subResults = transformResult.getSubResults();
            for (int j = 0; j <subResults.size() ; j++) {
                final TransformResult subTransformResult = subResults.get(j);
                if(subTransformResult.isSuccess()) {
                    System.out.println( subTransformResult.getOutputCoordinate() + " , " + subTransformResult.getCrsTransformationAdapterThatCreatedTheResult().getNameOfImplementation());
                }
            }
        }
    }

    private void verifyFiveImplementations(
        final CrsTransformationAdapterComposite crsTransformationAdapterComposite
    ) {
        final Coordinate input = CoordinateFactory.latLon(59, 18);
        final TransformResult result = crsTransformationAdapterComposite.transform(input, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertNotNull(result);
        assertTrue(result.isSuccess());
        assertEquals(5, result.getSubResults().size());
        final ResultsStatistic resultsStatistic = result.getResultsStatistic();
        assertNotNull(resultsStatistic);
        assertTrue(resultsStatistic.isStatisticsAvailable());
        assertEquals(5, resultsStatistic.getNumberOfResults());
        assertEquals(1.4857726637274027E-4, resultsStatistic.getMaxDiffXLongitude());
    }

    private PrintStream getNullStreamToAvoidOutputFromSystemOutAndSystemErr() {
        final PrintStream nullStream = new PrintStream(new OutputStream() {
            @Override
            public void write(int b) throws IOException {
                // do NOTHING here !
            }
        });
        return nullStream;
    }
}

/*
the test method 'findPotentialBuggyImplementations' took about 725 seconds
iterating all 6435 rows (using the delta value 'double deltaDiff = 0.01'
finding 80 results with big diffs and they are pasted below)

----------------------------------------
epsg EPSG:2155
MaxDiffYLatitude : 1.939436833402885
MaxDiffYLongitude: 0.07540464603476948
Coordinate(xLongitude=-169.15998941415342, yLatitude=-14.824547002341273, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-169.23539406018818, yLatitude=-12.885110168938388, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-169.23539404167536, yLatitude=-12.88511048293167, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-169.15998941415342, yLatitude=-14.824547002341273, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:2156
MaxDiffYLatitude : 5.576044976862704E-6
MaxDiffYLongitude: 360.00003095186287
Coordinate(xLongitude=-169.05835371559382, yLatitude=-13.851858975415933, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=190.94167723626902, yLatitude=-13.851853399370956, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-169.05835371559382, yLatitude=-13.851858975415933, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3078
MaxDiffYLatitude : 43.095759489189916
MaxDiffYLongitude: 31.443650606727118
Coordinate(xLongitude=-76.11215625078763, yLatitude=22.364236723242144, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-107.55580685751475, yLatitude=65.45999621243206, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:3079
MaxDiffYLatitude : 43.095759473545165
MaxDiffYLongitude: 31.443650624605752
Coordinate(xLongitude=-76.11215624864411, yLatitude=22.364236758706063, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-107.55580687324986, yLatitude=65.45999623225123, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:3167
MaxDiffYLatitude : 4.007163363674097
MaxDiffYLongitude: 3.8930455586025374
Coordinate(xLongitude=100.24327657883607, yLatitude=2.0072840630355526, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=104.13632213743861, yLatitude=6.01444742670965, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:3168
MaxDiffYLatitude : 3.996121809278274
MaxDiffYLongitude: 2.995971422355723
Coordinate(xLongitude=103.6791109022657, yLatitude=2.013683499660611, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=100.68313947990998, yLatitude=6.009805308938885, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:3200
MaxDiffYLatitude : 0.0143552702031009
MaxDiffYLongitude: 0.016256606488163072
Coordinate(xLongitude=50.06629636679306, yLatitude=30.77311055601624, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=50.082552973281224, yLatitude=30.787439319246044, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=50.0824906063301, yLatitude=30.78746582621934, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=50.06629636679306, yLatitude=30.77311055601624, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3388
MaxDiffYLatitude : 10.197051653530686
MaxDiffYLongitude: 0.1070230584296894
Coordinate(xLongitude=50.68819927427791, yLatitude=37.56559367350033, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=50.58120280037262, yLatitude=47.76264532703102, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=50.581176215848224, yLatitude=47.76262238035653, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=50.68819927427791, yLatitude=37.56559367350033, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3411
MaxDiffYLatitude : 138.42154464056222
MaxDiffYLongitude: 0.0
Coordinate(xLongitude=-1.90833280887811E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-1.90833280887811E-14, yLatitude=-63.42154464056222, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-1.90833280887811E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3413
MaxDiffYLatitude : 138.42153622742262
MaxDiffYLongitude: 0.0
Coordinate(xLongitude=-1.90833280887811E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-1.90833280887811E-14, yLatitude=74.99999999999767, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-1.90833280887811E-14, yLatitude=-63.42153622742265, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-1.90833280887811E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3468
MaxDiffYLatitude : 40.54868034255838
MaxDiffYLongitude: 88.44923218430515
Coordinate(xLongitude=-113.09973370471785, yLatitude=29.817757745558175, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-24.650501520412703, yLatitude=70.36643808811655, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:3571
MaxDiffYLatitude : 0.10238365301074737
MaxDiffYLongitude: 180.0
Coordinate(xLongitude=5.088887490341627E-14, yLatitude=67.4999999945899, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=5.088887490341627E-14, yLatitude=67.4999999945899, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=180.00000000000006, yLatitude=67.39761634157915, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=5.088887490341627E-14, yLatitude=67.4999999945899, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3572
MaxDiffYLatitude : 0.05147483442274847
MaxDiffYLongitude: 179.77835564394826
Coordinate(xLongitude=-5.088887490341627E-14, yLatitude=67.49999999458987, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-5.088887490341627E-14, yLatitude=67.49999999458986, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-179.77835564394832, yLatitude=67.44852516016712, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-5.088887490341627E-14, yLatitude=67.49999999458987, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3573
MaxDiffYLatitude : 0.09572113042673891
MaxDiffYLongitude: 179.91189886553846
Coordinate(xLongitude=2.5444437451708134E-14, yLatitude=67.49999999458981, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=2.5444437451708134E-14, yLatitude=67.49999999458981, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-179.91189886553843, yLatitude=67.59572112501655, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=2.5444437451708134E-14, yLatitude=67.49999999458981, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3574
MaxDiffYLatitude : 0.018187865480726373
MaxDiffYLongitude: 0.25241225001491036
Coordinate(xLongitude=1.5902773407317584E-13, yLatitude=67.49999999458986, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=1.5902773407317584E-13, yLatitude=67.49999999458986, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-0.2524122500147513, yLatitude=67.48181212910913, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=1.5902773407317584E-13, yLatitude=67.49999999458986, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3575
MaxDiffYLatitude : 0.09624903987401012
MaxDiffYLongitude: 0.0873629752016015
Coordinate(xLongitude=1.1131941385122309E-14, yLatitude=67.4999999945899, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=1.1131941385122309E-14, yLatitude=67.4999999945899, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=0.08736297520161262, yLatitude=67.40375095471589, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=1.1131941385122309E-14, yLatitude=67.4999999945899, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3576
MaxDiffYLatitude : 0.10190696828048829
MaxDiffYLongitude: 1.2722218725854067E-14
Coordinate(xLongitude=5.088887490341627E-14, yLatitude=67.49999999458981, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=5.088887490341627E-14, yLatitude=67.49999999458981, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=6.361109362927034E-14, yLatitude=67.6019069628703, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=5.088887490341627E-14, yLatitude=67.49999999458981, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3591
MaxDiffYLatitude : 43.095759473545165
MaxDiffYLongitude: 31.443650624605752
Coordinate(xLongitude=-76.11215624864411, yLatitude=22.364236758706063, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-107.55580687324986, yLatitude=65.45999623225123, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:3752
MaxDiffYLatitude : 9.711441390714235
MaxDiffYLongitude: 360.0
Coordinate(xLongitude=163.63408157315573, yLatitude=-38.147406391942646, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-175.8055747043628, yLatitude=-47.85884778262957, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=184.1944252956372, yLatitude=-47.85884778265688, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=163.63408157315573, yLatitude=-38.147406391942646, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3832
MaxDiffYLatitude : 2.042810365310288E-13
MaxDiffYLongitude: 360.0
Coordinate(xLongitude=-160.13355215545675, yLatitude=-2.451032079281607, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-160.13355215545675, yLatitude=-2.451032079281607, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=199.86644784454325, yLatitude=-2.4510320792818114, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-160.13355215545675, yLatitude=-2.451032079281607, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3994
MaxDiffYLatitude : 9.711441390714235
MaxDiffYLongitude: 360.0
Coordinate(xLongitude=163.63408157315573, yLatitude=-38.147406391942646, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-175.8055747043628, yLatitude=-47.85884778262957, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=184.1944252956372, yLatitude=-47.85884778265688, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=163.63408157315573, yLatitude=-38.147406391942646, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3995
MaxDiffYLatitude : 140.9815537001683
MaxDiffYLongitude: 0.0
Coordinate(xLongitude=-2.1221543041368323E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-2.1221543041368323E-14, yLatitude=74.99999999999767, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-2.1221543041368323E-14, yLatitude=-65.9815537001683, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-2.1221543041368323E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:3996
MaxDiffYLatitude : 149.99999999999773
MaxDiffYLongitude: 0.0
Coordinate(xLongitude=-2.122154304136832E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-2.122154304136832E-14, yLatitude=74.99999999999767, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-2.122154304136832E-14, yLatitude=-74.99999999999773, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-2.122154304136832E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:4272
MaxDiffYLatitude : 13.355854283947444
MaxDiffYLongitude: 134.86257676230002
Coordinate(xLongitude=-67.6598522288655, yLatitude=-16.2331592197092, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=67.20272453343452, yLatitude=-2.877304935803745, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=67.20272453343452, yLatitude=-2.8773049357617566, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-67.6598522288655, yLatitude=-16.2331592197092, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:4277
MaxDiffYLatitude : 6.8579446005568245
MaxDiffYLongitude: 139.19229245103602
Coordinate(xLongitude=-74.47736854395522, yLatitude=2.1957543103089314, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=64.7149239070808, yLatitude=-4.662190290247893, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=64.7149239070808, yLatitude=-4.662190290070227, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-74.47736854395522, yLatitude=2.1957543103089314, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:4299
MaxDiffYLatitude : 36.02794574712324
MaxDiffYLongitude: 17.172391422576993
Coordinate(xLongitude=-50.83027307393541, yLatitude=68.20880375539434, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-33.657881651358416, yLatitude=32.1808580082711, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-50.83027307393541, yLatitude=68.20880375539434, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:4807
MaxDiffYLatitude : 4.9139125498208855
MaxDiffYLongitude: 0.016977134839402996
Coordinate(xLongitude=2.489921840596637, yLatitude=44.22573778124648, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=2.50689897543604, yLatitude=49.139650331067365, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:4810
MaxDiffYLatitude : 2.0396416765909393
MaxDiffYLongitude: 4.6854327468877415
Coordinate(xLongitude=44.50363161899745, yLatitude=-18.357536104324762, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=49.189064365885194, yLatitude=-20.3971777809157, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:4811
MaxDiffYLatitude : 3.6245945578123226
MaxDiffYLongitude: 0.12508090107236658
Coordinate(xLongitude=3.4613830115353785, yLatitude=32.6222030864222, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=3.586463912607745, yLatitude=36.246797644234526, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:4816
MaxDiffYLatitude : 3.605942105524143
MaxDiffYLongitude: 0.7668426963446819
Coordinate(xLongitude=9.238779210129863, yLatitude=32.456663018316235, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=10.005621906474545, yLatitude=36.06260512384038, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:5482
MaxDiffYLatitude : 37.04457539630556
MaxDiffYLongitude: 38.85061204809645
Coordinate(xLongitude=180.0, yLatitude=-82.99999999999996, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=141.14938795190355, yLatitude=-45.9554246036944, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=180.0, yLatitude=-82.99999999999996, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:5530
MaxDiffYLatitude : 0.07444466827822893
MaxDiffYLongitude: 2.724247022882764E-4
Coordinate(xLongitude=-48.5120524419927, yLatitude=-11.251131758677346, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-48.51178001729041, yLatitude=-11.325576426955575, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-48.5120524419927, yLatitude=-11.251131758677346, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:24370
MaxDiffYLatitude : 0.005756825583532077
MaxDiffYLongitude: 0.015600174315210324
Coordinate(xLongitude=73.90298943964305, yLatitude=36.191037725529974, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=73.91858961395826, yLatitude=36.196794551113506, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=73.90298943964305, yLatitude=36.191037725529974, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:24371
MaxDiffYLatitude : 0.016583000683752402
MaxDiffYLongitude: 0.013727218787195739
Coordinate(xLongitude=73.0626259995873, yLatitude=31.042314696265816, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=73.07635321837449, yLatitude=31.05889769694957, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=73.0626259995873, yLatitude=31.042314696265816, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:24374
MaxDiffYLatitude : 0.01485224826905629
MaxDiffYLongitude: 0.0031304480879725816
Coordinate(xLongitude=77.5411597555667, yLatitude=12.258671303137268, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=77.54429020365467, yLatitude=12.273523551406324, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.5411597555667, yLatitude=12.258671303137268, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:24377
MaxDiffYLatitude : 0.06071884560195784
MaxDiffYLongitude: 0.003132933362053336
Coordinate(xLongitude=66.70987860591494, yLatitude=26.191994440096277, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=66.71301153927699, yLatitude=26.252713285698235, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=66.71296388191803, yLatitude=26.252693727534663, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=66.70987860591494, yLatitude=26.191994440096277, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:24378
MaxDiffYLatitude : 0.048917309796919994
MaxDiffYLongitude: 0.026229400566776917
Coordinate(xLongitude=76.76131117722699, yLatitude=30.942643871834363, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=76.78754057779376, yLatitude=30.991561181631283, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=76.78754057779376, yLatitude=30.99156115288286, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=76.76131117722699, yLatitude=30.942643871834363, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:24383
MaxDiffYLatitude : 0.014178000345538067
MaxDiffYLongitude: 0.0030272198336405154
Coordinate(xLongitude=77.53968795961507, yLatitude=12.251569072532321, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=77.5427151794487, yLatitude=12.26574707287786, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=77.5427151794487, yLatitude=12.265747069896157, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.53968795961507, yLatitude=12.251569072532321, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:24571
MaxDiffYLatitude : 3.9953759044324113
MaxDiffYLongitude: 2.9972959175522647
Coordinate(xLongitude=103.67987018146215, yLatitude=2.0139238444072203, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=100.68257426390988, yLatitude=6.009299748839632, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:24600
MaxDiffYLatitude : 0.0044366878686652456
MaxDiffYLongitude: 0.014922552211629636
Coordinate(xLongitude=47.63664659021619, yLatitude=29.355814103474984, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=47.65156914242782, yLatitude=29.351377415745233, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=47.65156912033472, yLatitude=29.35137741560632, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=47.63664659021619, yLatitude=29.355814103474984, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:26194
MaxDiffYLatitude : 0.050335798023521505
MaxDiffYLongitude: 3.8203019743399125E-4
Coordinate(xLongitude=-12.05590914912854, yLatitude=26.15285822087084, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-12.055527118931106, yLatitude=26.20319401889436, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-12.055527118931106, yLatitude=26.203193998125265, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-12.05590914912854, yLatitude=26.15285822087084, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:26195
MaxDiffYLatitude : 0.11987006928532651
MaxDiffYLongitude: 0.005805350119516106
Coordinate(xLongitude=-14.55633066041704, yLatitude=22.735600055347472, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-14.550525310297523, yLatitude=22.8554701246328, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-14.550525310297527, yLatitude=22.855470109326568, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-14.55633066041704, yLatitude=22.735600055347472, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:26591
MaxDiffYLatitude : 6.953462753500617E-5
MaxDiffYLongitude: 12.452839113281449
Coordinate(xLongitude=15.772364433524459, yLatitude=42.21617701139325, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=3.3195253202430104, yLatitude=42.216107476765714, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:26592
MaxDiffYLatitude : 2.7333960943565216E-6
MaxDiffYLongitude: 12.452880405959757
Coordinate(xLongitude=21.177282354914787, yLatitude=40.01267750511369, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=8.72440194895503, yLatitude=40.01268023850979, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:26731
MaxDiffYLatitude : 40.55041703386944
MaxDiffYLongitude: 88.45366504465784
Coordinate(xLongitude=-113.0992590439273, yLatitude=29.817074183834034, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-24.64559399926946, yLatitude=70.36749121770347, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:26931
MaxDiffYLatitude : 40.54868037793247
MaxDiffYLongitude: 88.44923223122301
Coordinate(xLongitude=-113.09973370949383, yLatitude=29.817757703526464, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-24.650501478270822, yLatitude=70.36643808145894, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
----------------------------------------
epsg EPSG:27205
MaxDiffYLatitude : 53.183812261730566
MaxDiffYLongitude: 68.36208380571226
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=146.35036391908892, yLatitude=-11.180289518161896, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27206
MaxDiffYLatitude : 45.140931653244664
MaxDiffYLongitude: 51.35934080996789
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=129.34762092334455, yLatitude=-19.223170126647805, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27207
MaxDiffYLatitude : 56.22185627359788
MaxDiffYLongitude: 61.82108707080694
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=139.8093671841836, yLatitude=-8.142245506294588, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27208
MaxDiffYLatitude : 37.75404701287442
MaxDiffYLongitude: 25.38791235169677
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=103.37619246507343, yLatitude=-26.610054767018042, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27209
MaxDiffYLatitude : 35.62673238579883
MaxDiffYLongitude: 15.221315377618552
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=93.20959549099521, yLatitude=-28.737369394093633, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27210
MaxDiffYLatitude : 35.03148154229015
MaxDiffYLongitude: 22.509941320759424
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=100.49822143413608, yLatitude=-29.332620237602317, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27211
MaxDiffYLatitude : 38.77835020902047
MaxDiffYLongitude: 173.6008859708362
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-95.61260585745956, yLatitude=-25.58575157087199, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27212
MaxDiffYLatitude : 46.36770580504318
MaxDiffYLongitude: 180.824442713527
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-102.83616260015035, yLatitude=-17.996395974849285, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27213
MaxDiffYLatitude : 51.21472898042392
MaxDiffYLongitude: 185.72084496645522
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-107.73256485307854, yLatitude=-13.149372799468546, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27214
MaxDiffYLatitude : 56.10526872759252
MaxDiffYLongitude: 192.0159080374673
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-114.02762792409065, yLatitude=-8.258833052299945, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27215
MaxDiffYLatitude : 60.284327264967416
MaxDiffYLongitude: 191.24945666336143
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-113.26117654998477, yLatitude=-4.079774514925046, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27216
MaxDiffYLatitude : 61.068659622831674
MaxDiffYLongitude: 192.25521206337035
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-114.26693194999369, yLatitude=-3.29544215706079, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27217
MaxDiffYLatitude : 67.39865603370161
MaxDiffYLongitude: 190.7826456267183
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-112.79436551334166, yLatitude=3.0345542538091537, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27218
MaxDiffYLatitude : 68.47780705720409
MaxDiffYLongitude: 189.0503237863618
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-111.06204367298514, yLatitude=4.113705277311621, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27219
MaxDiffYLatitude : 69.81860660572124
MaxDiffYLongitude: 187.332966402912
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-109.34468628953535, yLatitude=5.4545048258287725, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27220
MaxDiffYLatitude : 61.346213147771934
MaxDiffYLongitude: 189.8008764439931
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-111.81259633061643, yLatitude=-3.017888632120529, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27221
MaxDiffYLatitude : 73.55141706014024
MaxDiffYLongitude: 184.60674004837477
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-106.61845993499813, yLatitude=9.187315280247772, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27222
MaxDiffYLatitude : 75.64513881615399
MaxDiffYLongitude: 179.35333867600843
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-101.36505856263179, yLatitude=11.281037036261527, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27223
MaxDiffYLatitude : 73.94913096228444
MaxDiffYLongitude: 4.552487656119581
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=82.54076776949624, yLatitude=9.58502918239197, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27224
MaxDiffYLatitude : 73.85024582612073
MaxDiffYLongitude: 182.65718048009353
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-104.66890036671687, yLatitude=9.48614404622826, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27225
MaxDiffYLatitude : 74.30860538725128
MaxDiffYLongitude: 179.68307988318844
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-101.6947997698118, yLatitude=9.944503607358806, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27226
MaxDiffYLatitude : 74.86991357902605
MaxDiffYLongitude: 4.385058808054438
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=82.3733389214311, yLatitude=10.505811799133578, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27227
MaxDiffYLatitude : 72.8395569661419
MaxDiffYLongitude: 7.125394609942504
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=85.11367472331916, yLatitude=8.47545518624943, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27230
MaxDiffYLatitude : 71.71927908906589
MaxDiffYLongitude: 9.089050152079608
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=87.07733026545627, yLatitude=7.355177309173423, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27259
MaxDiffYLatitude : 53.35662121206029
MaxDiffYLongitude: 193.3303807025074
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-115.34210058913074, yLatitude=-11.007480567832175, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27260
MaxDiffYLatitude : 30.726384256230006
MaxDiffYLongitude: 22.376309153658497
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=100.36458926703516, yLatitude=-33.63771752366246, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27291
MaxDiffYLatitude : 39.518600131339284
MaxDiffYLongitude: 41.390069588162476
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=119.37834970153914, yLatitude=-24.84550164855318, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27292
MaxDiffYLatitude : 76.17573201338317
MaxDiffYLongitude: 4.554467194281145
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=82.5427473076578, yLatitude=11.811630233490696, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=77.98828011337666, yLatitude=-64.36410177989247, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:27700
MaxDiffYLatitude : 94.27858282920299
MaxDiffYLongitude: 154.96577091898743
Coordinate(xLongitude=-2.577909954367436, yLatitude=55.727252682738985, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-2.5779099543216417, yLatitude=55.727252710270356, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-2.577909954321746, yLatitude=55.72725267646059, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-157.54368087330909, yLatitude=-38.55133011893263, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:29100
MaxDiffYLatitude : 0.07378932427206841
MaxDiffYLongitude: 7.062511108273384E-4
Coordinate(xLongitude=-48.51240441126336, yLatitude=-11.214353410774699, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-48.511698160152534, yLatitude=-11.288142735046767, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-48.51169816015255, yLatitude=-11.288142732687126, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-48.51240441126336, yLatitude=-11.214353410774699, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:29101
MaxDiffYLatitude : 0.07387257870332498
MaxDiffYLongitude: 7.062443263947671E-4
Coordinate(xLongitude=-48.5124044078928, yLatitude=-11.214353758397575, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-48.51178469756933, yLatitude=-11.2882263371009, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-48.51169816356641, yLatitude=-11.288142378308455, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-48.5124044078928, yLatitude=-11.214353758397575, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:31300
MaxDiffYLatitude : 4.810337017602251E-7
MaxDiffYLongitude: 0.010547248707214685
Coordinate(xLongitude=4.643212079817477, yLatitude=50.650587257033415, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=4.653759326333845, yLatitude=50.65058723181032, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=4.643212077626631, yLatitude=50.65058677599971, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=4.643212079817477, yLatitude=50.650587257033415, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:32661
MaxDiffYLatitude : 2.3163693185779266E-12
MaxDiffYLongitude: 89.99999999999994
Coordinate(xLongitude=-2.3899337158287743E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=-89.99999999999997, yLatitude=74.99999999999767, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=-2.3899337158287743E-14, yLatitude=74.99999999999767, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=-2.3899337158287743E-14, yLatitude=74.99999999999999, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J
----------------------------------------
epsg EPSG:32761
MaxDiffYLatitude : 2.3163693185779266E-12
MaxDiffYLongitude: 89.99999999999994
Coordinate(xLongitude=3.186578287771716E-14, yLatitude=-75.00000000000007, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA
Coordinate(xLongitude=89.99999999999997, yLatitude=-74.99999999999777, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools
Coordinate(xLongitude=3.186578287771716E-14, yLatitude=-74.99999999999775, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
Coordinate(xLongitude=3.186578287771716E-14, yLatitude=-75.00000000000007, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J

 */