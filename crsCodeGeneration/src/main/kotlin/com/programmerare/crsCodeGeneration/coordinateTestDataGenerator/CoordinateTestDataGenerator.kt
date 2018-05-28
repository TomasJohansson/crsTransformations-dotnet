package com.programmerare.crsCodeGeneration.coordinateTestDataGenerator

import java.util.HashMap
import kotlin.collections.MutableMap
import com.programmerare.crsCodeGeneration.ConstantClassGenerator
import freemarker.template.Configuration
import freemarker.template.TemplateExceptionHandler
import org.springframework.jdbc.core.JdbcTemplate
import java.io.File
import java.io.FileOutputStream
import java.io.OutputStreamWriter
import java.nio.charset.Charset

class CoordinateTestDataGenerator {

    private val freemarkerConfiguration: Configuration
    private val constantClassGenerator: ConstantClassGenerator

    init {
        constantClassGenerator = ConstantClassGenerator()
        // TODO: put this stuff e.g. in a base class (it is currently duplicated in two code generation classes)
        freemarkerConfiguration = Configuration(Configuration.VERSION_2_3_28)
//        freemarkerConfiguration.setDirectoryForTemplateLoading(File(directoryForTemplates))
        freemarkerConfiguration.setClassForTemplateLoading(javaClass, ConstantClassGenerator.directoryForTemplates)
        freemarkerConfiguration.setDefaultEncoding(ConstantClassGenerator.ENCODING_UTF_8)
        freemarkerConfiguration.setTemplateExceptionHandler(TemplateExceptionHandler.RETHROW_HANDLER)
        freemarkerConfiguration.setLogTemplateExceptions(false)
        freemarkerConfiguration.setWrapUncheckedExceptions(true)
    }

    private fun getEpsgShapeFile(): File {
        val dir = constantClassGenerator.getRootDirectoryForCodeGenerationModule()
        val file = dir.resolve("../crsCodeGeneration/data_files/EPSG_Polygons_Ver_9.2.1/EPSG_Polygons.shp")
        if(!file.exists()) {
            throw RuntimeException("File does not exist: " + file.absolutePath)
        }
        return file
    }

    private fun getCsvFileToBecomeCreated(): File {
        val dir = constantClassGenerator.getRootDirectoryForCodeGenerationModule()
        val file = dir.resolve("../crsTransformationTest/src/test/resources/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv")
        if(!file.exists()) {
            throw RuntimeException("File does not exist: " + file.absolutePath)
        }
        return file
    }

    private fun getJdbcTemplate(): JdbcTemplate {
        var jdbcTemplate = constantClassGenerator.getJdbcTemplate()
        return jdbcTemplate
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

    private fun generateFile(listt: List<EpsgCrsAndAreaCodeWithCoordinates>) {
        val nameOfTemplate = "CoordinateTestCsvData.ftlh"
        val template = freemarkerConfiguration.getTemplate(nameOfTemplate)
        val root = HashMap<String, Any>()
        root.put(FREEMARKER_PROPERTY_NAME_OF_LIST_WITH_TEST_DATA, listt)

        val csvFileToBecomeCreated = getCsvFileToBecomeCreated()
        val outputStreamWriterWithUTF8encoding = OutputStreamWriter(
            FileOutputStream(csvFileToBecomeCreated),
            Charset.forName(ConstantClassGenerator.ENCODING_UTF_8).newEncoder()
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
        val sqlQuery = " SELECT [Area].[AREA_NAME], [Coordinate Reference System].[AREA_OF_USE_CODE], [Coordinate Reference System].[COORD_REF_SYS_CODE], [Coordinate Reference System].[COORD_REF_SYS_NAME] FROM [Coordinate Reference System] , [Area] WHERE [Coordinate Reference System].[AREA_OF_USE_CODE] = [Area].[AREA_CODE] "
        val map: MutableMap<String, Boolean> = HashMap<String, Boolean>()
        val list = mutableListOf<EpsgCrsAndAreaCode>()
        jdbcTemplate.query(sqlQuery) { rs, _ ->
            val epsgCrsCode = rs.getInt("COORD_REF_SYS_CODE")
            val epsgAreaName = rs.getString("AREA_NAME")
            val epsgAreaCode = rs.getInt("AREA_OF_USE_CODE")
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
    }

}

data class EpsgCrsAndAreaCode(
    val epsgCrsCode: Int,
    val epsgAreaCode: Int,
    val epsgAreaName: String
) {
}

// this class below is sent into freemaker template and usages of string is a convenient
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