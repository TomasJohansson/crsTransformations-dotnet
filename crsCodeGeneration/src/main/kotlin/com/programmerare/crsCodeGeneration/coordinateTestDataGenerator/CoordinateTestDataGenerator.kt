package com.programmerare.crsCodeGeneration.coordinateTestDataGenerator

import com.programmerare.crsCodeGeneration.CodeGeneratorBase
import java.util.HashMap
import java.io.File
import java.io.FileOutputStream
import java.io.OutputStreamWriter
import java.nio.charset.Charset

// command line execution (according to configuration in build.gradle):
// gradle generateCsvTestDataFromEpsgDatabaseAndShapefile

/**
 * Generates a CSV file with test data.
 * The data in each row is coming from two tables (throgh a SQL join) in an MS Access database
 * and indirectly from a shapefile.
 * The coordinates at each row are created throgh extracting a polygon associated with
 * an area code (also existing in the Access database used) and then GeoTools
 * is used for creating a centroid coordinate from the polygon.
 */
class CoordinateTestDataGenerator : CodeGeneratorBase() {

    private fun getEpsgShapeFile(): File {
        val shapefile = super.getFileOrDirectory(
            NAME_OF_MODULE_DIRECTORY_FOR_CODE_GENERATION,
            RELATIVE_PATH_TO_EPSG_SHAPEFILE,
            throwExceptionIfNotExisting = true
        )
        return shapefile
    }

    private fun getCsvFileToBecomeCreated(): File {
        val csvFile = super.getFileOrDirectory(
            NAME_OF_MODULE_DIRECTORY_FOR_TESTS,
            RELATIVE_PATH_TO_CSV_FILE_WITH_TESTDATA_TO_BECOME_GENERATED,
            throwExceptionIfNotExisting = false
        )
        throwExceptionIfDirectoryDoesNotExist(csvFile.parentFile)
        return csvFile
    }

    private fun generateTestData() {
        val pathToShapeFile = getEpsgShapeFile()
        val returnList = mutableListOf<EpsgCrsAndAreaCodeWithCoordinates>()
        val epsgShapeFileReader = EpsgShapeFileReader()
        val attributeDataFromShapefile: List<RowOfAttributeTableAndCentroid> = epsgShapeFileReader.extractAttributeDataFromShapefile(pathToShapeFile.absolutePath)
        val mapWithAreaCodeAsKey = mutableMapOf<Int, RowOfAttributeTableAndCentroid>()
        attributeDataFromShapefile.forEach { mapWithAreaCodeAsKey.put(it.AREA_CODE, it) }
        val list: List<EpsgCrsAndAreaCode> = getEpsgAndAreaCodes()
        list.forEach {
            val epsgAreaCode = it.epsgAreaCode
            val epsgCrsCode = it.epsgCrsCode
            val epsgAreaName = it.epsgAreaName
            if(!mapWithAreaCodeAsKey.containsKey(epsgAreaCode)) {
                println("missing areacode: " + epsgAreaCode)
            }
            else {
                val attr = mapWithAreaCodeAsKey.get(epsgAreaCode)!!
                val x = attr.centroidX
                val y = attr.centroidY
                returnList.add(EpsgCrsAndAreaCodeWithCoordinates(
                    epsgCrsCode.toString(),
                    epsgAreaCode.toString(),
                    epsgAreaName,
                    x.toString(),
                    y.toString()
                ))
            }
        }
        generateFile(returnList)
    }

    private fun generateFile(list: List<EpsgCrsAndAreaCodeWithCoordinates>) {
        val template = freemarkerConfiguration.getTemplate(NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CSV_TESTDATA)
        val root = HashMap<String, Any>()
        root.put(FREEMARKER_PROPERTY_NAME_OF_LIST_WITH_TEST_DATA, list)

        val csvFileToBecomeCreated = getCsvFileToBecomeCreated()
        val outputStreamWriterWithUTF8encoding = OutputStreamWriter(
            FileOutputStream(csvFileToBecomeCreated),
            Charset.forName(ENCODING_UTF_8).newEncoder()
        )
        template.process(root, outputStreamWriterWithUTF8encoding)
        outputStreamWriterWithUTF8encoding.close()
    }

    // SELECT MAX(COORD_REF_SYS_CODE) FROM  [Coordinate Reference System] // 69036405
    // SELECT COUNT(*) FROM [Coordinate Reference System] // 6583
    // SELECT COUNT(*) FROM [Area] // 3491
    // SELECT COUNT(*) FROM [Coordinate Reference System] , [Area] WHERE [Coordinate Reference System].[AREA_OF_USE_CODE] = [Area].[AREA_CODE] // 6583
    private fun getEpsgAndAreaCodes(): List<EpsgCrsAndAreaCode> {
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

    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            val c = CoordinateTestDataGenerator()
            c.generateTestData()
        }

        private val FREEMARKER_PROPERTY_NAME_OF_LIST_WITH_TEST_DATA = "testdata"

        private const val NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CSV_TESTDATA = "CoordinateTestCsvData.ftlh"

        private const val RELATIVE_PATH_TO_EPSG_SHAPEFILE = "data_files/EPSG_Polygons_Ver_9.2.1/EPSG_Polygons.shp"
        private const val RELATIVE_PATH_TO_CSV_FILE_WITH_TESTDATA_TO_BECOME_GENERATED = "src/test/resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv"
        //
    }
}

// used as a resultset object i.e. each instance represents some row resulting from an SQL query
data class EpsgCrsAndAreaCode(
    val epsgCrsCode: Int,
    val epsgAreaCode: Int,
    val epsgAreaName: String
) {
}

// instance of this class below are sent into freemaker template and usages of string is a convenient
// to avoid commas insted of dots within double field, and avoid
// spaces in integers (as thousands separator)
data class EpsgCrsAndAreaCodeWithCoordinates(
    val epsgCrsCode: String,
    val epsgAreaCode: String,
    val epsgAreaName: String,
    val centroidX: String,
    val centroidY: String
) {
}