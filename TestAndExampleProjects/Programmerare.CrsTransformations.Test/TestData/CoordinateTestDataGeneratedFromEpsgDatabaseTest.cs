using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_7;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.Coordinate;

namespace Programmerare.CrsTransformations.TestData
{
/**
 * This class can be used for producing "regression files" with lots of transformations
 * for different EPSG codes and saving those results in files.
 * Those files (produced by different adapter implementations) 
 * can then be compared with each other (also with methods in this class below).
 * It will also be possible to compare files produced by different versions 
 * of the same implementations when having done an upgrade.
 * TODO obsolete comment below about 1.4.1 since the new version is now 2.0.0
 * For example, there is currently a produced file "ProjNet_version_1.4.1.csv"
 * and if there will be a version 1.4.2. in the future then another file 
 * "ProjNet_version_1.4.2.csv" can be created and then those two files 
 * can be compared with each other, and then it might be possible to detect 
 * problems for certain EPSG codes when there is a new significant difference.
 * (Of course, a new detected difference can be explained in two ways,
 *  either an improvement/bugfix or the opposite)
 * 
 * The CSV file used as input in this "test" class:
 *  [YOUR_BASE_DIRECTORY_FOR_VS_SOLUTION]\Programmerare.CrsTransformations.Test\resources\generated\CoordinateTestDataGeneratedFromEpsgDatabase.csv
 * The above file has been created with the following class:
 *  [NOT_THIS_DOTNET_SOLUTION_BUT_A_JVM_PROJECT]\crsCodeGeneration\src\main\kotlin\com\programmerare\crsCodeGeneration\coordinateTestDataGenerator\CoordinateTestDataGenerator.kt
 * The relevant columns are the first column (EPSG code) and the last two column with WGS84 coordinates.
 * The WGS84 coordinate defines the "centroid" within an area where some other coordinate
 * system is used (and that other coordinate system is defined byt the EPSG code in the first column)
 * Thus the file defines a list of appropriate WGS84 coordinates which can be transformed back and forth
 * to/from the coordinate system in the first EPSG column.
 */

[TestFixture]
[Category(TestCategory.SideEffectFileCreation)]
[Category(TestCategory.SideEffectPrintingConsoleOutput)]
[Ignore("Not real tests but a method in this class can be executed if you temporary disable this 'Ignore'")]
// You may want to temporary change the above line if you want to run the "tests"
//  ( and also see comments in the class TestCategory regarding that this "test" file 
//    creates files and produces output to the console)
public class CoordinateTestDataGeneratedFromEpsgDatabaseTest {

    private const string NameOfVisualStudioProject = "Programmerare.CrsTransformations.Test";
    private const string OUTPUT_DIRECTORY_FOR_REGRESSION_RESULTS = "resources/regression_results";
    // The above two constants are used for finding a path like this:
    // ...\crsTransformations-dotnet\Programmerare.CrsTransformations.Test\resources\regression_results
    // by in runtime finding a path like this:
    // ...crsTransformations-dotnet\Programmerare.CrsTransformations.Test\bin\Debug\...
    // and then "navigating upwards" until the above project name is found,
    // and then combining it with the above relative path to the regression results
    
    private const string INPUT_TEST_DATA_FILE = "resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv";
    // The content (columns) of the above file are as below:
    // e.g. "3006|1225|Sweden|17.083659606206545|61.98770256318016"
    //  epsgCrsCode | epsgAreaCode | epsgAreaName | centroidX | centroidY
    
    
    private bool shouldCreateNewRegressionFile = true;

    private const double DELTA_LIMIT_FOR_SUCCESS = 0.0001;

    private const string PART_OF_FILENAME_DotSpatial = "DotSpatial";
    private const string PART_OF_FILENAME_ProjNet = "ProjNet";
    private const string PART_OF_FILENAME_MightyLittleGeodesy = "MightyLittleGeodesy";

    private static IList<EpsgCrsAndAreaCodeWithCoordinates> list;

    [SetUp]
    public void Before() {
        list = GetCoordinatesFromGeneratedCsvFile();
    }

    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // Below: "tests" labeled with 'TestCategory.SideEffectFileCreation'

    // To run tests from a certain Category from within Visual Studio,
    // see instructions in the file TestCategory.cs

    [Test] // currently not a real test with assertions but printing console output with differences
    // [Category(TestCategory.SlowTest)]  // actually not at all slow but very fast since very few coordinate systems are supported
    [Category(TestCategory.SideEffectFileCreation)] // directory: .../resources/regression_results/
    public void TestAllTransformationsInGeneratedCsvFileWithMightyLittleGeodesy() {
        //testResults for CrsTransformationAdapterMightyLittleGeodesy
        //seconds: 1
        //countOfSuccess: 19
        //countOfFailures: 6416
        TestResult testResultForMightyLittleGeodesy = RunAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationAdapterMightyLittleGeodesy(), list);
        HandleTestResults(
            testResultForMightyLittleGeodesy,
            DELTA_LIMIT_FOR_SUCCESS,
            shouldCreateNewRegressionFile,
            "_version_1.0.1" // LEAF_MIGHTY_LITTLE_GEODESY_1_0_1
            // File created: "resources/regression_results/MightyLittleGeodesy_version_1.0.1.csv
        );
    }

    [Test] // currently not a real test with assertions but printing console output with differences
    [Category(TestCategory.SideEffectFileCreation)] // directory: .../resources/regression_results/
    //[Category(TestCategory.SlowTest)] // no actually only 4 seconds which is not too bad
    public void TestAllTransformationsInGeneratedCsvFileWithDotSpatial() {
        //testResults for CrsTransformationAdapterDotSpatial
        //seconds: 4
        //countOfSuccess: 5074
        //countOfFailures: 1361
        TestResult testResultForDotSpatial = RunAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationAdapterDotSpatial(), list);
        HandleTestResults(
            testResultForDotSpatial,
            DELTA_LIMIT_FOR_SUCCESS,
            shouldCreateNewRegressionFile,
            "_version_2.0.0_rc1"  // LEAF_DOT_SPATIAL_2_0_0_RC1
            // File created: "resources/regression_results/DotSpatial_version_2.0.0_rc1.csv
        );
    }

    [Test]
    [Category(TestCategory.SideEffectFileCreation)] // directory: .../resources/regression_results/
    //[Category(TestCategory.SlowTest)] // no actually only 5 seconds which is not too bad
    public void TestAllTransformationsInGeneratedCsvFileWithProjNet() {
        //testResults for CrsTransformationAdapterProjNet
        //seconds: 5
        //countOfSuccess: 2481
        //countOfFailures: 3954
        TestResult testResultForProjNet = RunAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationAdapterProjNet(), list);
        HandleTestResults(
            testResultForProjNet,
            DELTA_LIMIT_FOR_SUCCESS,
            shouldCreateNewRegressionFile,
            "_version_1.4.1" // LEAF_PROJ_NET_4_GEO_API_1_4_1  TODO LEAF_PROJ_NET_2_0_0
            // File created: "resources/regression_results/ProjNet_version_1.4.1.csv
        );
    }

    // Above: "tests" labeled with 'TestCategory.SideEffectFileCreation'
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------


    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // Below: "tests" comparing results with different versions of the same implementation

    private static double deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation = 0.000000000001;

    [Test] // currently not a real test with assertions but printing console output with differences
    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    public void CompareResultsForDifferentVersionsOfDotSpatial() {
        // filename e.g. "DotSpatial_version_2.0.0_rc1.csv"
        CompareTheTwoLatestVersion(
            PART_OF_FILENAME_DotSpatial,
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    [Test] // currently not a real test with assertions but printing console output with differences
    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    public void CompareResultsForDifferentVersionsOfProjNet() {
        // filename e.g. "ProjNet_version_1.4.1.csv"
        CompareTheTwoLatestVersion(
            PART_OF_FILENAME_ProjNet,
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    [Test] // currently not a real test with assertions but printing console output with differences
    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    public void CompareResultsForDifferentVersionsOfMightyLittleGeodesy() {
        // filename e.g. "MightyLittleGeodesy_version_1.0.1.csv"
        CompareTheTwoLatestVersion(
            PART_OF_FILENAME_MightyLittleGeodesy,
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    // Above: "tests" comparing results with different versions of the same implementation
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------

    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------

    // Below: "tests" comparing results with different implementations

    private const double deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation = 0.0000001;


    // -------------------------------------------------------------------------------------
    // Comparing the latest results of DotSpatial with the results from the other files
    // (since DotSpatial seem to support the greatest number of EPSG codes, 
    //  based on the file sizes for the regression files)
    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    [Test] // currently not a real test with assertions but printing console output with differences
    public void CompareResultsForLatestDotSpatialAndMightyLittleGeodesyFile () {
        // filenames e.g. "MightyLittleGeodesy_version_1.0.1.csv"
        FileInfo dotSpatialFile = GetLatestDotSpatialFile();
        FileInfo mightyLittleGeodesyFile = GetLatestMightyLittleGeodesyFile();
        CompareWithRegressionFileContent(
            dotSpatialFile,
            mightyLittleGeodesyFile,
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation,
            false // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
        // when above comparing DotSpatial and MightyLittleGeodesy
        // with the following delta values:
        //0.0000001 (no difference detected)
        //0.00000001 (five differences detected, EPSG 3019-3024 i.e. when using CRS RT90)
        //0.000000001 (differences detected for ALMOST all transformatsions RT90 and SWEREF99 , EPSG 3006-3024, except EPSG 3012)
        //0.0000000001 (differences detected for ALL above including EPSG 3012)
        // Difference when transforming with EPSG 3012:
        //  13.7217045972052|62.9689174122742
        //  versus:
        //  13.7217045977201|62.9689174125193
        //   0.0000000001     0.0000000001
    }

    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    [Test] // currently not a real test with assertions but printing console output with differences
    public void CompareResultsForLatestDotSpatialAndProjNetFile() {
        FileInfo dotSpatialFile = GetLatestDotSpatialFile();
        FileInfo ProjNetFile = GetLatestProjNetFile();
        CompareWithRegressionFileContent(
            dotSpatialFile,
            ProjNetFile,
            0.01,
            false // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
        // when above comparing DotSpatial and ProjNetFile
        // with the following delta values:
        // 0.01 (no difference detected larger than this delta)
        // 0.001 (eight differences detected, with these EPSG codes: 4804 4817 25700 27394 27395 27396 27397 27398)
    }

    // Above: "tests" comparing results with different implementations
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    //
    // Below: comparing files for more than two implementations at a time,
    // to find the outliers that are significantly different for the others,
    // i.e. to make it easier to find implementations which are potentially very incorrect (buggy)
    // for transformatsion regarding a certain EPSG code where the significant differences were found

    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    [Test] // currently not a real test with assertions but printing console output with differences
    public void FindPotentialBuggyImplementations() { // similarly named method in another class, see comment there (current class name: CoordinateTestDataGeneratedFromEpsgDatabaseTest2')
        FileInfo dotSpatialFile = GetLatestDotSpatialFile();
        FileInfo ProjNetFile = GetLatestProjNetFile();
        FileInfo mightyLittleGeodesyFile = GetLatestMightyLittleGeodesyFile();
        IList<FileInfo> filesToCompare = new List<FileInfo>{dotSpatialFile, ProjNetFile, mightyLittleGeodesyFile };
        double deltaValueForDifferencesToIgnore = 0.001;
        CompareFiles(
            filesToCompare,
            deltaValueForDifferencesToIgnore
        );
        /// result from the above when using delta 0.001 :
        // Eight differences detected, with these EPSG codes: 4804 4817 25700 27394 27395 27396 27397 27398
    }

    private void CompareFiles(
        IList<FileInfo> filesToCompare,
        double deltaValueForDifferencesToIgnore
    ) {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        ResultAggregator resultAggregator = new ResultAggregator();
        bool shouldShowALLdifferences = deltaValueForDifferencesToIgnore < 0;
        IList<IList<String>> listOfRowsPerFile = new List<IList<string>>();
        int numberOfRowsInFile = -1;
        foreach(FileInfo file in filesToCompare) {
            IList<string> rowsInFile = GetAllLinesFromTextFileUTF8(file);
            listOfRowsPerFile.Add(rowsInFile);
            resultAggregator.AddRowsFromFile(rowsInFile, file);
            if(numberOfRowsInFile < 0) {
                numberOfRowsInFile = rowsInFile.Count;
            }
            else {
                if(rowsInFile.Count != numberOfRowsInFile) {
                    string errorMessage = "Not even the same number of rows in the files";
                    Console.WriteLine(errorMessage);
                    throw new Exception(errorMessage);
                }
            }
        }
        ISet<int> indexesForRowsWithSignificantDifference = resultAggregator.GetIndexesForRowsWithSignificantDifference(deltaValueForDifferencesToIgnore);
        foreach (int ind in indexesForRowsWithSignificantDifference) {
            Console.WriteLine("-----------------");
            Console.WriteLine("index " + ind);
            for (int i=0; i < listOfRowsPerFile.Count; i++) {
                FileInfo file = filesToCompare[i];
                String rowContent = listOfRowsPerFile[i][ind];
                Console.WriteLine(rowContent + " ; " + file.Name);
            }
        }
        long totalNumberOfSecondsForAllTransformations = (long)stopWatch.Elapsed.TotalSeconds;
        Console.WriteLine("Total number of seconds for method compareFiles: " + totalNumberOfSecondsForAllTransformations);
        Console.WriteLine("-------------------------------------------------");
    }
    // -------------------------------------------------------------------------------------


    private TestResult RunAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(
        ICrsTransformationAdapter crsTransformationAdapter,
        IList<EpsgCrsAndAreaCodeWithCoordinates> coordinatesFromGeneratedCsvFile
    ) {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        IList<TestResultItem> testResultItems = new List<TestResultItem>();
        int counter = 0;

        foreach (EpsgCrsAndAreaCodeWithCoordinates item in coordinatesFromGeneratedCsvFile) {
            CrsCoordinate inputCoordinateWGS84 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(item.centroidX, item.centroidY, EpsgNumber.WORLD__WGS_84__4326);
            CrsTransformationResult resultOfTransformationFromWGS84 = crsTransformationAdapter.Transform(inputCoordinateWGS84, item.epsgCrsCode);
            CrsTransformationResult resultOfTransformationBackToWGS84 = null;
            if (resultOfTransformationFromWGS84.IsSuccess) {
                resultOfTransformationBackToWGS84 = crsTransformationAdapter.Transform(resultOfTransformationFromWGS84.OutputCoordinate, EpsgNumber.WORLD__WGS_84__4326);
            }
            testResultItems.Add(new TestResultItem(item, inputCoordinateWGS84, resultOfTransformationFromWGS84, resultOfTransformationBackToWGS84));
            if (counter++ % 500 == 0) // just to show some progress
                Console.WriteLine(this.GetType().Name + " , counter: " + counter + " (of the total " + coordinatesFromGeneratedCsvFile.Count + ") for adapter " + crsTransformationAdapter.GetType().Name); // to show some progress
            // if(counter > 300) break;
        }
        long totalNumberOfSecondsForAllTransformations = (long)stopWatch.Elapsed.TotalSeconds;
        Console.WriteLine("Total number of seconds for method runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile: " + totalNumberOfSecondsForAllTransformations);
        return new TestResult(crsTransformationAdapter, totalNumberOfSecondsForAllTransformations, testResultItems);
    }

    /**
     * Iterates the test results and counts the number of successes and failures
     * including considering a maximum delta difference between the coordinates
     * when having transformed back and forth.
     * While iterating, lines of strings are also created, with the resulting coordinates.
     * Depennding on a bool parameter, those string lines are either written to
     * a file, or compared with a previous created file.
     *
     * @param testResult
     * @param deltaLimitForSuccess
     * @param createNewRegressionFileInfo if false, then instead compare with previous regression file
     */
    private void HandleTestResults(
        TestResult testResult,
        double deltaLimitForSuccess,
        bool shouldCreateNewRegressionFile,
        string fileNameSuffixExcludingExtension
    ) {
        Console.WriteLine("-------------------------------");
        Console.WriteLine("testResults for " + testResult.Adapter.GetType().Name);
        Console.WriteLine("seconds: " + testResult.TotalNumberOfSecondsForAllTransformations);
        IList<TestResultItem> testResultItems = testResult.TestResultItems;
        int countOfFailures = 0;
        int countOfSuccess = 0;
        bool isSuccess;
        IList<string> linesWithCurrentResults = new List<string>();
        foreach (TestResultItem testResultItem in testResultItems) {
            string resultString = testResultItem.GetResultStringForRegressionFile();
            linesWithCurrentResults.Add(resultString);
            isSuccess = testResultItem.IsSuccessfulTransformationFromWGS84();
            if (isSuccess) {
                isSuccess  = testResultItem.IsSuccessfulTransformationBackToWGS84();
                if (isSuccess) {
                    CrsCoordinate inputCoordinateWGS84 = testResultItem.GetInputCoordinateWGS84();
                    CrsCoordinate wgs84Again = testResultItem.GetCoordinateOutputTransformationBackToWGS84();
                    double deltaLong = Math.Abs(inputCoordinateWGS84.XEastingLongitude - wgs84Again.XEastingLongitude);
                    double deltaLat = Math.Abs(inputCoordinateWGS84.YNorthingLatitude - wgs84Again.YNorthingLatitude);
                    isSuccess = deltaLong < deltaLimitForSuccess && deltaLat < deltaLimitForSuccess;
                }
            }
            if (isSuccess) {
                countOfSuccess++;
            } else {
                countOfFailures++;
            }
        }
        Console.WriteLine("countOfSuccess: " + countOfSuccess);
        Console.WriteLine("countOfFailures: " + countOfFailures);
        Console.WriteLine("-------------------------------");

        FileInfo file = GetFileForRegressionResults(testResult.Adapter, fileNameSuffixExcludingExtension);
        if (shouldCreateNewRegressionFile) {
            CreateNewRegressionFile(file, linesWithCurrentResults);
        }
    }

    private FileInfo GetFileForRegressionResults(
        ICrsTransformationAdapter adapter, 
        string fileNameSuffixExcludingExtension
    ) {
        DirectoryInfo directoryForRegressionsResults = GetDirectoryForRegressionsResults();
        FileInfo file = new FileInfo(
            Path.Combine(
                directoryForRegressionsResults.FullName, 
                adapter.ShortNameOfImplementation + fileNameSuffixExcludingExtension + ".csv")
        );
        return file;
    }

    /// <summary>
    /// Should return a directory something like this:
    /// [YOUR_BASE_DIRECTORY_FOR_VS_SOLUTION]\Programmerare.CrsTransformations.Test\resources\regression_results
    /// </summary>
    /// <returns></returns>
    private DirectoryInfo GetDirectoryForRegressionsResults() {
        DirectoryInfo dir = new DirectoryInfo(".");
        // the above directory should now be something like below:
        // ...\Programmerare.CrsTransformations.Test\bin\Debug\netcoreapp2.1
        const int maxParentDirectoriesToNavigateUpwards = 4;
        int directoryNavigationCounter = 0;
        while(directoryNavigationCounter < maxParentDirectoriesToNavigateUpwards) {
            directoryNavigationCounter++;
            dir = dir.Parent;
            if(dir.Name.Equals(NameOfVisualStudioProject)) break;
        }
        if(!dir.Name.Equals(NameOfVisualStudioProject)) {
            throw new Exception("Could not find the project directory with the following name: " + NameOfVisualStudioProject);
        }
        DirectoryInfo directoryForRegressionsResults = new DirectoryInfo(
            Path.Combine(
                dir.FullName, 
                OUTPUT_DIRECTORY_FOR_REGRESSION_RESULTS
            )
        );
        if (!directoryForRegressionsResults.Exists) {
            Console.WriteLine("Directory did not exist: " + directoryForRegressionsResults.FullName);
            throw new Exception("Create the following directory manually if it seems to be a proper directory path: " + directoryForRegressionsResults.FullName);
        }
        return directoryForRegressionsResults;
    }

    private void CreateNewRegressionFile(
        FileInfo file, 
        IList<string> linesWithCurrentResults
    ) {
        //Console.WriteLine("createNewRegressionFile " + file.FullName);
        File.WriteAllLines(file.FullName, linesWithCurrentResults, Encoding.UTF8);
    }

    [Category(TestCategory.SideEffectPrintingConsoleOutput)]
    [Test]
    public void ShowDifferenceIfSignificantTest() {
        DifferenceWhenComparingCoordinateValues differenceWhenComparingCoordinateValues = ShowDifferenceIfSignificant(
            "35.00827072383671|31.517029225386523|2039|200816.30213267874|602774.2381723676|35.00827072137521|31.517029283149466",
            "35.00827072383671|31.517029225386523|2039|200816.30213267755|602774.2381723677|35.00827072137521|31.517029283149473",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
        if(differenceWhenComparingCoordinateValues != DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE) {
//            Console.WriteLine("no significant differenceWhenComparingCoordinateValues");
        }
        Assert.AreNotEqual(
            differenceWhenComparingCoordinateValues, 
            DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE
        );
    }

    /// <summary>
    /// The method can be used for comparing different versions for the same adapter
    /// (i.e. produced by different versions)
    /// In other words, if there would be (but currently not) a version 1.4.2 of ProjNet,
    /// then the following two files would be compared:
    ///     ProjNet_version_1.4.2.csv (in the parameter "file1")
    ///     ProjNet_version_1.4.1.csv (in the parameter "file2")
    /// The method can also be used for comparing versions from 
    /// different adapters e.g. comparing these two files:
    ///     ProjNet_version_1.4.1.csv
    ///     MightyLittleGeodesy_version_1.0.1.csv
    /// </summary>
    /// <param name="file1"></param>
    /// <param name="file2"></param>
    /// <param name="deltaValueForDifferencesToIgnore"></param>
    /// <param name="shouldAlsoDisplayDifferencesWhenValueIsMissing"></param>
    private void CompareWithRegressionFileContent(
        FileInfo file1,
        FileInfo file2,
        double deltaValueForDifferencesToIgnore, // if negative value then show ANY difference
        bool shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        bool shouldShowALLdifferences = deltaValueForDifferencesToIgnore < 0;
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("Will now compare the files " + file1.Name + " and " + file2.Name);
        IList<string> linesFromFile1 = GetAllLinesFromTextFileUTF8(file1);
        IList<string> linesFromFile2 = GetAllLinesFromTextFileUTF8(file2);

        Console.WriteLine("number of rows: " + linesFromFile1.Count);
        //Assert.AreEqual(linesFromFile1.Count, linesFromFile2.Count, "Not even the same number of results in the two files");
        if(linesFromFile1.Count != linesFromFile2.Count) {
            Console.WriteLine("Not even the same number of results in the two files: " + linesFromFile1.Count + " vs " + linesFromFile2.Count);
        }
        int numberOfRowsToIiterate = linesFromFile1.Count;
        for (int i = 0; i < numberOfRowsToIiterate; i++) {
            //Assert.AreEqual(linesFromFile1[i], linesFromFile2[i]);
            if(!linesFromFile1[i].Equals(linesFromFile2[i])) {
                if(shouldShowALLdifferences) {
                    Console.WriteLine("Diff lines:");
                    Console.WriteLine(linesFromFile1[i]);
                    Console.WriteLine(linesFromFile2[i]);
                }
                else {
                    ShowDifferenceIfSignificant(
                        linesFromFile1[i],
                        linesFromFile2[i],
                        deltaValueForDifferencesToIgnore,
                        shouldAlsoDisplayDifferencesWhenValueIsMissing
                    );
                }
            }
        }
        Console.WriteLine("-------------------------------------------------");
    }

    private DifferenceWhenComparingCoordinateValues ShowDifferenceIfSignificant(
        string lineFromFileWithRegressionResults,
        string lineFromFileWithRegressionResults2,
        double deltaValueForDifferencesToIgnore,
        bool shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        TestResultItem t1 = new TestResultItem(lineFromFileWithRegressionResults);
        TestResultItem t2 = new TestResultItem(lineFromFileWithRegressionResults2);
        DifferenceWhenComparingCoordinateValues diff = t1.IsDeltaDifferenceSignificant(t2, deltaValueForDifferencesToIgnore);
        if(
            (shouldAlsoDisplayDifferencesWhenValueIsMissing &&
                (
                    diff == DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE
                    ||
                    diff == DifferenceWhenComparingCoordinateValues.EXISTING_VS_NOT_EXISTING
                )
            )
            ||
            ( !shouldAlsoDisplayDifferencesWhenValueIsMissing &&
                (
                    diff == DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE
                )
            )
        ) {
            Console.WriteLine("Diff lines with significant delta " + deltaValueForDifferencesToIgnore + " : ");
            Console.WriteLine(lineFromFileWithRegressionResults);
            Console.WriteLine(lineFromFileWithRegressionResults2);
        }
        return diff;
    }

    private static IList<string> GetAllLinesFromTextFileUTF8(
        FileInfo file
    ) {
        var list = new List<string>();
        // the below "File.OpenText" opens an UTF8 encoded text file
        using(StreamReader sr = File.OpenText(file.FullName)) {
            string line;
            while( (line = sr.ReadLine()) != null ) {
                list.Add(line);
            }
        }
        return list;
    }

    /**
     * The FileInfo with coordinates are generated from another project "crsCodeGeneration" (a JVM project from which this .NET solution was ported).
     *  (the class 'CoordinateTestDataGenerator' which are creating the data from a relational database ("EPSG database")
     *  and from shapeFileInfo with polygon used for creating the coordinates as centroid points
     *  within a certain area where the EPSG code is defined to be used)
     *  Both the relational (SQL) database and the polygon was downloaded from EPSG website.
     */
    public static IList<EpsgCrsAndAreaCodeWithCoordinates> GetCoordinatesFromGeneratedCsvFile() {
        var list = new List<EpsgCrsAndAreaCodeWithCoordinates>();
        var lines = GetAllLinesFromTextFileUTF8(new FileInfo(INPUT_TEST_DATA_FILE));
        foreach(string line in lines) {
            EpsgCrsAndAreaCodeWithCoordinates epsgCrsAndAreaCodeWithCoordinates = CreateEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(line);
            list.Add(epsgCrsAndAreaCodeWithCoordinates);
        }
        return list;
    }

    private static EpsgCrsAndAreaCodeWithCoordinates CreateEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(
        string line
    ) {
        string trimmedLine = line.Trim();
        //Console.WriteLine("EpsgCrsAndAreaCodeWithCoordinates trimmedLine: " + trimmedLine);
        // e.g. "3006|1225|Sweden|17.083659606206545|61.98770256318016"
        string[] parts = trimmedLine.Split('|');
        Assert.AreEqual(5, parts.Length, "Problem with the expected parts in this line: " + trimmedLine);
        return new EpsgCrsAndAreaCodeWithCoordinates(
            int.Parse(parts[0]),    // epsgCrsCode
            int.Parse(parts[1]),    // epsgAreaCode
            parts[2],               // epsgAreaName
            Double.Parse(parts[3]), // centroidX
            Double.Parse(parts[4])  // centroidY
        );
    }


    private FileInfo GetLatestFileWithRegressionsResults(
        string partOfTheFileName
    ) {
        FileInfo[] res = GetFilesWithRegressionsResultsSortedWithLatestFirst(partOfTheFileName);
        if(res.Length < 1) throw new Exception("Could not find a file with this string as a part of the filename: " + partOfTheFileName);
        return res[0];
    }
    private FileInfo[] GetFilesWithRegressionsResultsSortedWithLatestFirst(
        string partOfTheFileName
    ) {
        DirectoryInfo directoryForRegressionsResults = GetDirectoryForRegressionsResults();
        //FileInfo[] files = directoryForRegressionsResults.GetFiles(partOfTheFileName); / fails
        FileInfo[] files = directoryForRegressionsResults.GetFiles("*" + partOfTheFileName +"*");
        SortFilesWithLatestFirst(files);
        return files;
    }

    /**
     * @param partOfTheFileName e.g. "DotSpatial" to match FileInfo names such as "DotSpatial_version_2.0.0_rc1.csv"
     */
    private void CompareTheTwoLatestVersion(
        string partOfTheFileName,
        double deltaValueForDifferencesToIgnore,
        bool shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        FileInfo[] files = GetFilesWithRegressionsResultsSortedWithLatestFirst(partOfTheFileName);
        if(files.Length < 2) {
            Console.WriteLine("There are not two files containing the filename part " + partOfTheFileName + " in the directory " + GetDirectoryForRegressionsResults().FullName);
            return;
        }
        CompareWithRegressionFileContent(
            files[0], 
            files[1], 
            deltaValueForDifferencesToIgnore, 
            shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    private void SortFilesWithLatestFirst(FileInfo[] files) {
        Array.Sort(files, (f1, f2) => {
            long diff = f2.LastWriteTime.Ticks - f1.LastWriteTime.Ticks;
            if (diff > 0) return 1;
            if (diff < 0) return -1;
            return 0;

        });
    }

    private FileInfo GetLatestDotSpatialFile() {
        return GetLatestFileWithRegressionsResults(PART_OF_FILENAME_DotSpatial);
    }
        
    private FileInfo GetLatestProjNetFile() {
        return GetLatestFileWithRegressionsResults(PART_OF_FILENAME_ProjNet);
    }
    private FileInfo GetLatestMightyLittleGeodesyFile() {
        return GetLatestFileWithRegressionsResults(PART_OF_FILENAME_MightyLittleGeodesy);
    }
}
}
//// The test results below were created when the following delta value was used:
//double DELTA_LIMIT_FOR_SUCCESS = 0.0001;
//-------------------------------
//testResults for CrsTransformationAdapterDotSpatial
//seconds: 4
//countOfSuccess: 5074
//countOfFailures: 1361
//-------------------------------
//testResults for CrsTransformationAdapterProjNet
//seconds: 5
//countOfSuccess: 2481
//countOfFailures: 3954
//-------------------------------
//testResults for CrsTransformationAdapterMightyLittleGeodesy
//seconds: 1
//countOfSuccess: 19
//countOfFailures: 6416
//-------------------------------
