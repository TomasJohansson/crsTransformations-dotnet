package com.programmerare.crsCodeGeneration

import freemarker.template.Configuration
import java.io.File;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.query
import org.springframework.jdbc.datasource.DriverManagerDataSource;
import java.nio.file.Paths
import java.sql.Driver

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

    fun run() {
        val freemarkerConfiguration = Configuration(Configuration.VERSION_2_3_28);
        println("TODO code generation ...")
        // TODO: generate classes with constants e.g. based on database with EPSG codes:
        // http://www.epsg.org/EPSGDataset/DownloadDataset.aspx

        val file: File = getAccessDatabaseFileWithEpsgData()
        val connectionString = "jdbc:ucanaccess://" + file.absolutePath + ";memory=false"// jdbc:ucanaccess://c:/data/mydb.mdb;memory=false
        val driverManagerDataSource = DriverManagerDataSource(connectionString)
        val jdbcTemplate = JdbcTemplate(driverManagerDataSource)

        val sqlQueryBase = " SELECT [Area].[AREA_NAME], [Coordinate Reference System].[AREA_OF_USE_CODE], [Coordinate Reference System].[COORD_REF_SYS_CODE], [Coordinate Reference System].[COORD_REF_SYS_NAME] FROM [Coordinate Reference System] , [Area] WHERE [Coordinate Reference System].[AREA_OF_USE_CODE] = [Area].[AREA_CODE] "
        val sqlQueryCondition = " AND [Area].[AREA_NAME] LIKE ? "
        val sqlQueryOrderClause = " ORDER BY [Area].[AREA_NAME] , [Coordinate Reference System].[COORD_REF_SYS_NAME] "
        val sqlQuery = sqlQueryBase + sqlQueryCondition + sqlQueryOrderClause

        val fieldSeparator = "__"

        jdbcTemplate.query(sqlQuery, "%Sweden%") { rs, _ ->
            val epsgNumber = rs.getInt("COORD_REF_SYS_CODE")
            val areaName = rs.getString("AREA_NAME")
            val crsName = rs.getString("COORD_REF_SYS_NAME")
            val nameForConstant1 = "_" + epsgNumber + fieldSeparator + areaName + fieldSeparator + crsName
            val nameForConstant2 = "_" + epsgNumber + fieldSeparator + crsName + fieldSeparator + areaName
            val nameForConstant3 = areaName + fieldSeparator + crsName + fieldSeparator + epsgNumber
            val nameForConstant4 = areaName + fieldSeparator + epsgNumber + fieldSeparator + crsName
            val nameForConstant5 = crsName + fieldSeparator + areaName + fieldSeparator + epsgNumber
            val nameForConstant6 = crsName + fieldSeparator + epsgNumber + fieldSeparator + areaName
            println("nameForConstant1 " + nameForConstant1.adjusted())
            println("nameForConstant2 " + nameForConstant2.adjusted())
            println("nameForConstant3 " + nameForConstant3.adjusted())
            println("nameForConstant4 " + nameForConstant4.adjusted())
            println("nameForConstant5 " + nameForConstant5.adjusted())
            println("nameForConstant6 " + nameForConstant6.adjusted())
//            nameForConstant1 _3006__SWEDEN__SWEREF99_TM
//            nameForConstant2 _3006__SWEREF99_TM__SWEDEN
//            nameForConstant3 SWEDEN__SWEREF99_TM__3006
//            nameForConstant4 SWEDEN__3006__SWEREF99_TM
//            nameForConstant5 SWEREF99_TM__SWEDEN__3006
//            nameForConstant6 SWEREF99_TM__3006__SWEDEN
        }
    }

    private fun getAccessDatabaseFileWithEpsgData(): File {
        val workingDirectory = Paths.get(System.getProperty("user.dir"))
        val pathToAccessDatabaseFile = workingDirectory.resolve(relativePathToAccessDatabaseFile)
        val file = pathToAccessDatabaseFile.toFile()
        if(!file.exists()) {
            throw RuntimeException("Access database file does not exist: " + file.absolutePath);
        }
        return file
    }

    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            verifyJdbcDriver()
            println("Code generation starts ... (TODO ...) ")
            val constantClassGenerator = ConstantClassGenerator()
            constantClassGenerator.run()
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

        private val nameOfJdbcDriver = "net.ucanaccess.jdbc.UcanaccessDriver"

        // Note that when the path below is changed: The path is mentioned in gitignore
        private val relativePathToAccessDatabaseFile = "data_files/EPSG_v9_3/EPSG_v9_3.mdb"
    }
}

fun String.adjusted(): String {
    return this.getUppercasedWithOnylValidCharacters()
}

fun String.getUppercasedWithOnylValidCharacters(): String {
    // the last regexp below just makes sure there are will not be more than two "_" in a row
    return this.toUpperCase().replace("[^a-zA-Z0-9_]".toRegex(), "_").replace("_{2,}".toRegex(), "__")
}