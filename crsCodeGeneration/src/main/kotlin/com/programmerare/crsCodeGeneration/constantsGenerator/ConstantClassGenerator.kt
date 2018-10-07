package com.programmerare.crsCodeGeneration.constantsGenerator

import com.programmerare.crsCodeGeneration.CodeGeneratorBase
import java.io.File;

// This class generates constants classes into subfolders of this folder:
// // .\crsConstants\src\main\java\com\programmerare\crsConstants

// command line execution (according to configuration in build.gradle):
// gradle generateClassesWithEpsgConstants

/**
 * Below are step by step instructions for how to generate new constants when a new EPSG version is downloaded.
 * (i.e. constants within "crsConstants" e.g. the file ".\crsConstants\src\main\java\com\programmerare\crsConstants\constantsByAreaNameNumber\v9_3\EpsgCode.java")
 *
 * 1. Download the latest version of a MySQL/MariaDB file from one of these websites:
 *          http://www.epsg-registry.org
 *          http://www.epsg.org
 *      It may be necessary to register to be able to download the latest file.
 *      The name of the downloaded file might be something like this: "EPSG-MySQL-export-9.5.3.zip"
 * 2. Unzip the content of the downloaded file into some directory.
 *      In the description below it is assumed that you unzipped the content to the directory "C:\temp\EPSG\"
 *      There should now be at least two relevant SQL files in your unzip directory:
 *          "MySQL_Table_Script.sql" (create table statements)
 *          "MySQL_Data_Script.sql" (insert into statement)
 *       ( There may also be a file "MySQL_FKey_Script.sql" for creating foreign keys but it is not
 *          really necessary for the purposes here, i.e. use the database content
 *          for generating Java or Kotlin classes with constants )
 * 3. Verify the encoding to be UTF-8 and convert if necessary.
 *       There may be a readme textfile among the unzipped files which may claim
 *       that the encoding UTF-8 is used, but you should verify that yourself.
 *       For example, open the above (in the previos step) two mentioned ".sql"-files with Notepad++
 *       and then look at the Encoding menu option to see that the files are UTF8.
 *       If some of them would not be UTF8, then you should convert it to UTF-8
 *       to avoid later problems (with awkward characters)
 *       when reading the content from the file or inserting/reading from the database.
 * 4. Execute the script/program "ImprovingTheEpsgImportFileForMysqlByAddingTransactions"
 *          ( .\crsCodeGeneration\src\main\kotlin\com\programmerare\crsCodeGeneration\databaseRelatedPreprocessingUtilities\ImprovingTheEpsgImportFileForMysqlByAddingTransactions.kt )
 *          The input should be the file with insert statements and the resulting output
 *          will be a similar file (but with transactions) in the same directory.
 *        Example usage (with the path as input to the program)
 *        com.programmerare.crsCodeGeneration.databaseRelatedPreprocessingUtilities.ImprovingTheEpsgImportFileForMysqlByAddingTransactions C:\temp\EPSG\MySQL_Data_Script.sql
 *          The input for the above command was the following file:
 *              "C:\temp\EPSG\MySQL_Data_Script.sql"
 *          and the result should then be the following file: (i.e. a file with alsmost the same path/name but different suffix)
 *              "C:\temp\EPSG\MySQL_Data_Script_WithTransactions.sql"
 *          The purpose is to use the above created file to improve the performance for
 *          the insert statements by creating a new file which uses transactions.
 *          Some background information explaining the reason for the above creation of a new file:
 *              The file "MySQL_Table_Script.sql" (from the unzipped directory mentioned above)
 *              contains a long long list of insert statements, which make it very very slow.
 *              Therefore, use a script (a main method in a Kotlin class file)
 *              which reads all those statements from the file and inserts in a new file,
 *              but with the difference that in the new file, there will also be
 *              "start transaction" and "commit" statements at around each 50th insert statement.
 *              (currently 50 is used, but it might change later without remembering to mention it here)
 * 5. Create the MariaDB database to become populated with the above two files
 *      i.e. the file that creates the tables and then the file (modified with transaction)
 *      which inserts the data.
 *    Note that the creation of a database (and user rights) is currently not described here in detail.
 *      You may for example create the database through a GUI such as DBeaver.
 *      For the next step, it is simply assumed that you have created a database
 *      with the name "epsg_version_9_5_3"
 *      (and please note that dots should be avoided in the name of the database !)
 *      and the database user "myuser" has the password "mypassword"
 *      and has the rights to create tables and insert data to the tables.
 * 6. Create the tables with the file "MySQL_Table_Script.sql"
 *      (located in the unzip directory mentioned in a previous step above)
 *      The commands below have been tested with Windows 10 and an installation of MariaDB version 10.1.34.
 *      Commands from a Windows 10 command prompt (first navigating to the unzip directory):
 *          cd C:\temp\EPSG
 *          mysql --user=myuser --password=mypassword --default-character-set=utf8 epsg_version_9_5_3
 *      Depending on how your path has been set up you may have to use the full path to the mysql tool e.g. like below:
 *          "C:\Program Files\MariaDB 10.1\bin\mysql" --user=myuser --password=mypassword --default-character-set=utf8 epsg_version_9_5_3
 *      Then run the "source" command two times from within the "mysql" program:
 *          source MySQL_Table_Script.sql
 *          source MySQL_Data_Script_WithTransactions.sql
 * 7. Generate the constants with the main method in ConstantClassGenerator.
 *      The main method requries four parameters in the following order:
 *          version name (this string is used as last part of the package name) e.g. "v9_5_3"
 *          database name e.g. "epsg_version_9_5_3"
 *          database user name
 *          database user password
 *      Example of running the main method:
 *          com.programmerare.crsCodeGeneration.constantsGenerator.ConstantClassGenerator v9_5_3 dbName dbUser dbPassword
 *      The resulting output should 12 classes with constants generated into the module "crsConstants"
 *      within the following directory:
 *          .\crsConstants\src\main\java\com\programmerare\crsConstants
 *         Example of the full names for some of those generated classes:
 *              com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_3.EpsgCode
 *              com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_3.EpsgNumber
 *              com.programmerare.crsConstants.constantsByAreaNumberName.v9_5_3.EpsgCode
 *              ....
 *         Six of the twelve classes has the name "EpsgNumber" and the others have the name "EpsgCode"
 *         In each of six packages those two classed are generated.
 *         The differences between them are reflected by the package names
 *         e.g. "constantsByAreaNameNumber" vs "constantsByAreaNumberName".
 *         The names of the constants are a concatenation of Area/Name/Number
 *         and the package names reflects the order of the concatenation.
 *         Two examples from the package including "constantsByAreaNameNumber":
 *          (e.g. the package com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_3)
 *          class EpsgCode {
 *              ...
 *              public final static String SWEDEN__SWEREF99_TM__3006 = "EPSG:3006";
 *              ...
 *          class EpsgNumber {
 *              ...
 *              public final static int SWEDEN__SWEREF99_TM__3006 = 3006;
 *              ...
 *          Note above that the names of the contants (within the same package)
 *          are exactly the same for the two classes in the package.
 *          You can also note that the order of the elements in the constant names
 *          are as you can see in the package name "constantsByAreaNameNumber"
 *          (i.e. the order Area and then Name and then Number as below):
 *              "Area":     "SWEDEN"
 *              "Name":     "SWEREF99_TM"
 *              "Number":   "3006"
 *          Another example from the package "constantsByNumberAreaName":
 *           (e.g. the package com.programmerare.crsConstants.constantsByNumberAreaName.v9_5_3)
 *              class EpsgNumber {
 *                  ...
 *                  public final static int _3006__SWEDEN__SWEREF99_TM = 3006;
 *                  ...
 *          Now you can note that the order of the elements in the constant names
 *          are as you can see in the package name "constantsByNumberAreaName"
 *          (i.e. the order Number and then Area and then Name as below):
 *              "Number":   "_3006"
 *              "Area":     "SWEDEN"
 *              "Name":     "SWEREF99_TM"
 *           (when the constant name begins with the number there is an additional _ as prefix)
 *          The purpose of the different contatenations is that it can be a matter of preference
 *          and how you want to use the constants, in which order you want the different parts.
 *          For example, if you like to use intellisense/autocompletion you might
 *          prefer to see the constants popped up being ordered by area, e.g. the
 *          constants for EPSG codes used specifically in Sweden are sorted together.
 */
class ConstantClassGenerator : CodeGeneratorBase() {

    private var nameOfConstants = mutableListOf<ConstantTypeNameValue>()
    private var constantNameRenderer = ConstantNameRenderer(RenderStrategyNameAreaNumberInteger())

    // Generates classes with constants based on database with EPSG codes:
    // http://www.epsg.org/EPSGDataset/DownloadDataset.aspx
    fun generateConstants() {
        val sqlQuery = SQL_STATEMENT_SELECTING_CRSCODE_CRSNAME_AREANAME

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

        // Generate Totally 12 classes below in 6 packages with 2 classes per package:
        generateJavaFileWithConstants(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForNameAreaNumber(), RenderStrategyNameAreaNumberInteger())
        generateJavaFileWithConstants(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForNameAreaNumber(), RenderStrategyNameAreaNumberString())

        generateJavaFileWithConstants(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForNameNumberArea(), RenderStrategyNameNumberAreaInteger())
        generateJavaFileWithConstants(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForNameNumberArea(), RenderStrategyNameNumberAreaString())

        generateJavaFileWithConstants(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForAreaNumberName(), RenderStrategyAreaNumberNameInteger())
        generateJavaFileWithConstants(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForAreaNumberName(), RenderStrategyAreaNumberNameString())

        generateJavaFileWithConstants(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForAreaNameNumber(), RenderStrategyAreaNameNumberInteger())
        generateJavaFileWithConstants(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForAreaNameNumber(), RenderStrategyAreaNameNumberString())

        generateJavaFileWithConstants(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForNumberAreaName(), RenderStrategyNumberAreaNameInteger(), sortByNumber = true)
        generateJavaFileWithConstants(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForNumberAreaName(), RenderStrategyNumberAreaNameString(), sortByNumber = true)

        generateJavaFileWithConstants(CLASS_NAME_INTEGER_CONSTANTS, getNameOfPackageForNumberNameArea(), RenderStrategyNumberNameAreaInteger(), sortByNumber = true)
        generateJavaFileWithConstants(CLASS_NAME_STRING_CONSTANTS,  getNameOfPackageForNumberNameArea(), RenderStrategyNumberNameAreaString(), sortByNumber = true)

        // The below method does not really belong here within this class
        // at least from a semantic point of view considering that the current name of the
        // class indicates that it should generated classes with constants ...
        generateCsvFile()
    }

    private fun generateJavaFileWithConstants(
        nameOfJavaClass: String,
        nameOfJavaPackage: String,
        renderStrategy: RenderStrategy,
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


    /**
     * Creates a pipe character separated file (but the file extension "csv" indicates comma as separator)
     * with the following three fields at each row: EpsgNumber|CrsName|AreaName
    * The name of the generated path/file will be something like this:
    *   ./crsCodeGeneration/src/main/resources/generated/csv_files/crs_number_name_area_v9_5_3.csv
    */
    private fun generateCsvFile() {
        val directoryWhereTheCsvFileShouldBeGenerated = getFileOrDirectory(NAME_OF_MODULE_DIRECTORY_FOR_CODE_GENERATION, RELATIVE_PATH_TO_TARGET_DIRECTORY_FOR_GENERATED_CODE_WITHIN_RESOURCES_DIRECTORY + "/csv_files", throwExceptionIfNotExisting = false)
        if(!directoryWhereTheCsvFileShouldBeGenerated.exists()) {
            println("Directory does not exist: " + directoryWhereTheCsvFileShouldBeGenerated.canonicalPath)
            val result = directoryWhereTheCsvFileShouldBeGenerated.mkdirs()
            println("Result of directory creation: " + result)
        }
        val fileName = "crs_number_name_area_" + getEpsgVersion() + ".csv"
        var csvFileToBecomeCreated = File(directoryWhereTheCsvFileShouldBeGenerated, fileName)
        nameOfConstants.sortedBy { it.epsgNumber }
        constantNameRenderer.renderStrategy = RenderStrategyNumberNameAreaInteger()
        // Note that the freemarker template file is using valueForConstant instead of epsgNumber
        // which is just a convenient way of creating rows such as:
        // 3006|SWEREF99 TM|Sweden
        // instead of: (i.e. with white space in the number)
        // 3 006|SWEREF99 TM|Sweden

        val rootHashMapWithDataToBeUsedByFreemarkerTemplate = HashMap<String, Any>()
        rootHashMapWithDataToBeUsedByFreemarkerTemplate.put(FREEMARKER_PROPERTY_NAME_OF_CONSTANTS, nameOfConstants)

        super.createFile(
            NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CSV_FILE,
            rootHashMapWithDataToBeUsedByFreemarkerTemplate,
            csvFileToBecomeCreated
        )
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
        val root = HashMap<String, Any>()
        root.put(FREEMARKER_PROPERTY_NAME_OF_CONSTANTS, constantsInformation.constants)
        root.put(FREEMARKER_PROPERTY_NAME_OF_JAVA_PACKAGE, constantsInformation.nameOfJavaPackage);
        root.put(FREEMARKER_PROPERTY_NAME_OF_JAVA_CLASS, constantsInformation.nameOfJavaClass);

        super.createFile(
            nameOfFreemarkerTemplate,
            root,
            javaFileToBecomeCreated
        )
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


    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            val validationMessage = getValidationErrorMessageOrEmptyStringIfNoError(args)
            if (!validationMessage.equals("")) {
                println(validationMessage)
                return
            }
            // args[0] is the version of EPSG and should be specified with underscores instead of dots e.g. "v9_5_3"
            setEpsgVersion(epsgVersion = args[0])
            setDatabaseInformationForMariaDbConnection(
                    databaseName = args[1],
                    databaseUserName = args[2],
                    databaseUserPassword = args[3]
            )
            val constantClassGenerator = ConstantClassGenerator()
            constantClassGenerator.generateConstants()
        }

        /**
         * Return empty string if validation is okay, otherwise
         * a string with a validation message that should be displayed
         */
        fun getValidationErrorMessageOrEmptyStringIfNoError(args: Array<String>): String {
            if (args.size < 4) {
                return "The method should have four parameters"
            }
            if(!isValidAsVersionPrefix(args[0])) {
                return "The version prefix is not valid. It should be a 'v' with some numbers, potentially separated with '_' instead of '.' . Example: 'v9_5_3' "
            }
            return ""
        }

        val regularExpressionForVersionSuffix = Regex("""v(\d+|\d+[_\d]*\d+)""")
        /**
         * @param versionSuffix a string such as "v9_5_3" (for a version 9.5.3)
         *  i.e. the "v" as prefix and then some version number with one or more digits,
         *  but instead of the normal dots the separator between major/minor version numbers should be underscore.
         *  The usage of the validated string is that it will be used as the last part of a package name.
         */
        fun isValidAsVersionPrefix(versionSuffix: String): Boolean {
            return regularExpressionForVersionSuffix.matches(versionSuffix)
        }

        private const val FREEMARKER_PROPERTY_NAME_OF_CONSTANTS = "constants"
        private const val FREEMARKER_PROPERTY_NAME_OF_JAVA_CLASS = "nameOfJavaClass"
        private const val FREEMARKER_PROPERTY_NAME_OF_JAVA_PACKAGE = "nameOfJavaPackage"

        private const val NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CONSTANTS = "Constants.ftlh"
        private const val NAME_OF_FREEMARKER_TEMPLATE_FILE_FOR_CSV_FILE = "CsvFileWithEpsgNumberAndCrsNameAndAreaName.ftlh"


        private const val CLASS_NAME_INTEGER_CONSTANTS = "EpsgNumber"
        private const val CLASS_NAME_STRING_CONSTANTS = "EpsgCode"

        private const val PACKAGE_NAME_PREFIX = "com.programmerare.crsConstants."

        private var _epsgVersion = "v_NotYetDefined"
        private fun setEpsgVersion(epsgVersion: String) {
            _epsgVersion = epsgVersion
        }
        private fun getEpsgVersion() : String {
            return _epsgVersion
        }
        private fun getPackageNameSuffix(): String {
            return "." + getEpsgVersion()
        }
   }
}