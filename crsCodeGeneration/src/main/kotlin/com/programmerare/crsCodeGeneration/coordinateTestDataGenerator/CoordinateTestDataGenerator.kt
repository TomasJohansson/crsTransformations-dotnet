package com.programmerare.crsCodeGeneration.coordinateTestDataGenerator

import com.programmerare.crsCodeGeneration.CodeGeneratorBase
import java.util.HashMap
import java.io.File

// command line execution (according to configuration in build.gradle):
// gradle generateCsvTestDataFromEpsgDatabaseAndShapefile

/**
 * Generates a CSV file with test data (regarding "CSV": actually pipe characters are the separator and not comma).
 *  (the current output directory for the CSV file is ./crsTransformationTest/src/test/resources/generated )
 * The output columns in the CSV files are as below:
 *      epsgCrsCode|epsgAreaCode|epsgAreaName|centroidX|centroidY
 *          (as defined in the Freemarker template file "CoordinateTestCsvData.ftlh")
 *      Example:
 *      3006|1225|Sweden|17.083659606206545|61.98770256318016
 *          epsgCrsCode - unique number defining a coordinate reference system
 *              (for example 3006 in the above example, https://epsg.io/3006 )
 *              No duplicates in the file but each crs code only once in the file.
 *          epsgAreaCode -the code for an area, e.g. "1225" is an area code for area Sweden" in the above example.
 *              The interpreation of the above row is that the epsg code 3006 is used within the area 1225.
 *              Many CRS codes (i.e. Coordinate Reference Systems) can be used within one area.
 *              For example, area code 1225 (Sweden) are using CRS 3006, 2400, 4124, ... as in the above and below examples.
 *                  2400|1225|Sweden|17.083659606206545|61.98770256318016
 *                  4124|1225|Sweden|17.083659606206545|61.98770256318016
 *              It should also be noted that areas are not always exactly one country.
 *              For example there are many area codes within Sweden as in the below examples:
 *                    3007|2833|Sweden - 12 00|12.146151472138385|58.46573396912418
 *                    3008|2834|Sweden - 13 30|13.470524334397624|58.58400993288184
 *                    3009|2835|Sweden - 15 00|14.977576047737374|58.78282472189397
 *                    ...
 *                    5846|2833|Sweden - 12 00|12.146151472138385|58.46573396912418
 *              One of the above CRS codes is "3007" which is the CRS "SWEREF99 12 00" ( https://epsg.io/3007 )
 *              and it has (almost) an area code of its own (2833) , but not quite since
 *              EPSG has also defined the CRS code 5846 to be associated with area code 2833.
 *          epsgAreaName - the name associated with the area code e.g. "Sweden" or "Sweden - 12 00" as in above examples.
 *          centroidX - the "X" (longitude) coordinate for the center of the area (defined by the area code)
 *          centroidY - one "Y" (latitude) coordinate for the center of the area (defined by the area code)
 *              Both X and Y above are defined as longitude/latitude in the global CRS 4326 (WGS84 or "GPS" coordinates)
 *
 * When the above described CSV file is being created, the data in the three first columns of each row
 * is retrieved with an SQL JOIN between two tables in an MariaDB/MySql database,
 * and then the remaining two columns with the coordinates are retrieved from a shapefile.
 * The coordinates in the last two columns of each row in the output file are created by extracting (from the shapefile)
 * a polygon associated with an area code (also existing in the database) and then the
 * library GeoTools is used for finding the centroid coordinate from that polygon.
 */
class CoordinateTestDataGenerator : CodeGeneratorBase() {

    private fun getCsvFileToBecomeCreated(): File {
        val csvFile = super.getFileOrDirectory(
            NAME_OF_MODULE_DIRECTORY_FOR_TESTS,
            RELATIVE_PATH_TO_CSV_FILE_WITH_TESTDATA_TO_BECOME_GENERATED,
            throwExceptionIfNotExisting = false
        )
        throwExceptionIfDirectoryDoesNotExist(csvFile.parentFile)
        return csvFile
    }

    private fun generateTestData(pathToShapeFile: String) {
        val shapeFile = File(pathToShapeFile) // getEpsgShapeFile()
        if(!shapeFile.exists()) {
            throw RuntimeException("Shapefile does not exist: " + shapeFile.canonicalPath)
        }

        val attributeDataFromShapefile: List<RowOfAttributeTableAndCentroid> = getAttributeDataIncludingCentroidCoordinatesFromShapefile(shapeFile)
        val mapWithCentroidCoordinatesFromShapefileAndAreaCodeAsKey = mutableMapOf<Int, RowOfAttributeTableAndCentroid>()
        attributeDataFromShapefile.forEach { mapWithCentroidCoordinatesFromShapefileAndAreaCodeAsKey.put(it.AREA_CODE, it) }

        val listWithEpsgAndAreaCodesFromDatabase: List<EpsgCrsAndAreaCode> = getEpsgAndAreaCodesFromDatabase()

        val listOfDataToBeIncludedInGeneratedCsvFile = mutableListOf<EpsgCrsAndAreaCodeWithCoordinates>()

        listWithEpsgAndAreaCodesFromDatabase.forEach {
            val epsgAreaCode = it.epsgAreaCode
            val epsgCrsCode = it.epsgCrsCode
            val epsgAreaName = it.epsgAreaName
            if(!mapWithCentroidCoordinatesFromShapefileAndAreaCodeAsKey.containsKey(epsgAreaCode)) {
                println("missing areacode: " + epsgAreaCode)
            }
            else {
                val attr = mapWithCentroidCoordinatesFromShapefileAndAreaCodeAsKey.get(epsgAreaCode)!!
                val x = attr.centroidX
                val y = attr.centroidY
                listOfDataToBeIncludedInGeneratedCsvFile.add(EpsgCrsAndAreaCodeWithCoordinates(
                    epsgCrsCode.toString(),
                    epsgAreaCode.toString(),
                    epsgAreaName,
                    x.toString(),
                    y.toString()
                ))
            }
        }
        generateFile(listOfDataToBeIncludedInGeneratedCsvFile)
    }

    private fun generateFile(listOfDataToBeIncludedInGeneratedCsvFile: List<EpsgCrsAndAreaCodeWithCoordinates>) {
        val rootHashMapWithDataToBeUsedByFreemarkerTemplate = HashMap<String, Any>()
        rootHashMapWithDataToBeUsedByFreemarkerTemplate.put(FREEMARKER_PROPERTY_NAME_OF_LIST_WITH_TEST_DATA, listOfDataToBeIncludedInGeneratedCsvFile)

        val csvFileToBecomeCreated = getCsvFileToBecomeCreated()
        println("csvFileToBecomeCreated: " + csvFileToBecomeCreated.canonicalPath)

        super.createFile(
            NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CSV_TESTDATA,
            rootHashMapWithDataToBeUsedByFreemarkerTemplate,
            csvFileToBecomeCreated
        )
    }

    // SELECT MAX(COORD_REF_SYS_CODE) FROM  [Coordinate Reference System] // 69036405
    // SELECT COUNT(*) FROM [Coordinate Reference System] // 6583
    // SELECT COUNT(*) FROM [Area] // 3491
    // SELECT COUNT(*) FROM [Coordinate Reference System] , [Area] WHERE [Coordinate Reference System].[AREA_OF_USE_CODE] = [Area].[AREA_CODE] // 6583
    private fun getEpsgAndAreaCodesFromDatabase(): List<EpsgCrsAndAreaCode> {
        val jdbcTemplate = getJdbcTemplate()
        val list = mutableListOf<EpsgCrsAndAreaCode>()
        jdbcTemplate.query(SQL_STATEMENT_SELECTING_CRSCODE_CRSNAME_AREANAME) { rs, _ ->
            val epsgCrsCode = rs.getInt(SQL_COLUMN_CRSCODE)
            val epsgAreaName = rs.getString(SQL_COLUMN_AREANAME)
            val epsgAreaCode = rs.getInt(SQL_COLUMN_AREACODE)
            list.add(EpsgCrsAndAreaCode(epsgCrsCode, epsgAreaCode, epsgAreaName))
        }
        return list
    }

    private fun getAttributeDataIncludingCentroidCoordinatesFromShapefile(shapeFile: File): List<RowOfAttributeTableAndCentroid> {
        val epsgShapeFileReader = EpsgShapeFileReader()
        val attributeDataFromShapefile: List<RowOfAttributeTableAndCentroid> = epsgShapeFileReader.extractAttributeDataFromShapefile(shapeFile.absolutePath)
        return attributeDataFromShapefile
    }

    companion object {
        @JvmStatic

        // Potential problem when you run the main method below:
        //      java.lang.IllegalAccessException: class org.geotools.resources.NIOUtilities$1 cannot access class jdk.internal.ref.Cleaner (in module java.base) because module java.base does not export jdk.internal.ref to unnamed module @4d49af10
        // Workaround solution: Add the following to VM options:
        //      --add-exports java.base/jdk.internal.ref=ALL-UNNAMED
        // Some more information about the above "add-exports":
        //      https://docs.oracle.com/javase/9/migrate/toc.htm


        /**
         * @param args as below:
         *      args[0] a full path to an EPSG shapefile such as the path to a file ...\crsCodeGeneration\data_files\EPSG_Polygons_Ver_9.2.1\EPSG_Polygons.shp
         *      args[1] name of the EPSG database
         *      args[2] name of database user with access to the above EPSG database
         *      args[3] password for the above database user
         */
        fun main(args: Array<String>) {
            if(args.size < 4) {
                println("You must provide four parameters: PathToShapeFile dbName dbUser dbPassword")
                return
            }

            setDatabaseInformationForMariaDbConnection(
                    databaseName = args[1],
                    databaseUserName = args[2],
                    databaseUserPassword = args[3]
            )
            val c = CoordinateTestDataGenerator()
            c.generateTestData(pathToShapeFile = args[0])
        }

        private val FREEMARKER_PROPERTY_NAME_OF_LIST_WITH_TEST_DATA = "testdata"

        private const val NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CSV_TESTDATA = "CoordinateTestCsvData.ftlh"

        private const val RELATIVE_PATH_TO_CSV_FILE_WITH_TESTDATA_TO_BECOME_GENERATED = "src/test/resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv"
    }
}

// Used as a resultset object i.e. each instance represents some row resulting from an SQL query
data class EpsgCrsAndAreaCode(
    val epsgCrsCode: Int,
    val epsgAreaCode: Int,
    val epsgAreaName: String
) {
}

// Instances of this class below are sent into freemaker template
// and the usages of string is a convenient way of
// avoiding commas insted of dots within double field, and to avoid
// spaces to become rendered in integers (as thousands separator)
data class EpsgCrsAndAreaCodeWithCoordinates(
    val epsgCrsCode: String,
    val epsgAreaCode: String,
    val epsgAreaName: String,
    val centroidX: String,
    val centroidY: String
) {
}