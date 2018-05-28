package com.programmerare.crsCodeGeneration

import freemarker.template.Configuration
import freemarker.template.TemplateExceptionHandler
import java.io.File;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.query
import org.springframework.jdbc.datasource.DriverManagerDataSource;
import java.sql.Driver
import java.io.FileOutputStream
import java.io.OutputStreamWriter
import java.nio.charset.Charset


// build.gradle:
//project(':crsCodeGeneration') {
//    // command line execution
//    // gradle execute
//    // or, if somethign else than the default/hardcoded class name should be used:
//    // gradle execute -PmainClass=com.programmerare.crsCodeGeneration.ConstantClassGenerator
//    task execute(type:JavaExec) {
//        main = project.hasProperty("mainClass") ? project.getProperty("mainClass") : "com.programmerare.crsCodeGeneration.ConstantClassGenerator"
//        classpath = sourceSets.main.runtimeClasspath
//    }
//
//    dependencies {
//        // https://mvnrepository.com/artifact/org.freemarker/freemarker
//        implementation("org.freemarker:freemarker:2.3.28")
//    }
//}
class ConstantClassGenerator {

    private val freemarkerConfiguration: Configuration
    private val jdbcTemplate: JdbcTemplate
    private val rootDirectoryForCodeGenerationModule: File
    private val rootDirectoryWhereTheJavaFilesShouldBeGenerated: File

    init {
        rootDirectoryForCodeGenerationModule = getRootDirectoryForCodeGenerationModule()
        val rootDirectoryForConstantsModule = getRootDirectoryForConstantsModule(rootDirectoryForCodeGenerationModule)
        rootDirectoryWhereTheJavaFilesShouldBeGenerated = getRootDirectoryWhereTheJavaFilesShouldBeGenerated(rootDirectoryForConstantsModule)

        freemarkerConfiguration = Configuration(Configuration.VERSION_2_3_28)
//        freemarkerConfiguration.setDirectoryForTemplateLoading(File(directoryForTemplates))
        freemarkerConfiguration.setClassForTemplateLoading(javaClass, directoryForTemplates)
        freemarkerConfiguration.setDefaultEncoding(ENCODING_UTF_8)
        freemarkerConfiguration.setTemplateExceptionHandler(TemplateExceptionHandler.RETHROW_HANDLER)
        freemarkerConfiguration.setLogTemplateExceptions(false)
        freemarkerConfiguration.setWrapUncheckedExceptions(true)

        jdbcTemplate = getJdbcTemplate()
    }

    // returns a file object for the directory "crsCodeGeneration"
    fun getRootDirectoryForCodeGenerationModule(): File {
        val pathToRootDirectoryForClassFiles: String? = ConstantClassGenerator.javaClass.getResource("/").path
        // the path retrieved above is now assumed to be like this:
        // " .../crsTransformations/crsCodeGeneration/build/classes/kotlin/main/"
        val rootDirectoryForClassFiles = File(pathToRootDirectoryForClassFiles)
        val rootDirectoryForModule = rootDirectoryForClassFiles.parentFile.parentFile.parentFile.parentFile
        if(!rootDirectoryForModule.name.equals(NAME_OF_DIRECTORY_FOR_CODE_GENERATION)) {
            throw RuntimeException("Assumption about directory structure was not valid. Expected 4 parent directories of the following dir to be named '$NAME_OF_DIRECTORY_FOR_CODE_GENERATION' : " + pathToRootDirectoryForClassFiles)
        }
        return rootDirectoryForModule
    }

    // The input file should be be something like:
    // "...crsTransformations\crsCodeGeneration"
    // and the output file should then become
    // "...crsTransformations\crsConstants"
    private fun getRootDirectoryForConstantsModule(rootDirectoryForCodeGenerationModule: File): File {
        if(!rootDirectoryForCodeGenerationModule.name.equals(NAME_OF_DIRECTORY_FOR_CODE_GENERATION)) {
            throw RuntimeException("Unexpected directory name. $NAME_OF_DIRECTORY_FOR_CODE_GENERATION was expected but it was: " + rootDirectoryForCodeGenerationModule.name)
        }
        val constantsDirectory = File(rootDirectoryForCodeGenerationModule.parentFile, NAME_OF_DIRECTORY_FOR_CONSTANTS)
        if(!constantsDirectory.exists()) {
            throw RuntimeException("The directory does not exist: " + constantsDirectory.absolutePath)
        }
        return constantsDirectory
    }


    // The input file should be be something like:
    // "...crsTransformations\crsConstants"
    // and the output file should then be
    // "...crsTransformations\crsConstants\src\main\java"
    private fun getRootDirectoryWhereTheJavaFilesShouldBeGenerated(rootDirectoryForConstantsModule: File): File {
        val subDirectory: File = rootDirectoryForConstantsModule.resolve(RELATIVE_PATH_TO_JAVA_FILES)
        if(!subDirectory.exists()) {
            throw RuntimeException("The file does not exist: " + subDirectory.absolutePath)
        }
        return subDirectory
    }

    // e.g. epsgVersion = ""
    fun generateConstants(epsgVersion: String) {
        // TODO: generate classes with constants e.g. based on database with EPSG codes:
        // http://www.epsg.org/EPSGDataset/DownloadDataset.aspx
        val sqlQueryBase = " SELECT [Area].[AREA_NAME], [Coordinate Reference System].[AREA_OF_USE_CODE], [Coordinate Reference System].[COORD_REF_SYS_CODE], [Coordinate Reference System].[COORD_REF_SYS_NAME] FROM [Coordinate Reference System] , [Area] WHERE [Coordinate Reference System].[AREA_OF_USE_CODE] = [Area].[AREA_CODE] "

        val sqlQueryCondition = " AND [Area].[AREA_NAME] LIKE ? "
        // val sqlQueryOrderClause = " ORDER BY [Area].[AREA_NAME] , [Coordinate Reference System].[COORD_REF_SYS_NAME] "
        // val sqlQuery = sqlQueryBase + sqlQueryCondition + sqlQueryOrderClause
//        val sqlQuery = sqlQueryBase + sqlQueryCondition
//        val sqlQuery = sqlQueryBase + sqlQueryOrderClause
        val sqlQuery = sqlQueryBase
        val fieldSeparator = "__"

        val constantNameRenderer = ConstantNameRenderer(RenderStrategyNameAreaNumberInteger())

        val nameOfConstants = mutableListOf<ConstantTypeNameValue>()
        jdbcTemplate.query(sqlQuery) { rs, _ ->
//        jdbcTemplate.query(sqlQuery, "%Sweden%") { rs, _ ->
            val epsgNumber = rs.getInt("COORD_REF_SYS_CODE")
            val areaName = rs.getString("AREA_NAME")
            val crsName = rs.getString("COORD_REF_SYS_NAME")

            val nameForConstant1 = "_" + epsgNumber + fieldSeparator + areaName + fieldSeparator + crsName
            val nameForConstant2 = "_" + epsgNumber + fieldSeparator + crsName + fieldSeparator + areaName
            val nameForConstant3 = areaName + fieldSeparator + crsName + fieldSeparator + epsgNumber
            val nameForConstant4 = areaName + fieldSeparator + epsgNumber + fieldSeparator + crsName
            val nameForConstant5 = crsName + fieldSeparator + areaName + fieldSeparator + epsgNumber
            val nameForConstant6 = crsName + fieldSeparator + epsgNumber + fieldSeparator + areaName

            nameOfConstants.add(
                ConstantTypeNameValue(
//                    "EPSG:" + epsgCrsCode + " , " + crsName + " , " + areaName,
//                    DataType.INTEGER,
//                    nameForConstant5.adjusted(),
//                    epsgCrsCode.toString()
                    constantNameRenderer,
                    epsgNumber,
                    areaName,
                    crsName
            ))
//            println("nameForConstant1 " + nameForConstant1.adjusted())
//            println("nameForConstant2 " + nameForConstant2.adjusted())
//            println("nameForConstant3 " + nameForConstant3.adjusted())
//            println("nameForConstant4 " + nameForConstant4.adjusted())
//            println("nameForConstant5 " + nameForConstant5.adjusted())
//            println("nameForConstant6 " + nameForConstant6.adjusted())
//            nameForConstant1 _3006__SWEDEN__SWEREF99_TM
//            nameForConstant2 _3006__SWEREF99_TM__SWEDEN
//            nameForConstant3 SWEDEN__SWEREF99_TM__3006
//            nameForConstant4 SWEDEN__3006__SWEREF99_TM
//            nameForConstant5 SWEREF99_TM__SWEDEN__3006
//            nameForConstant6 SWEREF99_TM__3006__SWEDEN

        }

        val packageNamePrefix = "com.programmerare.crsConstants."
        val packageNameSuffix = "." + epsgVersion

        val nameOfJavaClassForIntegerConstants = "EpsgNumber"
        val nameOfJavaClassForStringConstants = "EpsgCode"
        var nameOfJavaPackage = ""

        nameOfJavaPackage = packageNamePrefix + "constantsByNameAreaNumber" + packageNameSuffix
        generateFile(nameOfJavaClassForIntegerConstants, nameOfJavaPackage, RenderStrategyNameAreaNumberInteger(), constantNameRenderer, nameOfConstants)
        generateFile(nameOfJavaClassForStringConstants, nameOfJavaPackage, RenderStrategyNameAreaNumberString(), constantNameRenderer, nameOfConstants)

        nameOfJavaPackage = packageNamePrefix + "constantsByNameNumberArea" + packageNameSuffix
        generateFile(nameOfJavaClassForIntegerConstants, nameOfJavaPackage, RenderStrategyNameNumberAreaInteger(), constantNameRenderer, nameOfConstants)
        generateFile(nameOfJavaClassForStringConstants, nameOfJavaPackage, RenderStrategyNameNumberAreaString(), constantNameRenderer, nameOfConstants)

        nameOfJavaPackage = packageNamePrefix + "constantsByAreaNumberName" + packageNameSuffix
        generateFile(nameOfJavaClassForIntegerConstants, nameOfJavaPackage, RenderStrategyAreaNumberNameInteger(), constantNameRenderer, nameOfConstants)
        generateFile(nameOfJavaClassForStringConstants, nameOfJavaPackage, RenderStrategyAreaNumberNameString(), constantNameRenderer, nameOfConstants)

        nameOfJavaPackage = packageNamePrefix + "constantsByAreaNameNumber" + packageNameSuffix
        generateFile(nameOfJavaClassForIntegerConstants, nameOfJavaPackage, RenderStrategyAreaNameNumberInteger(), constantNameRenderer, nameOfConstants)
        generateFile(nameOfJavaClassForStringConstants, nameOfJavaPackage, RenderStrategyAreaNameNumberString(), constantNameRenderer, nameOfConstants)

        nameOfJavaPackage = packageNamePrefix + "constantsByNumberAreaName" + packageNameSuffix
        generateFile(nameOfJavaClassForIntegerConstants, nameOfJavaPackage, RenderStrategyNumberAreaNameInteger(), constantNameRenderer, nameOfConstants, sortByNumber = true)
        generateFile(nameOfJavaClassForStringConstants, nameOfJavaPackage, RenderStrategyNumberAreaNameString(), constantNameRenderer, nameOfConstants, sortByNumber = true)

        nameOfJavaPackage = packageNamePrefix + "constantsByNumberNameArea" + packageNameSuffix
        generateFile(nameOfJavaClassForIntegerConstants, nameOfJavaPackage, RenderStrategyNumberNameAreaInteger(), constantNameRenderer, nameOfConstants, sortByNumber = true)
        generateFile(nameOfJavaClassForStringConstants, nameOfJavaPackage, RenderStrategyNumberNameAreaString(), constantNameRenderer, nameOfConstants, sortByNumber = true)
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
        var javaFileToBecomeCreated = getJavaFileToBecomeCreated(nameOfJavaClass, nameOfJavaPackage)
        val constantsSortedByName: List<ConstantTypeNameValue> =
            if(sortByNumber)
                nameOfConstants.sortedBy { it.epsgNumber }
            else
                nameOfConstants.sortedBy { it.getNameForConstant() }
        val constantsInformation = ConstantsInformation(nameOfJavaClass, nameOfJavaPackage, constants = constantsSortedByName)
        generateJavaFileWithConstants(javaFileToBecomeCreated, constantsInformation, "Constants.ftlh")
    }

    private fun getJavaFileToBecomeCreated(nameOfClassToBeGenerated: String, nameOfPackageToBeGenerated: String): File {
//        val nameOfClassToBeGenerated = "EpsgNumber"
//        val nameOfPackageToBeGenerated = "com.programmerare.crsConstants"
        val fullClassName = nameOfPackageToBeGenerated + "." + nameOfClassToBeGenerated // "com.programmerare.crsConstants.EpsgNumber"
        val relativePathToJavaFile = fullClassName.replace('.', '/') + FILE_EXTENSION_FOR_JAVA_FILE // "com/programmerare/crsConstants/EpsgNumber.java"
        val javaFileToBecomeCreated = rootDirectoryWhereTheJavaFilesShouldBeGenerated.resolve(relativePathToJavaFile)
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
        if(!dir.isDirectory) {
            throw RuntimeException("Not directory: " + dir.absolutePath)
        }
        return javaFileToBecomeCreated
    }

    private val FREEMARKER_PROPERTY_NAME_OF_CONSTANTS = "constants"
    private val FREEMARKER_PROPERTY_NAME_OF_JAVA_CLASS = "nameOfJavaClass"
    private val FREEMARKER_PROPERTY_NAME_OF_JAVA_PACKAGE = "nameOfJavaPackage"

    private fun generateJavaFileWithConstants(
        javaFileToBecomeCreated: File,
        constantsInformation: ConstantsInformation,
        nameOfTemplate: String
    ) {
        val template = freemarkerConfiguration.getTemplate(nameOfTemplate)
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

    fun getJdbcTemplate(): JdbcTemplate { // TODO: refactor reusage instead of letting this method be public here ...
        val file: File = getAccessDatabaseFileWithEpsgData()
        val connectionString = "jdbc:ucanaccess://" + file.absolutePath + ";memory=false"// jdbc:ucanaccess://c:/data/mydb.mdb;memory=false
        val driverManagerDataSource = DriverManagerDataSource(connectionString)
        return JdbcTemplate(driverManagerDataSource)
    }

    private fun getAccessDatabaseFileWithEpsgData(): File {
        val file = getAbsoluteFileByPathRelativeFromWorkingDirectory(relativePathToAccessDatabaseFile)
        if(!file.exists()) {
            throw RuntimeException("Access database file does not exist: " + file.absolutePath);
        }
        return file
    }

    private fun getAbsoluteFileByPathRelativeFromWorkingDirectory(relativePath: String): File {
        val resolvedFile = rootDirectoryForCodeGenerationModule.resolve(relativePath)
        return resolvedFile
    }

    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            verifyJdbcDriver()
            val constantClassGenerator = ConstantClassGenerator()
            constantClassGenerator.generateConstants(epsgVersion)
        }

        private fun verifyJdbcDriver() {
            var driver: Driver? = null
            var throwable: Throwable? = null
            try {
                driver = net.ucanaccess.jdbc.UcanaccessDriver()
            }
            catch (e: Throwable) {
                throwable = e
            }
            if(driver == null) {
                val message = "Problem with the database driver '$nameOfJdbcDriver'"
                if(throwable == null) {
                    throw RuntimeException(message)
                }
                else {
                    throw RuntimeException(message, throwable)
                }
            }
        }

        private val FILE_EXTENSION_FOR_JAVA_FILE = ".java"

        private val NAME_OF_DIRECTORY_FOR_CODE_GENERATION = "crsCodeGeneration"
        private val NAME_OF_DIRECTORY_FOR_CONSTANTS = "crsConstants"

        private val RELATIVE_PATH_TO_JAVA_FILES = "src/main/java"

        val ENCODING_UTF_8 = "UTF-8"

        private val nameOfJdbcDriver = "net.ucanaccess.jdbc.UcanaccessDriver"

        val directoryForTemplates = "/freemarker_templates" // means the directory ".../src/main/resources/freemarker_templates"

        private val epsgVersion = "v9_3" // TODO: maybe iterate the file system to extract version names from the directory names


        // Note that when the path below is changed: The path is mentioned in gitignore
        private val relativePathToAccessDatabaseFile: String //  = "data_files/EPSG_v9_3/EPSG_v9_3.mdb"

        init {
            //relativePathToAccessDatabaseFile = "data_files/EPSG_v9_3/EPSG_v9_3.mdb"
            relativePathToAccessDatabaseFile = "data_files/EPSG_" + epsgVersion + "/EPSG_" + epsgVersion + ".mdb"
        }
    }
}

fun String.adjusted(): String {
    return this.getUppercasedWithOnylValidCharacters()
}

fun String.getUppercasedWithOnylValidCharacters(): String {
    // the last regexp below just makes sure there are will not be more than two "_" in a row
    return this.toUpperCase().replace("[^a-zA-Z0-9_]".toRegex(), "_").replace("_{2,}".toRegex(), "__")
}