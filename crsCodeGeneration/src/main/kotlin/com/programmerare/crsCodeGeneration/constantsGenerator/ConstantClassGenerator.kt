package com.programmerare.crsCodeGeneration.constantsGenerator

import com.programmerare.crsCodeGeneration.CodeGeneratorBase
import java.io.File;
import java.io.FileOutputStream
import java.io.OutputStreamWriter
import java.nio.charset.Charset

// command line execution (according to configuration in build.gradle):
// gradle generateClassesWithEpsgConstants
class ConstantClassGenerator : CodeGeneratorBase() {

    // Generates classes with constants based on database with EPSG codes:
    // http://www.epsg.org/EPSGDataset/DownloadDataset.aspx
    fun generateConstants(epsgVersion: String) {
        val sqlQueryBase = SQL_STATEMENT_SELECTING_CRSCODE_CRSNAME_AREANAME
        val sqlQueryCondition = " AND [Area].[AREA_NAME] LIKE ? "
        // val sqlQueryOrderClause = " ORDER BY [Area].[AREA_NAME] , [Coordinate Reference System].[COORD_REF_SYS_NAME] "
        // val sqlQuery = sqlQueryBase + sqlQueryCondition
        val sqlQuery = sqlQueryBase

        val constantNameRenderer = ConstantNameRenderer(RenderStrategyNameAreaNumberInteger())
        val nameOfConstants = mutableListOf<ConstantTypeNameValue>()

        getJdbcTemplate().query(sqlQuery) { rs, _ ->
            // jdbcTemplate.query(sqlQuery, "%Sweden%") { rs, _ ->
            val epsgNumber = rs.getInt(SQL_COLUMN_CRSCODE)
            val areaName = rs.getString(SQL_COLUMN_AREANAME)
            val crsName = rs.getString(SQL_COLUMN_CRSNAME)

            nameOfConstants.add(
                ConstantTypeNameValue(
                    constantNameRenderer,
                    epsgNumber,
                    areaName,
                    crsName
                )
            )
        }

        var nameOfJavaPackage = ""

        // Generate Totally 12 classes below in 6 packages with 2 classes per package:
        generateFile(CLASS_NAME_INTEGER_CONSTANTS, PACKAGE_NameAreaNumber, RenderStrategyNameAreaNumberInteger(), constantNameRenderer, nameOfConstants)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  PACKAGE_NameAreaNumber, RenderStrategyNameAreaNumberString(),  constantNameRenderer, nameOfConstants)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, PACKAGE_NameNumberArea, RenderStrategyNameNumberAreaInteger(), constantNameRenderer, nameOfConstants)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  PACKAGE_NameNumberArea, RenderStrategyNameNumberAreaString(),  constantNameRenderer, nameOfConstants)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, PACKAGE_AreaNumberName, RenderStrategyAreaNumberNameInteger(), constantNameRenderer, nameOfConstants)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  PACKAGE_AreaNumberName, RenderStrategyAreaNumberNameString(),  constantNameRenderer, nameOfConstants)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, PACKAGE_AreaNameNumber, RenderStrategyAreaNameNumberInteger(), constantNameRenderer, nameOfConstants)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  PACKAGE_AreaNameNumber, RenderStrategyAreaNameNumberString(),  constantNameRenderer, nameOfConstants)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, PACKAGE_NumberAreaName, RenderStrategyNumberAreaNameInteger(), constantNameRenderer, nameOfConstants, sortByNumber = true)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  PACKAGE_NumberAreaName, RenderStrategyNumberAreaNameString(),  constantNameRenderer, nameOfConstants, sortByNumber = true)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, PACKAGE_NumberNameArea, RenderStrategyNumberNameAreaInteger(), constantNameRenderer, nameOfConstants, sortByNumber = true)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  PACKAGE_NumberNameArea, RenderStrategyNumberNameAreaString(),  constantNameRenderer, nameOfConstants, sortByNumber = true)
    }

    private fun generateFile(
        nameOfJavaClass: String,
        nameOfJavaPackage: String,
        renderStrategy: RenderStrategy,
        constantNameRenderer: ConstantNameRenderer,
        nameOfConstants: MutableList<ConstantTypeNameValue>,
        sortByNumber: Boolean = false
    ) {
        constantNameRenderer.renderStrategy = renderStrategy
        val directoryWhereTheJavaFilesShouldBeGenerated = getFileOrDirectory(NAME_OF_MODULE_DIRECTORY_FOR_CONSTANTS, RELATIVE_PATH_TO_JAVA_FILES)
        var javaFileToBecomeCreated = getJavaFileToBecomeCreated(nameOfJavaClass, nameOfJavaPackage, directoryWhereTheJavaFilesShouldBeGenerated)
        val constantsSortedByName: List<ConstantTypeNameValue> =
            if(sortByNumber)
                nameOfConstants.sortedBy { it.epsgNumber }
            else
                nameOfConstants.sortedBy { it.getNameForConstant() }
        val constantsInformation = ConstantsInformation(nameOfJavaClass, nameOfJavaPackage, constants = constantsSortedByName)
        generateJavaFileWithConstants(javaFileToBecomeCreated, constantsInformation, NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CONSTANTS)
    }

    private fun getJavaFileToBecomeCreated(nameOfClassToBeGenerated: String, nameOfPackageToBeGenerated: String, directoryWhereTheJavaFilesShouldBeGenerated: File): File {
        val fullClassName = nameOfPackageToBeGenerated + "." + nameOfClassToBeGenerated // e.g. "com.programmerare.crsConstants.EpsgNumber"
        val relativePathToJavaFile = fullClassName.replace('.', '/') + FILE_EXTENSION_FOR_JAVA_FILE // "com/programmerare/crsConstants/EpsgNumber.java"
        val javaFileToBecomeCreated = directoryWhereTheJavaFilesShouldBeGenerated.resolve(relativePathToJavaFile)
        val dir = javaFileToBecomeCreated.parentFile
        if(!dir.exists()) {
            val result: Boolean = dir.mkdirs()
            if(result) {
                println("Created directory: " + dir.absolutePath)
            }
            else {
                throw RuntimeException("Directory does not exist and could not be created: " + dir.absolutePath)
            }
        }
        throwExceptionIfDirectoryDoesNotExist(dir)
        if(!dir.isDirectory) {
            throw RuntimeException("Not directory: " + dir.absolutePath)
        }
        return javaFileToBecomeCreated
    }

    private fun generateJavaFileWithConstants(
        javaFileToBecomeCreated: File,
        constantsInformation: ConstantsInformation,
        nameOfFreemarkerTemplate: String
    ) {
        val template = freemarkerConfiguration.getTemplate(nameOfFreemarkerTemplate)

        val root = HashMap<String, Any>()
        root.put(FREEMARKER_PROPERTY_NAME_OF_CONSTANTS, constantsInformation.constants)
        root.put(FREEMARKER_PROPERTY_NAME_OF_JAVA_PACKAGE, constantsInformation.nameOfJavaPackage);
        root.put(FREEMARKER_PROPERTY_NAME_OF_JAVA_CLASS, constantsInformation.nameOfJavaClass);

        val outputStreamWriterWithUTF8encoding = OutputStreamWriter(
            FileOutputStream(javaFileToBecomeCreated),
            Charset.forName(ENCODING_UTF_8).newEncoder()
        )
        template.process(root, outputStreamWriterWithUTF8encoding)
        outputStreamWriterWithUTF8encoding.close()
    }

    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            val constantClassGenerator = ConstantClassGenerator()
            constantClassGenerator.generateConstants(EPSG_VERSION)
        }

        private const val FREEMARKER_PROPERTY_NAME_OF_CONSTANTS = "constants"
        private const val FREEMARKER_PROPERTY_NAME_OF_JAVA_CLASS = "nameOfJavaClass"
        private const val FREEMARKER_PROPERTY_NAME_OF_JAVA_PACKAGE = "nameOfJavaPackage"

        private const val NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CONSTANTS = "Constants.ftlh"

        private const val CLASS_NAME_INTEGER_CONSTANTS = "EpsgNumber"
        private const val CLASS_NAME_STRING_CONSTANTS = "EpsgCode"

        private val PACKAGE_NAME_PREFIX = "com.programmerare.crsConstants."
        private val PACKAGE_NAME_SUFFIX = "." + EPSG_VERSION

        private val PACKAGE_NameAreaNumber = PACKAGE_NAME_PREFIX + "constantsByNameAreaNumber" + PACKAGE_NAME_SUFFIX
        private val PACKAGE_NameNumberArea = PACKAGE_NAME_PREFIX + "constantsByNameNumberArea" + PACKAGE_NAME_SUFFIX
        private val PACKAGE_AreaNumberName = PACKAGE_NAME_PREFIX + "constantsByAreaNumberName" + PACKAGE_NAME_SUFFIX
        private val PACKAGE_AreaNameNumber = PACKAGE_NAME_PREFIX + "constantsByAreaNameNumber" + PACKAGE_NAME_SUFFIX
        private val PACKAGE_NumberAreaName = PACKAGE_NAME_PREFIX + "constantsByNumberAreaName" + PACKAGE_NAME_SUFFIX
        private val PACKAGE_NumberNameArea = PACKAGE_NAME_PREFIX + "constantsByNumberNameArea" + PACKAGE_NAME_SUFFIX
   }
}