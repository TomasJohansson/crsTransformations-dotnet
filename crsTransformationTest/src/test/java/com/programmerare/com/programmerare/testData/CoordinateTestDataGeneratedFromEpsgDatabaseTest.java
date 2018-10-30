package com.programmerare.com.programmerare.testData;

import com.google.common.io.Resources;
import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgCode;
import com.programmerare.crsTransformationFacadeGeoPackageNGA.CrsTransformationFacadeGeoPackageNGA;
import com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.TransformResult;
import org.junit.jupiter.api.*;

import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Comparator;
import java.util.List;
import java.util.concurrent.TimeUnit;

import static org.junit.jupiter.api.Assertions.*;

// TODO: programmatically compare the results in two ways:
//  - Compare results with itself with different versions of the same library when having done an upgrade
//  - Compare results with different libraries

/**
 * The CSV file used in this test:
 *  src/test/resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv
 * The above file ahs been created with the following class:
 *  \crsCodeGeneration\src\main\kotlin\com\programmerare\crsCodeGeneration\coordinateTestDataGenerator\CoordinateTestDataGenerator.kt
 * The relevant columns are the first column (EPSG code) and the last two column with WGS84 coordinates.
 * The WGS84 coordinate defines the "centroid" within an area where some other coordinate
 * system is used (and that other coordinate system is defined byt the EPSG code in the first column)
 * Thus the file defines a list of appropriate WGS84 coordinates which can be transformed back and forth
 * to/from the coordinate system in the first EPSG column.
 */
class CoordinateTestDataGeneratedFromEpsgDatabaseTest {

    private final static String OUTPUT_DIRECTORY_FOR_REGRESSION_RESULTS = "src/test/resources/regression_results";

    // the below file is used with method 'Resources.getResource' (the path is test/resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv )
    private final static String INPUT_TEST_DATA_FILE = "generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv";

    private boolean createNewRegressionFile = true;

    private final static double DELTA_LIMIT_FOR_SUCCESS = 0.0001;

    private static List<EpsgCrsAndAreaCodeWithCoordinates> list;

    @BeforeAll
    static void before() {
        list = getCoordinatesFromGeneratedCsvFile();
    }

    // To run all tests excluding tests labeled with @Tag("SlowTest")
    // as below, in IntelliJ IDEA:
    // Run --> Edit configuration --> Junit --> Test kind --> Tags --> Tag expression: !SlowTest

    @Test
    // @Tag("SlowTest") // actually not at all slow but very fast since very few coordinate systems are supported
    @Tag("SideEffectFileCreation") // test/resources/regression_results/
    void testAllTransformationsInGeneratedCsvFileWithGoober() {
        TestResult testResultForGoober = runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationFacadeGooberCTL(), list);
        handleTestResults(
            testResultForGoober,
            DELTA_LIMIT_FOR_SUCCESS,
            createNewRegressionFile,
            "_version_1.1" // build.gradle: implementation("com.github.goober:coordinate-transformation-library:1.1")
        );
    }

    @Test
    // @Disabled
    @Tag("SlowTest") // e.g. 224 seconds for this test method while all other (approx 80) tests (except those in this test class) take about one minute
    @Tag("SideEffectFileCreation") // test/resources/regression_results/
    void testAllTransformationsInGeneratedCsvFileWithGeoTools() {
        //    seconds: 224
        //    countOfSuccess: 4036
        //    countOfFailures: 2399
        TestResult testResultForGeoTools = runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationFacadeGeoTools(), list);
        handleTestResults(
            testResultForGeoTools,
            DELTA_LIMIT_FOR_SUCCESS,
            createNewRegressionFile,
            "_version_20.0"  // build.gradle: implementation("org.geotools:gt-main:20.0")
            // file created: "test/resources/regression_results/CrsTransformationFacadeGeoTools_version_20.0_.csv
        );
        // There are differences in the above generated file (when using version 20.0 instead of 19.1)
        // but when roughly looking at the files with WinMerge the differences seem to be very small.
        // However: TODO: use code to detect significant differences, and if those exist,
        // then try to figure out if it is improvement or the opposite.
        // If the later version seem to have introduced a bug/error then try to report it to the GeoTools project

        // TODO: compute standard deviations for the results e.g.
        // the deviations from the original coordinate when transforming back and forth,
        // and also compare them with each other and caluclate the standard deviation
        // from the median value ...
    }

    @Test
    @Tag("SlowTest") // e.g. 122 seconds for this test method while all other (approx 80) tests (except those in this test class) take about one minute
    @Tag("SideEffectFileCreation")
    void testAllTransformationsInGeneratedCsvFileWithGeoPackage() {
        //    seconds: 122
        //    countOfSuccess: 3918
        //    countOfFailures: 2517
        TestResult testResultForGeoPackage = runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationFacadeGeoPackageNGA(), list);
        handleTestResults(
            testResultForGeoPackage,
            DELTA_LIMIT_FOR_SUCCESS,
            createNewRegressionFile,
            "_version_3.1.0" // build.gradle: compile group: 'mil.nga.geopackage', name: 'geopackage', version: '3.1.0'
            // file created: "test/resources/regression_results/CrsTransformationFacadeGeoPackageNGA_version_3.1.0.csv
        );
        // The above created latest output file "CrsTransformationFacadeGeoPackageNGA_version_3.1.0.csv"
        // was identical with the file created for the previous version (generated by GeoPackage 3.0.0)
    }

    @Test
    @Tag("SlowTest") // e.g. 201 seconds for this test method while all other (approx 80) tests (except those in this test class) take about one minute
    @Tag("SideEffectFileCreation")
    void testAllTransformationsInGeneratedCsvFileWithProj4J() {
        //    seconds: 201
        //    countOfSuccess: 3916
        //    countOfFailures: 2519
        TestResult testResultForProj4J = runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationFacadeProj4J(), list);
        handleTestResults(
            testResultForProj4J,
            DELTA_LIMIT_FOR_SUCCESS,
            createNewRegressionFile,
            "_version_0.1.0" // build.gradle: implementation("org.osgeo:proj4j:0.1.0")
        );
    }

    @Test
    @Tag("SlowTest") // e.g. 384 seconds for this test method while all other (approx 80) tests (except those in this test class) take about one minute
    @Tag("SideEffectFileCreation")
    void testAllTransformationsInGeneratedCsvFileWithOrbisgis() {
        //    seconds: 384
        //    countOfSuccess: 3799
        //    countOfFailures: 2636
        TestResult testResultForOrbisgis = runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(new CrsTransformationFacadeOrbisgisCTS(), list);
        handleTestResults(
            testResultForOrbisgis,
            DELTA_LIMIT_FOR_SUCCESS,
            createNewRegressionFile,
            "_version_1.5.1" // build.gradle: implementation("org.orbisgis:cts:1.5.1")
        );
    }

    private TestResult runAllTransformationsOfTheCoordinatesInTheGeneratedCsvFile(
        CrsTransformationFacade crsTransformationFacade,
        List<EpsgCrsAndAreaCodeWithCoordinates> coordinatesFromGeneratedCsvFile
    ) {
        ArrayList<TestResultItem> testResultItems = new ArrayList<>();
        int counter = 0;

        long startTime = System.nanoTime();
        for (EpsgCrsAndAreaCodeWithCoordinates item : coordinatesFromGeneratedCsvFile) {
            final Coordinate inputCoordinateWGS84 = Coordinate.createFromXLongitudeYLatitude(item.centroidX, item.centroidY, EpsgCode.WORLD__WGS_84__4326);
            final TransformResult resultOfTransformationFromWGS84 = crsTransformationFacade.transform(inputCoordinateWGS84, item.epsgCrsCode);
            TransformResult resultOfTransformationBackToWGS84 = null;
            if (resultOfTransformationFromWGS84.isSuccess()) {
                resultOfTransformationBackToWGS84 = crsTransformationFacade.transform(resultOfTransformationFromWGS84.getOutputCoordinate(), EpsgCode.WORLD__WGS_84__4326);
            }
            testResultItems.add(new TestResultItem(item, inputCoordinateWGS84, resultOfTransformationFromWGS84, resultOfTransformationBackToWGS84));
            if (counter++ % 500 == 0) // just to show some progress
                System.out.println(this.getClass().getSimpleName() + " , counter: " + counter + " (of the total " + coordinatesFromGeneratedCsvFile.size() + ") for facade " + crsTransformationFacade.getClass().getSimpleName()); // to show some progress
            // if(counter > 300) break;
        }
        long elapsedNanos = System.nanoTime() - startTime;
        long totalNumberOfSecondsForAllTransformations = TimeUnit.NANOSECONDS.toSeconds(elapsedNanos);
        return new TestResult(crsTransformationFacade, totalNumberOfSecondsForAllTransformations, testResultItems);
    }

    /**
     * Iterates the test results and counts the number of successes and failures
     * including considering a maximum delta difference between the coordinates
     * when having transformed back and forth.
     * While iterating, lines of strings are also created, with the resulting coordinates.
     * Depennding on a boolean parameter, those string lines are either written to
     * a file, or compared with a previous created file.
     *
     * @param testResult
     * @param deltaLimitForSuccess
     * @param createNewRegressionFile if false, then instead compare with previous regression file
     */
    private void handleTestResults(
        TestResult testResult,
        double deltaLimitForSuccess,
        boolean createNewRegressionFile,
        String fileNameSuffixExcludingExtension
    ) {
        System.out.println("-------------------------------");
        System.out.println("testResults for " + testResult.facade.getClass().getSimpleName());
        System.out.println("seconds: " + testResult.totalNumberOfSecondsForAllTransformations);
        List<TestResultItem> testResultItems = testResult.testResultItems;
        int countOfFailures = 0;
        int countOfSuccess = 0;
        boolean isSuccess;
        ArrayList<String> linesWithCurrentResults = new ArrayList<>();
        for (TestResultItem testResultItem : testResultItems) {
            String s = testResultItem.getResultStringForRegressionFile();
            linesWithCurrentResults.add(s);
            isSuccess = testResultItem.isSuccessfulTransformationFromWGS84();
            if (isSuccess) {
                isSuccess  = testResultItem.isSuccessfulTransformationBackToWGS84();
                if (isSuccess) {
                    Coordinate inputCoordinateWGS84 = testResultItem.getInputCoordinateWGS84();
                    //Coordinate wgs84Again = resultOfTransformationBackToWGS84.getOutputCoordinate();
                    Coordinate wgs84Again = testResultItem.getCoordinateOutputTransformationBackToWGS84();
                    final double deltaLong = Math.abs(inputCoordinateWGS84.getXLongitude() - wgs84Again.getXLongitude());
                    final double deltaLat = Math.abs(inputCoordinateWGS84.getYLatitude() - wgs84Again.getYLatitude());
                    isSuccess = deltaLong < deltaLimitForSuccess && deltaLat < deltaLimitForSuccess;
                }
            }
            if (isSuccess) {
                countOfSuccess++;
            } else {
                countOfFailures++;
            }
        }
        System.out.println("countOfSuccess: " + countOfSuccess);
        System.out.println("countOfFailures: " + countOfFailures);
        System.out.println("-------------------------------");

        final File file = getFileForRegressionResults(testResult.facade, fileNameSuffixExcludingExtension);
        if (createNewRegressionFile) {
            createNewRegressionFile(file, linesWithCurrentResults);
        }
    }

    private File getFileForRegressionResults(CrsTransformationFacade facade, String fileNameSuffixExcludingExtension) {
        File directoryForRegressionsResults = getDirectoryForRegressionsResults();
        File file = new File(directoryForRegressionsResults, facade.getClass().getSimpleName() + fileNameSuffixExcludingExtension + ".csv");
        return file;
    }

    private File getDirectoryForRegressionsResults() {
        // https://docs.oracle.com/javase/7/docs/api/java/io/File.html
        // "... system property user.dir, and is typically the directory in which the Java virtual machine was invoked"
        File userDir = new File(System.getProperty("user.dir"));
        File directoryForRegressionsResults = new File(userDir, OUTPUT_DIRECTORY_FOR_REGRESSION_RESULTS);
        if (!directoryForRegressionsResults.exists() || !directoryForRegressionsResults.isDirectory()) {
            throw new RuntimeException("Directory does not exist: " + directoryForRegressionsResults.getAbsolutePath());
        }
        return directoryForRegressionsResults;
    }

    private void createNewRegressionFile(File file, ArrayList<String> linesWithCurrentResults) {
        try {
            Path write = Files.write(file.toPath(), linesWithCurrentResults, Charset.forName("UTF-8"));
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    // -------------------------------------------------------------------------------------
    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForDifferentVersionsOfGeoTools() {
        // filename e.g. "CrsTransformationFacadeGeoTools_version_20.0.csv"
        compareTheTwoLatestVersion(
            "GeoTools",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForDifferentVersionsOfNGA() {
        // filename e.g. "CrsTransformationFacadeGeoPackageNGA_version_3.1.0.csv"
        compareTheTwoLatestVersion(
            "NGA",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForDifferentVersionsOfGoober() {
        // filename e.g. "CrsTransformationFacadeGooberCTL_version_1.1.csv"
        compareTheTwoLatestVersion(
            "Goober",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForDifferentVersionsOfOrbis() {
        // filename e.g. "CrsTransformationFacadeOrbisgisCTS_version_1.5.1.csv"
        compareTheTwoLatestVersion(
            "Orbis",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForDifferentVersionsOfProj4J() {
        // filename e.g. "CrsTransformationFacadeProj4J_version_0.1.0.csv"
        compareTheTwoLatestVersion(
            "Proj4J",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    private final static double deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation = 0.000000000001;

    private final static double deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation = 0.00001;
    // -------------------------------------------------------------------------------------

    // -------------------------------------------------------------------------------------
    // Comparing the latest results of GeoTools with the results from the other files
    // (since GeoTools seem to support the greatest number of EPSG codes, based on the file sizes for the regression files)
    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForLatestGeoToolsAndGoober() {
        // filenames e.g. "CrsTransformationFacadeGeoTools_version_20.0.csv" and "CrsTransformationFacadeGooberCTL_version_1.1.csv"
        File geoToolsFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("GeoTools")[0];
        File gooberFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("Goober")[0];
        compareWithRegressionFileContent(
            geoToolsFile,
            gooberFile,
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation,
            false // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForLatestGeoToolsAndGeoPackageNGA() {
        File geoToolsFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("GeoTools")[0];
        File geoPackageNGAFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("NGA")[0];
        compareWithRegressionFileContent(
            geoToolsFile,
            geoPackageNGAFile,
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation,
            false // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForLatestGeoToolsAndProj4J() {
        File geoToolsFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("GeoTools")[0];
        File proj4JFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("Proj4J")[0];
        compareWithRegressionFileContent(
            geoToolsFile,
            proj4JFile,
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation,
            false // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }

    @Test // currently not a real test with assertions but printing console output with differences
    @Tag("SideEffectPrintingConsoleOutput")
    void compareResultsForLatestGeoToolsAndOrbisgis() {
        File geoToolsFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("GeoTools")[0];
        File orbisFile = this.getFilesWithRegressionsResultsSortedWithLatesFirst("Orbis")[0];
        compareWithRegressionFileContent(
            geoToolsFile,
            orbisFile,
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForDIFFERENTImplementation,
            false // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
    }
    // -------------------------------------------------------------------------------------
    // TODO: create some method which will compare files for more than two implementations
    // at a time, to find the outliers that are signinficantly different for the others,
    // i.e. to make it easier to find implementations which are potentially very incorrect (buggy)
    // for transformatsion regarding a certain EPSG code where the significant differences were found
    // -------------------------------------------------------------------------------------

    @Test
    @Tag("SideEffectPrintingConsoleOutput")
    void showDifferenceIfSignificantTest() {
        TestResultItem.DIFFERENCE difference = showDifferenceIfSignificant(
            "35.00827072383671|31.517029225386523|2039|200816.30213267874|602774.2381723676|35.00827072137521|31.517029283149466",
            "35.00827072383671|31.517029225386523|2039|200816.30213267755|602774.2381723677|35.00827072137521|31.517029283149473",
            deltaValueForDifferencesToIgnoreWhenComparingDifferentVersionForSameImplementation,
            true // shouldAlsoDisplayDifferencesWhenValueIsMissing
        );
        if(difference != TestResultItem.DIFFERENCE.SIGNIFICANT_VALUE_DIFFERENCE) {
//            System.out.println("no significant difference");
        }
        assertNotEquals(difference, TestResultItem.DIFFERENCE.SIGNIFICANT_VALUE_DIFFERENCE);
    }

    private void compareWithRegressionFileContent(
        File fileWithLatestResults,
        File fileWithSecondLatestResults,
        double deltaValueForDifferencesToIgnore, // if negative value then show ANY difference
        boolean shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        boolean shouldShowALLdifferences = deltaValueForDifferencesToIgnore < 0;
        System.out.println("-------------------------------------------------");
        System.out.println("Will now compare the files " + fileWithLatestResults.getName() + " and " + fileWithSecondLatestResults.getName());
        final List<String> linesWithLatestResults = getAllLinesFromTextFileUTF8(fileWithLatestResults);
        final List<String> linesWithSecondLatestResults = getAllLinesFromTextFileUTF8(fileWithSecondLatestResults);

        // assertEquals(linesWithLatestResults.size(), linesWithSecondLatestResults.size(), "Not even the same number of results as previously");
        if(linesWithLatestResults.size() != linesWithSecondLatestResults.size()) {
            System.out.println("Not even the same number of results as previously: " + linesWithLatestResults.size() + " vs " + linesWithSecondLatestResults.size());
        }
        for (int i = 0; i < linesWithLatestResults.size(); i++) {
            //assertEquals(linesWithLatestResults.get(i), linesWithSecondLatestResults.get(i));
            if(!linesWithLatestResults.get(i).equals(linesWithSecondLatestResults.get(i))) {
                if(shouldShowALLdifferences) {
                    System.out.println("Diff lines:");
                    System.out.println(linesWithLatestResults.get(i));
                    System.out.println(linesWithSecondLatestResults.get(i));
                }
                else {
                    showDifferenceIfSignificant(
                        linesWithLatestResults.get(i),
                        linesWithSecondLatestResults.get(i),
                        deltaValueForDifferencesToIgnore,
                        shouldAlsoDisplayDifferencesWhenValueIsMissing
                    );
                }
            }
        }
        System.out.println("-------------------------------------------------");
    }

    private TestResultItem.DIFFERENCE showDifferenceIfSignificant(
        String lineFromFileWithRegressionResults,
        String lineFromFileWithRegressionResults2,
        double deltaValueForDifferencesToIgnore,
        boolean shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        TestResultItem t1 = new TestResultItem(lineFromFileWithRegressionResults);
        TestResultItem t2 = new TestResultItem(lineFromFileWithRegressionResults2);
        TestResultItem.DIFFERENCE diff = t1.isDeltaDifferenceSignificant(t2, deltaValueForDifferencesToIgnore);
        if(
            (shouldAlsoDisplayDifferencesWhenValueIsMissing &&
                (
                    diff == TestResultItem.DIFFERENCE.SIGNIFICANT_VALUE_DIFFERENCE
                    ||
                    diff == TestResultItem.DIFFERENCE.EXISTING_VS_NOT_EXISTING
                )
            )
            ||
            ( !shouldAlsoDisplayDifferencesWhenValueIsMissing &&
                (
                    diff == TestResultItem.DIFFERENCE.SIGNIFICANT_VALUE_DIFFERENCE
                )
            )
        ) {
            System.out.println("Diff lines with significant delta " + deltaValueForDifferencesToIgnore + " : ");
            System.out.println(lineFromFileWithRegressionResults);
            System.out.println(lineFromFileWithRegressionResults2);
        }
        return diff;
    }

    private List<String> getAllLinesFromTextFileUTF8(File file) {
        try {
            return Resources.readLines(file.toURI().toURL(), Charset.forName("UTF-8"));
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }

    /**
     * The file with coordinates are generated from another module "crsCodeGeneration".
     *  (the class 'CoordinateTestDataGenerator' which are creating the data from a MS Access file
     *  and from shapefile with polygon used for creating the coordinates as centroid points
     *  within a certain area where the EPSG code is defined to be used)
     */
    private static List<EpsgCrsAndAreaCodeWithCoordinates> getCoordinatesFromGeneratedCsvFile() {
        final ArrayList<EpsgCrsAndAreaCodeWithCoordinates> list = new ArrayList<>();
        try {
            final URL url = Resources.getResource(INPUT_TEST_DATA_FILE);
            final List<String> lines = Resources.readLines(url, Charset.forName("UTF-8"));
            for (String line : lines) {
                final EpsgCrsAndAreaCodeWithCoordinates epsgCrsAndAreaCodeWithCoordinates = createEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(line);
                list.add(epsgCrsAndAreaCodeWithCoordinates);
            }
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
        return list;
    }

    // @Test
//    void temporaryTestAsStartingPointOfExecutionWhenTroubleShooting() {
//        testOneRowFromCsvFile(
//            new CrsTransformationFacadeGeoPackageNGA(),
//            "3006|1225|Sweden|17.083659606206545|61.98770256318016"
//        );
//    }

//    /**
//     *
//     * @param crsTransformationFacade
//     * @param oneRowFromCsvFile e.g. "3006|1225|Sweden|17.083659606206545|61.98770256318016"
//     *  (can be copied from the file "generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv" )
//     */
//    private void testOneRowFromCsvFile(
//        CrsTransformationFacade crsTransformationFacade,
//        String oneRowFromCsvFile
//    ) {
//        EpsgCrsAndAreaCodeWithCoordinates item = createEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(oneRowFromCsvFile);
//        final Coordinate inputCoordinateWGS84 = Coordinate.createFromXLongitudeYLatitude(item.centroidX, item.centroidY, EpsgCode.WORLD__WGS_84__4326);
//        final TransformResult resultOfTransformationFromWGS84 = crsTransformationFacade.transform(inputCoordinateWGS84, item.epsgCrsCode);
//        if(!resultOfTransformationFromWGS84.isSuccess()) {
//            System.out.println(resultOfTransformationFromWGS84.getException());
//        }
//        assertTrue(resultOfTransformationFromWGS84.isSuccess());
//    }
    private static EpsgCrsAndAreaCodeWithCoordinates createEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(String line) {
        final String trimmedLine = line.trim();
        // e.g. "3006|1225|Sweden|17.083659606206545|61.98770256318016"
        final String[] parts = trimmedLine.split("\\|");
        return new EpsgCrsAndAreaCodeWithCoordinates(
            Integer.parseInt(parts[0]),     // epsgCrsCode
            Integer.parseInt(parts[1]),     // epsgAreaCode
            parts[2],                       // epsgAreaName
            Double.parseDouble(parts[3]),   // centroidX
            Double.parseDouble(parts[4])    // centroidY
        );
    }


    private File[] getFilesWithRegressionsResultsSortedWithLatesFirst(
        String partOfTheFileName
    ) {
        File directoryForRegressionsResults = getDirectoryForRegressionsResults();
        File[] files = directoryForRegressionsResults.listFiles(file -> file.getName().contains(partOfTheFileName));
        sortFilesWithLatestFirst(files);
        for (File f : files) {
            System.out.println(f.getName());
        }
        return files;
    }

    /**
     * @param partOfTheFileName e.g. "GeoTools" to match file names such as "CrsTransformationFacadeGeoTools_version_19.1"
     */
    private void compareTheTwoLatestVersion(
        String partOfTheFileName,
        double deltaValueForDifferencesToIgnore,
        boolean shouldAlsoDisplayDifferencesWhenValueIsMissing
    ) {
        File[] files = getFilesWithRegressionsResultsSortedWithLatesFirst(partOfTheFileName);
        if(files.length < 2) {
            System.out.println("There are not two files containing the filename part " + partOfTheFileName + " in the directory " + getDirectoryForRegressionsResults().getAbsolutePath());
            return;
        }
        //compareWithRegressionFileContent(files[0], files[1], 0.0000000000000001);
        compareWithRegressionFileContent(files[0], files[1], deltaValueForDifferencesToIgnore, shouldAlsoDisplayDifferencesWhenValueIsMissing);
    }

    private void sortFilesWithLatestFirst(File[] files) {
        Arrays.sort(files, new Comparator<File>() {
            @Override
            public int compare(File o1, File o2) {
                long diff = o2.lastModified() - o1.lastModified();
                if(diff > 0) return 1;
                if(diff < 0) return -1;
                return 0;
            }
        });
    }

    class TestResult {
        private final CrsTransformationFacade facade;
        private final long totalNumberOfSecondsForAllTransformations;
        private final List<TestResultItem> testResultItems;

        TestResult(
            CrsTransformationFacade facade,
            long totalNumberOfSecondsForAllTransformations,
            List<TestResultItem> testResultItems
        ) {
            this.facade = facade;
            this.totalNumberOfSecondsForAllTransformations = totalNumberOfSecondsForAllTransformations;
            this.testResultItems = testResultItems;
        }
    }

    static class TestResultItem {
        private String wgs84sourceX , wgs84sourceY , epsgCrsCode;
        private String epsgTargetSourceX = "", epsgTargetSourceY = "", wgs84targetX = "", wgs84targetY = "";
        private final static String SEPARATOR = "|";

        public String getResultStringForRegressionFile() {
            return
                wgs84sourceX + SEPARATOR +
                wgs84sourceY + SEPARATOR +
                epsgCrsCode + SEPARATOR +
                epsgTargetSourceX + SEPARATOR +
                epsgTargetSourceY + SEPARATOR +
                wgs84targetX + SEPARATOR +
                wgs84targetY;
        }

        TestResultItem( // This constructor was added in a refactoring to be used later when comparing files ...
            String wgs84sourceX ,
            String wgs84sourceY ,
            String epsgCrsCode ,
            String epsgTargetSourceX ,
            String epsgTargetSourceY ,
            String wgs84targetX ,
            String wgs84targetY
        ) {
            this.wgs84sourceX = wgs84sourceX;
            this.wgs84sourceY = wgs84sourceY;
            this.epsgCrsCode = epsgCrsCode;
            this.epsgTargetSourceX = epsgTargetSourceX;
            this.epsgTargetSourceY = epsgTargetSourceY;
            this.wgs84targetX = wgs84targetX;
            this.wgs84targetY = wgs84targetY;
        }
        TestResultItem(String lineFromRow) {
            String[] array = lineFromRow.split("\\" + SEPARATOR);
            this.wgs84sourceX = array[0];
            this.wgs84sourceY = array[1];
            this.epsgCrsCode = array[2];
            if(array.length > 4) {
                this.epsgTargetSourceX = array[3];
                this.epsgTargetSourceY = array[4];
                if(array.length > 6) {
                    this.wgs84targetX = array[5];
                    this.wgs84targetY = array[6];
                }
            }
        }

        TestResultItem(
            EpsgCrsAndAreaCodeWithCoordinates item,
            Coordinate inputCoordinateWGS84,
            TransformResult resultOfTransformationFromWGS84,
            TransformResult resultOfTransformationBackToWGS84
        ) {
            wgs84sourceX = "" + item.centroidX;
            wgs84sourceY = "" + item.centroidY;
            epsgCrsCode = "" + item.epsgCrsCode;
            if (resultOfTransformationFromWGS84 != null && resultOfTransformationFromWGS84.isSuccess()) {
                final Coordinate outputCoordinate = resultOfTransformationFromWGS84.getOutputCoordinate();
                epsgTargetSourceX = "" + outputCoordinate.getXLongitude();
                epsgTargetSourceY = "" + outputCoordinate.getYLatitude();
            }
            if (resultOfTransformationBackToWGS84 != null && resultOfTransformationBackToWGS84.isSuccess()) {
                final Coordinate outputCoordinate = resultOfTransformationBackToWGS84.getOutputCoordinate();
                wgs84targetX = "" + outputCoordinate.getXLongitude();
                wgs84targetY = "" + outputCoordinate.getYLatitude();
            }
        }

        public Coordinate getInputCoordinateWGS84() {
            double lat = Double.parseDouble(wgs84sourceY);
            double lon = Double.parseDouble(wgs84sourceX);
            return Coordinate.latLon(lat, lon);
        }

        public boolean isSuccessfulTransformationFromWGS84() {
            if(epsgTargetSourceX == null || epsgTargetSourceY== null) return false;
            if(epsgTargetSourceX.isEmpty() || epsgTargetSourceY.isEmpty()) return false;
            // TODO: to improve this we should also verify that the values are doubles
            return true;
        }

        public boolean isSuccessfulTransformationBackToWGS84() {
            if(wgs84targetX == null || wgs84targetY == null) return false;
            if(wgs84targetX.isEmpty() || wgs84targetY.isEmpty()) return false;
            // TODO: to improve this we should also verify that the values are doubles
            return true;
        }

        public Coordinate getCoordinateOutputTransformationBackToWGS84() {
            if(!isSuccessfulTransformationBackToWGS84()) {
                return null;
            }
            // TODO: to improve this we should also verify that the values are doubles
            // i.e. exception might be thrown below
            double lat = Double.parseDouble(wgs84targetY);
            double lon = Double.parseDouble(wgs84targetX);
            return Coordinate.latLon(lat, lon);
        }

        public enum DIFFERENCE {
            EXISTING_VS_NOT_EXISTING,
            SIGNIFICANT_VALUE_DIFFERENCE,
            NO
        }
        /**
         * @param that
         * @param deltaValueForDifferencesToIgnore
         * @return
         */
        public DIFFERENCE isDeltaDifferenceSignificant(
            TestResultItem that,
            double deltaValueForDifferencesToIgnore
        ) {
            boolean thisXIsDouble = isValueExistingAndDouble(this.wgs84targetX);
            boolean thisYIsDouble = isValueExistingAndDouble(this.wgs84targetY);
            boolean thatXIsDouble = isValueExistingAndDouble(that.wgs84targetX);
            boolean thatYIsDouble = isValueExistingAndDouble(that.wgs84targetY);
            if(thisXIsDouble != thatXIsDouble) {
                return DIFFERENCE.EXISTING_VS_NOT_EXISTING;
            }
            if(thisYIsDouble != thatYIsDouble) {
                return DIFFERENCE.EXISTING_VS_NOT_EXISTING;
            }
            if(thisYIsDouble && thisXIsDouble) { // then the others are also double according to above
                double thisLat = Double.parseDouble(this.wgs84targetY);
                double thisLon = Double.parseDouble(this.wgs84targetX);

                double thatLat = Double.parseDouble(that.wgs84targetY);
                double thatLon = Double.parseDouble(that.wgs84targetX);

                double diffLat = Math.abs(thisLat - thatLat);
                double diffLon = Math.abs(thisLon - thatLon);

//                System.out.println("diffLat " + diffLat);
//                System.out.println("diffLon " + diffLon);
//                System.out.println("thisLon " + thisLon);
//                System.out.println("thatLon " + thatLon);

                if(diffLon > deltaValueForDifferencesToIgnore || diffLat > deltaValueForDifferencesToIgnore) {
                    return DIFFERENCE.SIGNIFICANT_VALUE_DIFFERENCE;
                }
            }
            return DIFFERENCE.NO;
        }

        private boolean isValueExistingAndDouble(String value) {
            if (value == null) return false;
            if (value.isEmpty()) return false;
            // TODO improve the code below, maybe with a regular expression instead,
            // see the documentation of 'Double.valueOf(String)'
            try {
                Double.parseDouble(value);
                return true;
            }
            catch(Exception e) {
                return false;
            }
        }
    }

    static class EpsgCrsAndAreaCodeWithCoordinates {
        final int epsgCrsCode;
        final int epsgAreaCode;
        final String epsgAreaName;
        final double centroidX;
        final double centroidY;

        private final static String SEPARATOR = "_";

        EpsgCrsAndAreaCodeWithCoordinates(
            int epsgCrsCode,
            int epsgAreaCode,
            String epsgAreaName,
            double centroidX,
            double centroidY
        ) {
            this.epsgCrsCode = epsgCrsCode;
            this.epsgAreaCode = epsgAreaCode;
            this.epsgAreaName = epsgAreaName;
            this.centroidX = centroidX;
            this.centroidY = centroidY;
        }

        @Override
        public String toString() {
            return
                "EpsgCrsAndAreaCodeWithCoordinates{" +
                "epsgAreaCode=" + epsgAreaCode +
                ", epsgCrsCode=" + epsgCrsCode +
                ", epsgAreaName='" + epsgAreaName + '\'' +
                ", centroidX=" + centroidX +
                ", centroidY=" + centroidY +
                '}';
        }
    }
}
//// The test results below were created when the following delta value was used:
//double DELTA_LIMIT_FOR_SUCCESS = 0.0001;
//-------------------------------
//testResults for CrsTransformationFacadeGooberCTL
//seconds: 2
//countOfSuccess: 19
//countOfFailures: 6416
//-------------------------------
//testResults for CrsTransformationFacadeProj4J
//seconds: 222
//countOfSuccess: 3916
//countOfFailures: 2519
//-------------------------------
//testResults for CrsTransformationFacadeOrbisgisCTS
//seconds: 455
//countOfSuccess: 3799
//countOfFailures: 2636
//-------------------------------
//testResults for CrsTransformationFacadeGeoTools
//seconds: 210
//countOfSuccess: 4036
//countOfFailures: 2399
//-------------------------------
//testResults for CrsTransformationFacadeGeoPackageNGA
//seconds: 249
//countOfSuccess: 3918
//countOfFailures: 2517
//-------------------------------
