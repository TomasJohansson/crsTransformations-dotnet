package com.programmerare.crsCodeGeneration

import freemarker.template.Configuration
import freemarker.template.TemplateExceptionHandler
import org.springframework.jdbc.core.JdbcTemplate
import org.springframework.jdbc.datasource.DriverManagerDataSource
import java.io.File
import java.sql.Driver

// TODO: currently you need to hardcode the password (and database name and user name in the CONNECTION_STRING_EPSG_DATABASE_MARIADB )
// which should be fixed ...

abstract class CodeGeneratorBase {

    protected val freemarkerConfiguration: Configuration

    init {
        verifyJdbcDriver()

        freemarkerConfiguration = Configuration(Configuration.VERSION_2_3_28)
//        freemarkerConfiguration.setDirectoryForTemplateLoading(File(DIRECTORY_FOR_FREEMARKER_TEMPLATES))
        freemarkerConfiguration.setClassForTemplateLoading(javaClass, DIRECTORY_FOR_FREEMARKER_TEMPLATES)
        freemarkerConfiguration.setDefaultEncoding(ENCODING_UTF_8)
        freemarkerConfiguration.setTemplateExceptionHandler(TemplateExceptionHandler.RETHROW_HANDLER)
        freemarkerConfiguration.setLogTemplateExceptions(false)
        freemarkerConfiguration.setWrapUncheckedExceptions(true)
    }

    protected fun getJdbcTemplate(): JdbcTemplate {
        val driverManagerDataSource = DriverManagerDataSource(CONNECTION_STRING_EPSG_DATABASE_MARIADB)
        return JdbcTemplate(driverManagerDataSource)
    }

    private fun verifyJdbcDriver() {
        var driver: Driver? = null
        var throwable: Throwable? = null
        try {
            driver = org.mariadb.jdbc.Driver()
        }
        catch (e: Throwable) {
            throwable = e
        }
        if(driver == null) {
            val message = "Problem with the database driver '${JDBC_DRIVER_CLASS_NAME_MARIADB}'"
            if(throwable == null) {
                throw RuntimeException(message)
            }
            else {
                throw RuntimeException(message, throwable)
            }
        }
    }

    // returns a file object for the directory "crsCodeGeneration"
    private fun getDirectoryForCodeGenerationModule(): File {
        val pathToRootDirectoryForClassFiles: String? = CodeGeneratorBase.javaClass.getResource("/").path
        // the path retrieved above is now assumed to be like this:
        // " .../crsTransformations/crsCodeGeneration/build/classes/kotlin/main/"
        val rootDirectoryForClassFiles = File(pathToRootDirectoryForClassFiles)
        throwExceptionIfDirectoryDoesNotExist(rootDirectoryForClassFiles)
        val rootDirectoryForModule = rootDirectoryForClassFiles.parentFile.parentFile.parentFile.parentFile
        throwExceptionIfDirectoryDoesNotExist(rootDirectoryForModule)
        if(!rootDirectoryForModule.name.equals(NAME_OF_MODULE_DIRECTORY_FOR_CODE_GENERATION)) {
            throw RuntimeException("Assumption about directory structure was not valid. Expected 4 parent directories of the following dir to be named '${NAME_OF_MODULE_DIRECTORY_FOR_CODE_GENERATION}' : " + pathToRootDirectoryForClassFiles)
        }
        return rootDirectoryForModule
    }

    /**
     * @nameOfModuleDirectory should be e.g. NAME_OF_MODULE_DIRECTORY_FOR_CODE_GENERATION or NAME_OF_MODULE_DIRECTORY_FOR_CONSTANTS
     */
    protected fun getModuleDirectory(nameOfModuleDirectory: String): File {
        val codeGenerationDirectory = getDirectoryForCodeGenerationModule()
        val baseDirectoryWithAllModules = codeGenerationDirectory.parentFile
        val moduleDirectory = File(baseDirectoryWithAllModules, nameOfModuleDirectory)
        throwExceptionIfDirectoryDoesNotExist(moduleDirectory)
        return moduleDirectory
    }

    /**
     * @nameOfModuleDirectory should be e.g. NAME_OF_MODULE_DIRECTORY_FOR_CODE_GENERATION or NAME_OF_MODULE_DIRECTORY_FOR_CONSTANTS
     * @subpathToFileOrDirectoryRelativeToModuleDirectory should be e.g. RELATIVE_PATH_TO_JAVA_FILES
     */
    protected fun getFileOrDirectory(nameOfModuleDirectory: String, subpathToFileOrDirectoryRelativeToModuleDirectory: String, throwExceptionIfNotExisting: Boolean = true): File {
        val baseDir = getModuleDirectory(nameOfModuleDirectory)
        var directoryOrFile = baseDir.resolve(subpathToFileOrDirectoryRelativeToModuleDirectory)
        if(throwExceptionIfNotExisting) throwExceptionIfFileOrDirectoryDoesNotExist(directoryOrFile)
        return directoryOrFile
    }

    protected fun throwExceptionIfFileDoesNotExist(file: File, errorPrefix: String) {
        if(!file.exists()) {
            throw RuntimeException(errorPrefix + file.absolutePath)
        }
        if(!file.isFile()) {
            throw RuntimeException("The path is not a file: " + file.absolutePath)
        }
    }
    protected fun throwExceptionIfDirectoryDoesNotExist(dir: File) {
        if(!dir.exists()) {
            throw RuntimeException("directory does not exist: " + dir.absolutePath)
        }
        if(!dir.isDirectory()) {
            throw RuntimeException("file is not directory: " + dir.absolutePath)
        }
    }

    protected fun throwExceptionIfFileOrDirectoryDoesNotExist(dirOrFile: File) {
        if(!dirOrFile.exists()) {
            throw RuntimeException("File or directory does not exist: " + dirOrFile.absolutePath)
        }
    }

    companion object {
        // in the future maybe the row below will work:
        // protected const val EPSG_VERSION = "v9_3"
        // but currently not supported when trying to use from subclass.
        // therefore instead public (implicit when not protected/private) and "@JvmField" and without "const"
        @JvmField
        val EPSG_VERSION = "v9_3" // TODO: maybe iterate the file system to extract version names from the directory names

        // TODO: currently you need to hardcode the password (and database name and user name in the CONNECTION_STRING_EPSG_DATABASE_MARIADB )
        // which should be fixed ...
        @JvmField
        val CONNECTION_STRING_EPSG_DATABASE_MARIADB = "jdbc:mariadb://localhost:3306/epsg_version_9_3?user=tomas&password=mypassword"

        @JvmField
        val NAME_OF_MODULE_DIRECTORY_FOR_CODE_GENERATION = "crsCodeGeneration"

        @JvmField
        val NAME_OF_MODULE_DIRECTORY_FOR_CONSTANTS = "crsConstants"

        @JvmField
        val NAME_OF_MODULE_DIRECTORY_FOR_TESTS = "crsTransformationTest"

        @JvmField
        val FILE_EXTENSION_FOR_JAVA_FILE = ".java"

//        @JvmStatic
//        protected val FILE_EXTENSION_FOR_KOTLIN_FILE = ".kt"

        @JvmField
        val RELATIVE_PATH_TO_JAVA_FILES = "src/main/java"

//        @JvmStatic
//        protected val RELATIVE_PATH_TO_KOTLIN_FILES = "src/main/kotlin"

        @JvmField
        val ENCODING_UTF_8 = "UTF-8"

        @JvmField
        val JDBC_DRIVER_CLASS_NAME_MARIADB = "org.mariadb.jdbc.Driver"


        @JvmField
        val DIRECTORY_FOR_FREEMARKER_TEMPLATES = "/freemarker_templates" // means the directory ".../src/main/resources/freemarker_templates"

        // The SQL below works for the downloaded database MySQL/MariaDB database
        // (but had slightly different names in the previously used MS Access database)
        // for example the table names are using the prefix "epsg_" in the MySQL database but not in the MS Access database.
        @JvmField val SQL_STATEMENT_SELECTING_CRSCODE_CRSNAME_AREANAME = """
            SELECT
                area.area_name,
                crs.area_of_use_code,
                crs.coord_ref_sys_code,
                crs.coord_ref_sys_name
            FROM
                epsg_coordinatereferencesystem AS crs
                INNER JOIN
                epsg_area AS area
                    ON
                    crs.area_of_use_code = area.area_code
        """

        @JvmField val SQL_COLUMN_CRSCODE = "coord_ref_sys_code"
        @JvmField val SQL_COLUMN_CRSNAME = "coord_ref_sys_name"
        @JvmField val SQL_COLUMN_AREANAME = "area_name"
        @JvmField val SQL_COLUMN_AREACODE = "area_of_use_code"
    }
}