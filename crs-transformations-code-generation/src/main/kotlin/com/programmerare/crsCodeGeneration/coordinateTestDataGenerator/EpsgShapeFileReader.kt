package com.programmerare.crsCodeGeneration.coordinateTestDataGenerator

import org.locationtech.jts.geom.Geometry
import org.locationtech.jts.geom.MultiPolygon
import org.geotools.data.*
import org.opengis.feature.simple.SimpleFeature
import java.io.File

// Note that if you do not have "gt-shapefile" at the class path
// there will be a "silent error" i.e. null will be returned
// without information that "gt-shapefile" is missing ...
// build.gradle
//compile group: 'org.geotools', name: 'gt-shapefile', version: '20.0'
//compile group: 'org.geotools', name: 'gt-data', version: '20.0'
// http://docs.geotools.org/latest/userguide/welcome/architecture.html

class EpsgShapeFileReader {

    fun extractAttributeDataFromShapefile(pathToEpsgShapeFile: String): List<RowOfAttributeTableAndCentroid> {
        val rowsOfAttributeTable = mutableListOf<RowOfAttributeTableAndCentroid>()
        val epsgShapefile = File(pathToEpsgShapeFile)
        val fileDataStore: FileDataStore = FileDataStoreFinder.getDataStore(epsgShapefile)!!
        val featureSource = fileDataStore.getFeatureSource()
        val simpleFeaturesCollection = featureSource.features
        val simpleFeatureIterator = simpleFeaturesCollection.features()
        while (simpleFeatureIterator.hasNext()) {
            val simpleFeature: SimpleFeature = simpleFeatureIterator.next()
            val areaName = simpleFeature.getAttribute("AREA_NAME") as String
            val areaCode = simpleFeature.getAttribute("AREA_CODE") as Int
            val version = simpleFeature.getAttribute("VERSION") as String
            val region = simpleFeature.getAttribute("REGION") as String
            val label = simpleFeature.getAttribute("LABEL") as String
            val dbRelease = simpleFeature.getAttribute("DB_RELEASE") as String
            val geometry = simpleFeature.defaultGeometry as Geometry
            val multiPolygon = geometry as MultiPolygon
            val centroidPoint = multiPolygon.centroid
            val rowOfAttributeTableAndCentroid = RowOfAttributeTableAndCentroid(
                centroidPoint.x,
                centroidPoint.y,
                areaName,
                areaCode,
                version,
                region,
                label,
                dbRelease
            )
            rowsOfAttributeTable.add(rowOfAttributeTableAndCentroid)
        }
        return rowsOfAttributeTable
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
         *  The method needs a parameter which is a full path to a shapefile
         *  (e.g. use the absolute path for a file such as ...\crsCodeGeneration\data_files\EPSG_Polygons_Ver_9.2.1\EPSG_Polygons.shp )
         */
        fun main(args: Array<String>) {
            if(args.size < 1) {
                println("You must provide the full path to an EPSG shapefile")
                return
            }
            val file = File(args[0])
            if(!file.exists()) {
                println("The shapefile does not exist: " + file.canonicalPath)
                return
            }
            val epsgShapeFileReader = EpsgShapeFileReader()
            val rowsOfAttributeTableAndCentroid = epsgShapeFileReader.extractAttributeDataFromShapefile(file.canonicalPath)
            for (item in rowsOfAttributeTableAndCentroid) {
                if(item.AREA_NAME.toUpperCase().contains("SWEDEN")) {
                    println("AREA_CODE: " + item.AREA_CODE)
                    println("AREA_NAME: " + item.AREA_NAME)
                    println("LABEL: " + item.LABEL)
                    println("REGION: " + item.REGION)
                    println("VERSION: " + item.VERSION)
                    println("centroidX: " + item.centroidX)
                    println("centroidY: " + item.centroidY)
                }
            }
        }
    }

}

data class RowOfAttributeTableAndCentroid(
    //val the_geom
    val centroidX: Double,
    val centroidY: Double,
    val AREA_NAME: String,
    val AREA_CODE: Int,
    val VERSION: String,
    val REGION: String,
    val LABEL: String,
    val DB_RELEASE: String
)
{
}