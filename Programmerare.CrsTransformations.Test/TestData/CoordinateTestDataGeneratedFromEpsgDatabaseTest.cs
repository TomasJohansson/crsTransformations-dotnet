using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.CompositeTransformations;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using System;
using System.Collections.Generic;
using System.IO;

namespace Programmerare.CrsTransformations.TestData
{
/**
 * The CSV FileInfo used in this test:
 *  src/test/resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv
 * The above FileInfo ahs been created with the following class:
 *  \crsCodeGeneration\src\main\kotlin\com\programmerare\crsCodeGeneration\coordinateTestDataGenerator\CoordinateTestDataGenerator.kt
 * The relevant columns are the first column (EPSG code) and the last two column with WGS84 coordinates.
 * The WGS84 coordinate defines the "centroid" within an area where some other coordinate
 * system is used (and that other coordinate system is defined byt the EPSG code in the first column)
 * Thus the FileInfo defines a list of appropriate WGS84 coordinates which can be transformed back and forth
 * to/from the coordinate system in the first EPSG column.
 */
//@Disabled // you may want to temporary change this line if you want to run the "tests"
// (and also see comments in the class TestCategory regarding that this "test" FileInfo creates files and produces output to the console)
[Ignore("Not yet implemented")]
public class CoordinateTestDataGeneratedFromEpsgDatabaseTest {

    private const string OUTPUT_DIRECTORY_FOR_REGRESSION_RESULTS = "src/test/resources/regression_results";

    // the below FileInfo is used with method 'Resources.getResource' (the path is test/resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv )
    // and the content (columns) of the FileInfo are as below:
    // e.g. "3006|1225|Sweden|17.083659606206545|61.98770256318016"
    //      epsgCrsCode | epsgAreaCode | epsgAreaName | centroidX | centroidY
    private const string INPUT_TEST_DATA_FILE = "resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv";


    private bool shouldCreateNewRegressionFile = true;

    private const double DELTA_LIMIT_FOR_SUCCESS = 0.0001;

    private static IList<EpsgCrsAndAreaCodeWithCoordinates> list;

    [SetUp]
    public void before() {
        list = GetCoordinatesFromGeneratedCsvFile();
    }

    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // -------------------------------------------------------------------------------------
    // Below: "tests" labeled with 'TestCategory.SideEffectFileCreation'

    // To run all tests excluding tests labeled with @Tag("SlowTest")
    // as below, in IntelliJ IDEA:
    // Run --> Edit configuration --> Junit --> Test kind --> Tags --> Tag expression: !SlowTest

    [Test]
    // @Tag("SlowTest") // actually not at all slow but very fast since very few coordinate systems are supported
    //@Tag(TestCategory.SideEffectFileCreation) // test/resources/regression_results/
    public void testAllTransformationsInGeneratedCsvFileWithGoober() {
        TestResult testResultForGoober = runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationAdapterDotSpatial(), list);
        handleTestResults(
            testResultForGoober,
            DELTA_LIMIT_FOR_SUCCESS,
            shouldCreateNewRegressionFile,
            "_version_1.1" // build.gradle: implementation("com.github.goober:coordinate-transformation-library:1.1")
        );
    }

    // @Disabled
    //@Tag(TestCategory.SlowTest) // e.g. 224 seconds for this test method while all other (approx 80) tests (except those in this test class) take about one minute
    //@Tag(TestCategory.SideEffectFileCreation) // test/resources/regression_results/

    [Test] // currently not a real test with assertions but printing console output with differences
    [Category(TestCategory.SideEffectFileCreation)]
    [Category(TestCategory.SlowTest)] 
    //[Ignore(TestCategory.SlowTest)] 
    public void testAllTransformationsInGeneratedCsvFileWithGeoTools() {
        //    seconds: 224
        //    countOfSuccess: 4036
        //    countOfFailures: 2399
        TestResult testResultForGeoTools = runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationAdapterDotSpatial(), list);
        handleTestResults(
            testResultForGeoTools,
            DELTA_LIMIT_FOR_SUCCESS,
            shouldCreateNewRegressionFile,
            "_version_20.0"  // build.gradle: implementation("org.geotools:gt-main:20.0")
            // FileInfo created: "test/resources/regression_results/CrsTransformationAdapterGeoTools_version_20.0_.csv
        );
        // There are differences in the above generated FileInfo (when using version 20.0 instead of 19.1)
        // but when roughly looking at the files with WinMerge the differences seem to be very small.
        // However: TODO: use code to detect significant differences, and if those exist,
        // then try to figure out if it is improvement or the opposite.
        // If the later version seem to have introduced a bug/error then try to report it to the GeoTools project

        // TODO: compute standard deviations for the results e.g.
        // the deviations from the original coordinate when transforming back and forth,
        // and also compare them with each other and caluclate the standard deviation
        // from the median value ...
    }

    //@Test
    //@Tag(TestCategory.SlowTest) // e.g. 122 seconds for this test method while all other (approx 80) tests (except those in this test class) take about one minute
    //@Tag(TestCategory.SideEffectFileCreation)
    [Test]
    public void testAllTransformationsInGeneratedCsvFileWithGeoPackage() {
        //    seconds: 122
        //    countOfSuccess: 3918
        //    countOfFailures: 2517
        TestResult testResultForGeoPackage = runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationAdapterDotSpatial(), list);
        handleTestResults(
            testResultForGeoPackage,
            DELTA_LIMIT_FOR_SUCCESS,
            shouldCreateNewRegressionFile,
            "_version_3.1.0" // build.gradle: compile group: 'mil.nga.geopackage', name: 'geopackage', version: '3.1.0'
            // FileInfo created: "test/resources/regression_results/CrsTransformationAdapterGeoPackageNGA_version_3.1.0.csv
        );
        // The above created latest output FileInfo "CrsTransformationAdapterGeoPackageNGA_version_3.1.0.csv"
        // was identical with the FileInfo created for the previous version (generated by GeoPackage 3.0.0)
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

    //@Test // currently not a real test with assertions but printing console output with differences
    //@Tag(TestCategory.SideEffectPrintingConsoleOutput)
    public void compareResultsForDifferentVersionsOfGeoTools() {
        // filename e.g. "CrsTransformationAdapterGeoTools_version_20.0.csv"
        compareTheTwoLatestVersion(
            "DotSpatial",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    //@Test // currently not a real test with assertions but printing console output with differences
    //@Tag(TestCategory.SideEffectPrintingConsoleOutput)
    [Test]
    public void compareResultsForDifferentVersionsOfNGA() {
        // filename e.g. "CrsTransformationAdapterGeoPackageNGA_version_3.1.0.csv"
        compareTheTwoLatestVersion(
            "NGA",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    //@Test // currently not a real test with assertions but printing console output with differences
    //@Tag(TestCategory.SideEffectPrintingConsoleOutput)
    [Test]
    public void compareResultsForDifferentVersionsOfGoober() {
        // filename e.g. "CrsTransformationAdapterGooberCTL_version_1.1.csv"
        compareTheTwoLatestVersion(
            "Goober",
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

    private const double deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation = 0.00001;

    // -------------------------------------------------------------------------------------
    // Comparing the latest results of GeoTools with the results from the other files
    // (since GeoTools seem to support the greatest number of EPSG codes, based on the FileInfo sizes for the regression files)
    //@Test // currently not a real test with assertions but printing console output with differences
    //@Tag(TestCategory.SideEffectPrintingConsoleOutput)
    [Test]
    public void compareResultsForLatestGeoToolsAndGoober() {
        // filenames e.g. "CrsTransformationAdapterGeoTools_version_20.0.csv" and "CrsTransformationAdapterGooberCTL_version_1.1.csv"
        FileInfo geoToolsFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("GeoTools")[0];
        FileInfo gooberFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("Goober")[0];
        compareWithRegressionFileContent(
                geoToolsFile,
                gooberFile,
                deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation,
                false // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    //@Test // currently not a real test with assertions but printing console output with differences
    //@Tag(TestCategory.SideEffectPrintingConsoleOutput)
    [Test]
    public void compareResultsForLatestGeoToolsAndGeoPackageNGA() {
        FileInfo geoToolsFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("GeoTools")[0];
        FileInfo geoPackageNGAFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("NGA")[0];
        compareWithRegressionFileContent(
                geoToolsFile,
                geoPackageNGAFile,
                deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation,
                false // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
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

    //@Test // currently not a real test with assertions but printing console output with differences
    //@Tag(TestCategory.SideEffectPrintingConsoleOutput)
    [Test]
    public void findPotentialBuggyImplementations() { // similarly named method in another class, see comment there (current class name: CoordinateTestDataGeneratedFromEpsgDatabaseTest2')
        FileInfo geoToolsFileInfo = this.getFilesWithRegressionsResultsSortedWithLatesFirst("GeoTools")[0];
        FileInfo gooberFileInfo = this.getFilesWithRegressionsResultsSortedWithLatesFirst("Goober")[0];
        FileInfo ngaFileInfo = this.getFilesWithRegressionsResultsSortedWithLatesFirst("NGA")[0];

        throw new Exception(".NET todo below");
        //List<FileInfo> filesToCompare = Arrays.asList(geoToolsFile, gooberFile, ngaFile, orbisFile, projFile);
        //double deltaValueForDifferencesToIgnore = 0.001;
        //compareFiles(
        //    filesToCompare,
        //    deltaValueForDifferencesToIgnore
        //);
    }

    private void compareFiles(
        IList<FileInfo> filesToCompare,
        double deltaValueForDifferencesToIgnore
    ) {
        ResultAggregator resultAggregator = new ResultAggregator();
        bool shouldShowALLdifferences = deltaValueForDifferencesToIgnore < 0;
        List<List<String>> listOfRowsPerFile = new List<List<string>>();
        int numberOfRowsInFile = -1;
        foreach(FileInfo file in filesToCompare) {
            List<String> rowsInFile = getAllLinesFromTextFileUTF8(file);
            listOfRowsPerFile.Add(rowsInFile);
            resultAggregator.addRowsFromFile(rowsInFile, file);
            if(numberOfRowsInFile < 0) {
                numberOfRowsInFile = rowsInFile.Count;
            }
            else {
                if(rowsInFile.Count != numberOfRowsInFile) {
                    String errorMessage = "Not even the same number of rows in the files";
                    Console.WriteLine(errorMessage);
                    throw new Exception(errorMessage);
                }
            }
        }
        ISet<int> indexesForRowsWithSignificantDifference = resultAggregator.getIndexesForRowsWithSignificantDifference(deltaValueForDifferencesToIgnore);
        foreach (int ind in indexesForRowsWithSignificantDifference) {
            Console.WriteLine("-----------------");
            Console.WriteLine("index " + ind);
            for (int i=0; i < listOfRowsPerFile.Count; i++) {
                FileInfo file = filesToCompare[i];
                String rowContent = listOfRowsPerFile[i][ind];
                Console.WriteLine(rowContent + " ; " + file.Name);
            }
        }
        Console.WriteLine("-------------------------------------------------");
    }
    // -------------------------------------------------------------------------------------


    private TestResult runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(
            ICrsTransformationAdapter crsTransformationAdapter,
            IList<EpsgCrsAndAreaCodeWithCoordinates> coordinatesFromGeneratedCsvFile
    ) {
        List<TestResultItem> testResultItems = new List<TestResultItem>();
        int counter = 0;

        //long startTime = System.nanoTime();
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
        //long elapsedNanos = System.nanoTime() - startTime;
        //long totalNumberOfSecondsForAllTransformations = TimeUnit.NANOSECONDS.toSeconds(elapsedNanos);
        long totalNumberOfSecondsForAllTransformations = 432; // TODO
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
    private void handleTestResults(
        TestResult testResult,
        double deltaLimitForSuccess,
        bool shouldCreateNewRegressionFile,
        String fileNameSuffixExcludingExtension
    ) {
        Console.WriteLine("-------------------------------");
        Console.WriteLine("testResults for " + testResult.adapter.GetType().Name);
        Console.WriteLine("seconds: " + testResult.totalNumberOfSecondsForAllTransformations);
        List<TestResultItem> testResultItems = testResult.testResultItems;
        int countOfFailures = 0;
        int countOfSuccess = 0;
        bool isSuccess;
        IList<string> linesWithCurrentResults = new List<string>();
        foreach (TestResultItem testResultItem in testResultItems) {
            String s = testResultItem.getResultStringForRegressionFile();
            linesWithCurrentResults.Add(s);
            isSuccess = testResultItem.isSuccessfulTransformationFromWGS84();
            if (isSuccess) {
                isSuccess  = testResultItem.isSuccessfulTransformationBackToWGS84();
                if (isSuccess) {
                    CrsCoordinate inputCoordinateWGS84 = testResultItem.getInputCoordinateWGS84();
                    //Coordinate wgs84Again = resultOfTransformationBackToWGS84.getOutputCoordinate();
                    CrsCoordinate wgs84Again = testResultItem.getCoordinateOutputTransformationBackToWGS84();
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

        FileInfo file = getFileForRegressionResults(testResult.adapter, fileNameSuffixExcludingExtension);
        if (shouldCreateNewRegressionFile) {
            createNewRegressionFile(file, linesWithCurrentResults);
        }
    }

    private FileInfo getFileForRegressionResults(
        ICrsTransformationAdapter adapter, 
        string fileNameSuffixExcludingExtension
    ) {
        DirectoryInfo directoryForRegressionsResults = getDirectoryForRegressionsResults();
            //new FileInfo()
        FileInfo file = new FileInfo(directoryForRegressionsResults.Name + "" + adapter.GetType().Name + fileNameSuffixExcludingExtension + ".csv");
        return file;
    }

    private DirectoryInfo getDirectoryForRegressionsResults() {
        //// https://docs.oracle.com/javase/7/docs/api/java/io/File.html
        //// "... system property user.dir, and is typically the directory in which the Java virtual machine was invoked"
        //FileInfo userDir = new FileInfo(System.getProperty("user.dir"));
        //FileInfo directoryForRegressionsResults = new File(userDir, OUTPUT_DIRECTORY_FOR_REGRESSION_RESULTS);
        //if (!directoryForRegressionsResults.exists() || !directoryForRegressionsResults.isDirectory()) {
        //    throw new Exception("Directory does not exist: " + directoryForRegressionsResults.getAbsolutePath());
        //}
        //return directoryForRegressionsResults;
        throw new Exception("Not implemented with .NET yet");
    }

    private void createNewRegressionFile(FileInfo file, IList<string> linesWithCurrentResults) {
        //try {
        //    Path write = Files.write(file.toPath(), linesWithCurrentResults, Charset.forName("UTF-8"));
        //} catch (IOException e) {
        //    e.printStackTrace();
        //}
        throw new Exception("Not implemented with .NET yet");
    }

    //@Test
    //@Tag(TestCategory.SideEffectPrintingConsoleOutput)
    [Test]
    public void showDifferenceIfSignificantTest() {
        DifferenceWhenComparingCoordinateValues differenceWhenComparingCoordinateValues = showDifferenceIfSignificant(
            "35.00827072383671|31.517029225386523|2039|200816.30213267874|602774.2381723676|35.00827072137521|31.517029283149466",
            "35.00827072383671|31.517029225386523|2039|200816.30213267755|602774.2381723677|35.00827072137521|31.517029283149473",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
        if(differenceWhenComparingCoordinateValues != DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE) {
//            Console.WriteLine("no significant differenceWhenComparingCoordinateValues");
        }
        Assert.AreNotEqual(differenceWhenComparingCoordinateValues, DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE);
    }

    private void compareWithRegressionFileContent(
        FileInfo fileWithLatestResults,
        FileInfo fileWithSecondLatestResults,
        double deltaValueForDifferencesToIgnore, // if negative value then show ANY difference
        bool shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        bool shouldShowALLdifferences = deltaValueForDifferencesToIgnore < 0;
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("Will now compare the files " + fileWithLatestResults.Name + " and " + fileWithSecondLatestResults.Name);
        List<String> linesWithLatestResults = getAllLinesFromTextFileUTF8(fileWithLatestResults);
        List<String> linesWithSecondLatestResults = getAllLinesFromTextFileUTF8(fileWithSecondLatestResults);

        // assertEquals(linesWithLatestResults.size(), linesWithSecondLatestResults.size(), "Not even the same number of results as previously");
        if(linesWithLatestResults.Count != linesWithSecondLatestResults.Count) {
            Console.WriteLine("Not even the same number of results as previously: " + linesWithLatestResults.Count + " vs " + linesWithSecondLatestResults.Count);
        }
        for (int i = 0; i < linesWithLatestResults.Count; i++) {
            //assertEquals(linesWithLatestResults.get(i), linesWithSecondLatestResults.get(i));
            if(!linesWithLatestResults[i].Equals(linesWithSecondLatestResults[i])) {
                if(shouldShowALLdifferences) {
                    Console.WriteLine("Diff lines:");
                    Console.WriteLine(linesWithLatestResults[i]);
                    Console.WriteLine(linesWithSecondLatestResults[i]);
                }
                else {
                    showDifferenceIfSignificant(
                        linesWithLatestResults[i],
                        linesWithSecondLatestResults[i],
                        deltaValueForDifferencesToIgnore,
                        shouldAlsoDisplayDifferencesWhenValueIsMissing
                    );
                }
            }
        }
        Console.WriteLine("-------------------------------------------------");
    }

    private DifferenceWhenComparingCoordinateValues showDifferenceIfSignificant(
        String lineFromFileWithRegressionResults,
        String lineFromFileWithRegressionResults2,
        double deltaValueForDifferencesToIgnore,
        bool shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        TestResultItem t1 = new TestResultItem(lineFromFileWithRegressionResults);
        TestResultItem t2 = new TestResultItem(lineFromFileWithRegressionResults2);
        DifferenceWhenComparingCoordinateValues diff = t1.isDeltaDifferenceSignificant(t2, deltaValueForDifferencesToIgnore);
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

    private List<String> getAllLinesFromTextFileUTF8(FileInfo file) {
        //try {
        //    return Resources.readLines(file.toURI().toURL(), Charset.forName("UTF-8"));
        //} catch (IOException e) {
        //    throw new RuntimeException(e);
        //}
        throw new Exception(".NET todo");
    }

    /**
     * The FileInfo with coordinates are generated from another module "crsCodeGeneration".
     *  (the class 'CoordinateTestDataGenerator' which are creating the data from a MS Access file
     *  and from shapeFileInfo with polygon used for creating the coordinates as centroid points
     *  within a certain area where the EPSG code is defined to be used)
     */
    public static IList<EpsgCrsAndAreaCodeWithCoordinates> GetCoordinatesFromGeneratedCsvFile() {
        var list = new List<EpsgCrsAndAreaCodeWithCoordinates>();
        using(StreamReader sr = File.OpenText(INPUT_TEST_DATA_FILE)) {
            string line;
            while( (line = sr.ReadLine()) != null ) {
                EpsgCrsAndAreaCodeWithCoordinates epsgCrsAndAreaCodeWithCoordinates = createEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(line);
                list.Add(epsgCrsAndAreaCodeWithCoordinates);
            }
        }
        return list;
    }

    private static EpsgCrsAndAreaCodeWithCoordinates createEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(string line) {
        string trimmedLine = line.Trim();
        //Console.WriteLine("EpsgCrsAndAreaCodeWithCoordinates trimmedLine: " + trimmedLine);
        // e.g. "3006|1225|Sweden|17.083659606206545|61.98770256318016"
        string[] parts = trimmedLine.Split('|');
        Assert.AreEqual(5, parts.Length, "Problem with the expected parts in this line: " + trimmedLine);
        return new EpsgCrsAndAreaCodeWithCoordinates(
            int.Parse(parts[0]),     // epsgCrsCode
            int.Parse(parts[1]),     // epsgAreaCode
            parts[2],                       // epsgAreaName
            Double.Parse(parts[3]),   // centroidX
            Double.Parse(parts[4])    // centroidY
        );
    }


    private FileInfo[] getFilesWithRegressionsResultsSortedWithLatesFirst(
        String partOfTheFileName
    ) {
        //FileInfo directoryForRegressionsResults = getDirectoryForRegressionsResults();
        //FileInfo[] files = directoryForRegressionsResults.listFiles(FileInfo -> file.getName().contains(partOfTheFileName));
        //sortFilesWithLatestFirst(files);
        //foreach (FileInfo f in files) {
        //    Console.WriteLine(f.Name);
        //}
        //return files;
        throw new Exception(".NET todo");
    }

    /**
     * @param partOfTheFileName e.g. "GeoTools" to match FileInfo names such as "CrsTransformationAdapterGeoTools_version_19.1"
     */
    private void compareTheTwoLatestVersion(
        String partOfTheFileName,
        double deltaValueForDifferencesToIgnore,
        bool shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        FileInfo[] files = getFilesWithRegressionsResultsSortedWithLatesFirst(partOfTheFileName);
        if(files.Length < 2) {
            Console.WriteLine("There are not two files containing the filename part " + partOfTheFileName + " in the directory " + getDirectoryForRegressionsResults().FullName);
            return;
        }
        //compareWithRegressionFileContent(files[0], files[1], 0.0000000000000001);
        compareWithRegressionFileContent(files[0], files[1], deltaValueForDifferencesToIgnore, shouldAlsoDisplayDifferencesWhenValueIsMissing);
    }

    private void sortFilesWithLatestFirst(FileInfo[] files) {
        //Arrays.sort(files, new Comparator<File>() {
        //    @Override
        //    public int compare(FileInfo o1, FileInfo o2) {
        //        long diff = o2.lastModified() - o1.lastModified();
        //        if(diff > 0) return 1;
        //        if(diff < 0) return -1;
        //        return 0;
        //    }
        //});
        throw new Exception(".NET todo");
    }

}
}
//// The test results below were created when the following delta value was used:
//double DELTA_LIMIT_FOR_SUCCESS = 0.0001;
//-------------------------------
//testResults for CrsTransformationAdapterGooberCTL
//seconds: 2
//countOfSuccess: 19
//countOfFailures: 6416
//-------------------------------
//testResults for CrsTransformationAdapterProj4J
//seconds: 222
//countOfSuccess: 3916
//countOfFailures: 2519
//-------------------------------
//testResults for CrsTransformationAdapterOrbisgisCTS
//seconds: 455
//countOfSuccess: 3799
//countOfFailures: 2636
//-------------------------------
//testResults for CrsTransformationAdapterGeoTools
//seconds: 210
//countOfSuccess: 4036
//countOfFailures: 2399
//-------------------------------
//testResults for CrsTransformationAdapterGeoPackageNGA
//seconds: 249
//countOfSuccess: 3918
//countOfFailures: 2517
//-------------------------------
