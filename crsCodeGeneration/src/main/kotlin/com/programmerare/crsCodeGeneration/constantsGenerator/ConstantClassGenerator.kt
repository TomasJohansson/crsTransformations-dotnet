package com.programmerare.crsCodeGeneration.constantsGenerator

import com.programmerare.crsCodeGeneration.CodeGeneratorBase
import java.io.File;
import java.io.FileOutputStream
import java.io.OutputStreamWriter
import java.nio.charset.Charset

// This class generates constants classes into subfolders of this folder:
// // .\crsConstants\src\main\java\com\programmerare\crsConstants

// command line execution (according to configuration in build.gradle):
// gradle generateClassesWithEpsgConstants

/**
 * TODO: below document the whole procedure of how to create new constants
 * including downloading zip file with EPSG information ...
 * (and creation of a new database ...)
 */
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
        generateFile(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForNameAreaNumber(), RenderStrategyNameAreaNumberInteger(), constantNameRenderer, nameOfConstants)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForNameAreaNumber(), RenderStrategyNameAreaNumberString(),  constantNameRenderer, nameOfConstants)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForNameNumberArea(), RenderStrategyNameNumberAreaInteger(), constantNameRenderer, nameOfConstants)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForNameNumberArea(), RenderStrategyNameNumberAreaString(),  constantNameRenderer, nameOfConstants)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForAreaNumberName(), RenderStrategyAreaNumberNameInteger(), constantNameRenderer, nameOfConstants)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForAreaNumberName(), RenderStrategyAreaNumberNameString(),  constantNameRenderer, nameOfConstants)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForAreaNameNumber(), RenderStrategyAreaNameNumberInteger(), constantNameRenderer, nameOfConstants)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForAreaNameNumber(), RenderStrategyAreaNameNumberString(),  constantNameRenderer, nameOfConstants)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForNumberAreaName(), RenderStrategyNumberAreaNameInteger(), constantNameRenderer, nameOfConstants, sortByNumber = true)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForNumberAreaName(), RenderStrategyNumberAreaNameString(),  constantNameRenderer, nameOfConstants, sortByNumber = true)

        generateFile(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForNumberNameArea(), RenderStrategyNumberNameAreaInteger(), constantNameRenderer, nameOfConstants, sortByNumber = true)
        generateFile(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForNumberNameArea(), RenderStrategyNumberNameAreaString(),  constantNameRenderer, nameOfConstants, sortByNumber = true)
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
            if (args.size < 4) {
                println("The method should have four parameters")
                // TODO validate not only the number of arguments but also with some regular expression
                // to only allow "v" as prefix and then numbers and underscores as in e.g. "v9_5_3"
                return
            }
            // EPSG_VERSION should be specified (with underscores instead of dots e.g. 'v9_5_3'"
            EPSG_VERSION = args[0]
            // TODO validate not only the number of arguments but also with some regular expression
            // to only allow "v" as prefix (for EPSG_VERSION) and then numbers and underscores as in e.g. "v9_5_3"

            CodeGeneratorBase.setDatabaseInformationForMariaDbConnection(
                    databaseName = args[1],
                    databaseUserName = args[2],
                    databaseUserPassword = args[3]
            )

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

        // TODO: improve this ugly hack below when doing refactoring from a hardcoded EPSG_VERSION
        // to instead using a parameter to the main method
        private var EPSG_VERSION = "v_NotYetDefined" // should instead be set to something like "EPSG_VERSION"

        private fun getPackageNameSuffix(): String {
            return "." + EPSG_VERSION
        }

        private fun getNameOfPackageForNameAreaNumber(): String {
            return PACKAGE_NAME_PREFIX + "constantsByNameAreaNumber" + getPackageNameSuffix()
        }
        private fun getNameOfPackageForNameNumberArea(): String {
            return PACKAGE_NAME_PREFIX + "constantsByNameNumberArea" + getPackageNameSuffix()
        }
        private fun getNameOfPackageForAreaNumberName(): String {
            return PACKAGE_NAME_PREFIX + "constantsByAreaNumberName" + getPackageNameSuffix()
        }
        private fun getNameOfPackageForAreaNameNumber(): String {
            return PACKAGE_NAME_PREFIX + "constantsByAreaNameNumber" + getPackageNameSuffix()
        }
        private fun getNameOfPackageForNumberAreaName(): String {
            return PACKAGE_NAME_PREFIX + "constantsByNumberAreaName" + getPackageNameSuffix()
        }
        private fun getNameOfPackageForNumberNameArea(): String {
            return PACKAGE_NAME_PREFIX + "constantsByNumberNameArea" + getPackageNameSuffix()
        }
   }
}