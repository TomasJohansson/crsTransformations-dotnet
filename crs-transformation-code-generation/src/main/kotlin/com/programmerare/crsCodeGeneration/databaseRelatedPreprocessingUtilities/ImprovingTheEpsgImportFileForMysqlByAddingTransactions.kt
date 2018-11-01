package com.programmerare.crsCodeGeneration.databaseRelatedPreprocessingUtilities

import java.io.File
import java.nio.charset.Charset
import java.nio.file.Files

/**
 * The purpose of this class is to improve the downloaded "EPSG" file with MySQL/MariaDB
 * insert statements regarding performance.
 * The downloaded file contains a long list of insert sql statements
 * and this class creates a new file with the same insert statements
 * but also with "start transaction" and "commit" statements
 * at for example each 50th insert statement.
 * The main method below creates the output file in the same directory as the
 * input file, with some suffix.
 * For example if the input file has the name "EPSG_v9_3.mdb_Data_MySQL.sql"
 * then the output file may get the name "EPSG_v9_3.mdb_Data_MySQL_WithTransactions.sql"
 *
 * There are more documentation about the entire process of generating new constants
 * for a new version, in the file ConstantClassGenerator.
 */
class ImprovingTheEpsgImportFileForMysqlByAddingTransactions {

    /**
     * Not intended to be used with ".Companion" from client code.
     * The reason for its existence has to do with the fact that the
     * JVM class has been created with the programming language Kotlin.
     */
    companion object {
        @JvmStatic

        /**
         * The method needs one parameter, the full path for the big file
         * with sql insert statements, e.g. the full path to the file "EPSG_v9_3.mdb_Data_MySQL.sql"
         */
        fun main(args: Array<String>) {
            if(args.size == 0) {
                println("The file path was not provided as parameter to the main method")
                return
            }
            // val fullPathToEpsgMySqlFile = "F:\\code\\Kotlin\\crsFacade\\crsCodeGeneration\\data_files\\epsg-v9_3sql-MySQL\\EPSG_v9_3.mdb_Data_MySQL.sql"
            val fullPathToEpsgMySqlFile = args[0]
            val inputEpsgFileWithMysqlInsertStatements = File(fullPathToEpsgMySqlFile)
            if(!inputEpsgFileWithMysqlInsertStatements.exists()) {
                println("The path is not a file: " + fullPathToEpsgMySqlFile)
                return
            }
            val outputEpsgFileWithMysqlInsertStatements = File(inputEpsgFileWithMysqlInsertStatements.parent, inputEpsgFileWithMysqlInsertStatements.nameWithoutExtension + "_WithTransactions.sql")
            if(outputEpsgFileWithMysqlInsertStatements.exists()) {
                println("The output file already exists. To avoid accidental overwriting you must delete it manually: " + outputEpsgFileWithMysqlInsertStatements.canonicalPath)
                return
            }

            val maxNumberOfInsertsForOneTransaction = 50

            var insertCounterForOneTransaction = 0
            var insertCounterTotal = 0
            val rowsForOutputFile= mutableListOf<String>()
            rowsForOutputFile.add("set autocommit=0;")
            rowsForOutputFile.add("start transaction;")

            // Charsets.UTF_8
            // Charsets.ISO_8859_1
            //inputEpsgFileWithMysqlInsertStatements.forEachLine(Charsets.ISO_8859_1){ line ->
            // The "EPSG" file should be UTF8. If not then it should be converted to UTF8 before this method is executed.
            // For further information about the expected encoding see the documentation in "ConstantClassGenerator"
            // ( .\crsCodeGeneration\src\main\kotlin\com\programmerare\crsCodeGeneration\constantsGenerator\ConstantClassGenerator.kt )
            inputEpsgFileWithMysqlInsertStatements.forEachLine(Charsets.UTF_8){ line ->
                // Australia - 102�E to 108�E
                // Australia - 102°E to 108°E
                if(line.contains("Australia - 102")) {
                    println("ANSI / UTF8 problem ? : " + line)
                }

                if(line.trim().toLowerCase().startsWith("insert into")) {
                    insertCounterTotal++
                    insertCounterForOneTransaction++
                    if(insertCounterForOneTransaction > maxNumberOfInsertsForOneTransaction) {
                        rowsForOutputFile.add("commit;")
                        rowsForOutputFile.add("start transaction;")
                        insertCounterForOneTransaction = 0
                    }
                }
                rowsForOutputFile.add(line)
            }
            rowsForOutputFile.add("commit;")
            rowsForOutputFile.add("set autocommit=1;")

            print("Total number of SQL INSERT statements: " + insertCounterTotal) // 53801

            Files.write(outputEpsgFileWithMysqlInsertStatements.toPath(), rowsForOutputFile, Charsets.UTF_8)
       }
    }
}