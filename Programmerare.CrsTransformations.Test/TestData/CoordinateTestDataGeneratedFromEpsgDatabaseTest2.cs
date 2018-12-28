using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using Programmerare.CrsTransformations.CompositeTransformations;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Programmerare.CrsTransformations.TestData
{

[Ignore("Not real tests but a method in this class can be executed if you temporary disable this 'Ignore'")]
// You may want to temporary change the above line iftestf you want to run the "tests"
[TestFixture] 
// a better class name would be desirable...
public class CoordinateTestDataGeneratedFromEpsgDatabaseTest2 : CrsTransformationTestBase {

    private const double deltaDiff = 0.0001; // fairly large value to only find the large differences (potential large problems/bugs)
    // when the delta value above is 0.01 then about 80 EPSG codes are found i.e. there
    // are than many transformation (from and back that EPSG code) which will create
    // a bigger difference than 0.01 in either the latitude or longitude (or both)
    // between at least two of the implementations.

    // Note that there are currently two methods 'findPotentialBuggyImplementations'
    // (one in this class and one in the class 'CoordinateTestDataGeneratedFromEpsgDatabaseTest')
    // Which are essentially trying to do the same thing i.e. transform back and forth
    // with lots of coordinates from a csv file.
    // However, the first will be based on having down those transformations and generated
    // output to different files, and then comparing those files.
    // This method below will instead use the normal composite adapter to do the transformations
    // with all leaf implementations and use methods of the result statistics object
    // to find problematic transformations with large differences.

    [Test] // currently not a real test with assertions but printing console output with differences
    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    //Java: @Tag(TestCategory.SlowTest) // about 725 seconds iterating all 6435 rows (using the delta value 'double deltaDiff = 0.01' finding 80 results with big diffs)
    //[Category(TestCategory.SlowTest)] // but no actually not very slow in the .NET port
    //[Ignore(TestCategory.SlowTest)] 
    public void FindPotentialBuggyImplementations() {
        FindPotentialBuggyImplementationsHelper(
            0, 
            int.MaxValue,
            0.01
        );
    }

    [Test]
    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    public void FindPotentialBuggyImplementationsSwedishCrs(
    ) {
        // EPSG codes for RT90 and SWEREF99
        // are in the intervall 3006 - 3024
        FindPotentialBuggyImplementationsHelper(
            3006, 
            3024,
            0.001
        );
    }
    public void FindPotentialBuggyImplementationsHelper(
        int minEpsgCrsCode,
        int maxEpsgCrsCode,
        double? optionalDelta = null
    ) {
        int numberIfEpsgCodesToConsiderInIteration = maxEpsgCrsCode - minEpsgCrsCode;
        bool manyWillBeIterated = numberIfEpsgCodesToConsiderInIteration > 100;
        double deltaDiffToUse = manyWillBeIterated ? deltaDiff : double.MinValue;
        if(optionalDelta.HasValue) {
            deltaDiffToUse = optionalDelta.Value;
        }

        var crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();
        CrsTransformationAdapterComposite crsTransformationComposite = crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian();
        verifyThreeImplementations(crsTransformationComposite); // to make sure that the above factory really creates an object which will use three implementations

        IList<CrsTransformationResult> transformResultsWithLargeDifferences = new List<CrsTransformationResult>();

        CrsIdentifier wgs84 = CrsIdentifierFactory.CreateFromEpsgNumber(EpsgNumber.WORLD__WGS_84__4326);
        
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        IList<EpsgCrsAndAreaCodeWithCoordinates> coordinatesFromGeneratedCsvFile = CoordinateTestDataGeneratedFromEpsgDatabaseTest.GetCoordinatesFromGeneratedCsvFile();
        int seconds = (int)stopWatch.Elapsed.TotalSeconds;
        WriteLine("Time for reading the content of the input file: " + seconds);
        stopWatch.Restart();    
        int totalNumberOfSeconds;

        WriteLine("number of rows to iterate: " + coordinatesFromGeneratedCsvFile.Count);
        for (int i = 0; i <coordinatesFromGeneratedCsvFile.Count ; i++) {
                if (manyWillBeIterated && (i % 100 == 0)) {
                    //WriteLine("number of rows iterated so far: " + i);
                    totalNumberOfSeconds = (int)stopWatch.Elapsed.TotalSeconds;
                    //WriteLine("Number of seconds so far: " + totalNumberOfSeconds);
                    // if (i > 50) break;
                }
                EpsgCrsAndAreaCodeWithCoordinates epsgCrsAndAreaCodeWithCoordinates = coordinatesFromGeneratedCsvFile[i];
            if(
                epsgCrsAndAreaCodeWithCoordinates.epsgCrsCode < minEpsgCrsCode
                    ||
                epsgCrsAndAreaCodeWithCoordinates.epsgCrsCode > maxEpsgCrsCode
            ) continue;
            if(!manyWillBeIterated) {
                //Console.WriteLine("iterated epsgCrsCode: " + epsgCrsAndAreaCodeWithCoordinates.epsgCrsCode);
            }
            

            CrsCoordinate coordinateInputWgs84 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(epsgCrsAndAreaCodeWithCoordinates.centroidY, epsgCrsAndAreaCodeWithCoordinates.centroidX, wgs84);

            CrsTransformationResult resultOutputFromWgs4 = crsTransformationComposite.Transform(coordinateInputWgs84, epsgCrsAndAreaCodeWithCoordinates.epsgCrsCode);
            if(!resultOutputFromWgs4.IsSuccess) continue;

            CrsTransformationResult resultWhenTransformedBackToWgs84 = crsTransformationComposite.Transform(resultOutputFromWgs4.OutputCoordinate, wgs84);
            if(!resultWhenTransformedBackToWgs84.IsSuccess) continue;

            CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformedBackToWgs84.CrsTransformationResultStatistic;
            Assert.IsNotNull(crsTransformationResultStatistic);
            Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
            if(
                crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude > deltaDiffToUse
                ||
                crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude > deltaDiffToUse
            ) {
                transformResultsWithLargeDifferences.Add(resultWhenTransformedBackToWgs84);
            }
            else{
                if(!manyWillBeIterated) {
                    //Console.WriteLine("NOT 'big' difference for EPSG " + epsgCrsAndAreaCodeWithCoordinates.epsgCrsCode);
                    int count = crsTransformationComposite.TransformationAdapterChildren.Count;
                    //Console.WriteLine("Number of implementations not having big difference: " + count);
                }
            }
        }
        WriteLine("Number of iterated rows/coordinates: " + coordinatesFromGeneratedCsvFile.Count);

        WriteLine("Number of results with 'large' differences: " + transformResultsWithLargeDifferences.Count);
        for (int i = 0; i <transformResultsWithLargeDifferences.Count ; i++) {
            CrsTransformationResult transformResult = transformResultsWithLargeDifferences[i];
            WriteLine("----------------------------------------");
            WriteLine("epsg " + transformResult.InputCoordinate.CrsIdentifier.CrsCode);
            WriteLine("MaxDiffYLatitude : " + transformResult.CrsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude);
            WriteLine("MaxDiffYLongitude: " + transformResult.CrsTransformationResultStatistic.MaxDifferenceForXEastingLongitude);
            IList<CrsTransformationResult> subResults = transformResult.GetTransformationResultChildren();
            for (int j = 0; j <subResults.Count ; j++) {
                CrsTransformationResult subTransformResult = subResults[j];
                if(subTransformResult.IsSuccess) {
                    WriteLine( subTransformResult.OutputCoordinate + " , " + subTransformResult.CrsTransformationAdapterResultSource.AdapteeType);
                }
            }
        }
    }

    private void verifyThreeImplementations(
        CrsTransformationAdapterComposite crsTransformationAdapterComposite
    ) {
        CrsCoordinate input = CrsCoordinateFactory.LatLon(59.0, 18.0);
        CrsTransformationResult result = crsTransformationAdapterComposite.Transform(input, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        const int numberOfImplementations = CrsTransformationAdapterTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;
        Assert.AreEqual(numberOfImplementations, result.GetTransformationResultChildren().Count);
        CrsTransformationResultStatistic crsTransformationResultStatistic = result.CrsTransformationResultStatistic;
        Assert.IsNotNull(crsTransformationResultStatistic);
        Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
        Assert.AreEqual(numberOfImplementations, crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults);
        Assert.That(crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude, Is.LessThan(0.001));
    }
    //---------------------------------------------------------------
    // Resulting output from the below method:
    //---------------------
    //Result for adapter: LEAF_DOT_SPATIAL_2_0_0_RC1
    //numberOfSuccesses: 5078
    //numberOfFailures: 1357
    //Number of seconds: 1
    //---------------------
    //Result for adapter: LEAF_PROJ_NET_4_GEO_API_1_4_1
    //numberOfSuccesses: 2494
    //numberOfFailures: 3941
    //Number of seconds: 2
    //---------------------
    //Result for adapter: LEAF_MIGHTY_LITTLE_GEODESY_1_0_1
    //numberOfSuccesses: 19
    //numberOfFailures: 6416
    //Number of seconds: 0
    //---------------------
    [Test] // currently not a real test with assertions but printing console output with differences
    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    public void FindTheNumberOfEpsgCodesPotentiallySupported() {
        IList<EpsgCrsAndAreaCodeWithCoordinates> coordinatesFromGeneratedCsvFile = CoordinateTestDataGeneratedFromEpsgDatabaseTest.GetCoordinatesFromGeneratedCsvFile();

        int totalNumberOfSeconds;

        foreach(ICrsTransformationAdapter leafAdapter in base.crsTransformationAdapterLeafImplementations) {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            int numberOfFailures = 0;
            int numberOfSuccesses = 0;
            for (int i = 0; i <coordinatesFromGeneratedCsvFile.Count ; i++) {
                EpsgCrsAndAreaCodeWithCoordinates epsgCrsAndAreaCodeWithCoordinates = coordinatesFromGeneratedCsvFile[i];
                CrsCoordinate coordinateInputWgs84 = CrsCoordinateFactory.LatLon(
                    epsgCrsAndAreaCodeWithCoordinates.centroidY, 
                    epsgCrsAndAreaCodeWithCoordinates.centroidX
                );
                CrsTransformationResult resultOutputFromWgs4 = leafAdapter.Transform(
                    coordinateInputWgs84, epsgCrsAndAreaCodeWithCoordinates.epsgCrsCode
                );
                if(resultOutputFromWgs4.IsSuccess) {
                    numberOfSuccesses++;
                    //WriteLine("Success " + DateTime.Now + " " + leafAdapter.AdapteeType + " " + resultOutputFromWgs4.OutputCoordinate);
                }
                else
                {
                    //WriteLine("Failure " + DateTime.Now + " " + leafAdapter.AdapteeType);
                    numberOfFailures++;
                }
                if(i % 500 == 0) {
                    //WriteLine("number of rows iterated so far: " + i + " (for leafAdapter " + leafAdapter.AdapteeType + " )");
                    totalNumberOfSeconds = (int)stopWatch.Elapsed.TotalSeconds;
                    //WriteLine("Number of seconds so far (for the above adapter) : " + totalNumberOfSeconds);
                    //if(i > 500) break;
                }
            }
            totalNumberOfSeconds = (int)stopWatch.Elapsed.TotalSeconds;
            WriteLine("---------------------");
            WriteLine("Result for adapter: " + leafAdapter.AdapteeType);
            WriteLine("numberOfSuccesses: " + numberOfSuccesses);
            WriteLine("numberOfFailures: " + numberOfFailures);
            WriteLine("Number of seconds: " + totalNumberOfSeconds);
            WriteLine("---------------------");
        }
    }

    private static void WriteLine(String s) {
        Console.WriteLine(s);
        Debug.WriteLine("Write Debug " + s);
        Trace.WriteLine("Write Trace " + s);
    }
}
}

/*
The test method 'FindPotentialBuggyImplementations' 
(using the "large" delta value 0.01)
found 5 results (when iterating all 6435 rows) 
with such large diffs, and the result 
to the output console is pasted below:


Number of iterated rows/coordinates: 6435
Number of results with 'large' differences: 5
----------------------------------------
epsg EPSG:3058
MaxDiffYLatitude : 0.0321997739620201
MaxDiffYLongitude: 0.0559705530624282
Coordinate(xEastingLongitude=-8.43710569292684, yNorthingLatitude=70.9887727591044, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_DOT_SPATIAL_2_0_0_RC1
Coordinate(xEastingLongitude=-8.49307624598927, yNorthingLatitude=71.0209725330664, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_PROJ_NET_4_GEO_API_1_4_1
----------------------------------------
epsg EPSG:4660
MaxDiffYLatitude : 0.0321997692078071
MaxDiffYLongitude: 0.0559705860460955
Coordinate(xEastingLongitude=-8.43712846504745, yNorthingLatitude=70.9887706478081, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_DOT_SPATIAL_2_0_0_RC1
Coordinate(xEastingLongitude=-8.49309905109354, yNorthingLatitude=71.0209704170159, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_PROJ_NET_4_GEO_API_1_4_1
----------------------------------------
epsg EPSG:26591
MaxDiffYLatitude : 0.00140713323131791
MaxDiffYLongitude: 12.4528560122085
Coordinate(xEastingLongitude=3.31943528605832, yNorthingLatitude=42.2168526459318, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_DOT_SPATIAL_2_0_0_RC1
Coordinate(xEastingLongitude=15.7722912982669, yNorthingLatitude=42.2154455127005, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_PROJ_NET_4_GEO_API_1_4_1
----------------------------------------
epsg EPSG:26592
MaxDiffYLatitude : 0.00141564007302719
MaxDiffYLongitude: 12.4526383015982
Coordinate(xEastingLongitude=8.7244396490393, yNorthingLatitude=40.0133940905228, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_DOT_SPATIAL_2_0_0_RC1
Coordinate(xEastingLongitude=21.1770779506375, yNorthingLatitude=40.0119784504498, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_PROJ_NET_4_GEO_API_1_4_1
----------------------------------------
epsg EPSG:29700
MaxDiffYLatitude : 2.09865842074941
MaxDiffYLongitude: 4.87242430028321
Coordinate(xEastingLongitude=44.2840080421097, yNorthingLatitude=-18.2913284605982, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_DOT_SPATIAL_2_0_0_RC1
Coordinate(xEastingLongitude=49.1564323423929, yNorthingLatitude=-20.3899868813477, crsIdentifier=CrsIdentifier(crsCode='EPSG:4326', isEpsgCode=True, epsgNumber=4326)) , LEAF_PROJ_NET_4_GEO_API_1_4_1
 */