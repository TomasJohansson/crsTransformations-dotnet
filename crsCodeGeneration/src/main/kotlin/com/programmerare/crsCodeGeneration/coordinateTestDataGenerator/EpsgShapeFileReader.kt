package com.programmerare.crsCodeGeneration.coordinateTestDataGenerator

import com.vividsolutions.jts.geom.Geometry
import com.vividsolutions.jts.geom.MultiPolygon
import org.geotools.data.*
import org.opengis.feature.simple.SimpleFeature
import java.io.File

// Note that if you do not have shapefile at the class path
// there will be a "silent error" i.e. null will be returned
// without information that "gt-shapefile" is missing ...
// build.gradle
//compile group: 'org.geotools', name: 'gt-shapefile', version: '19.1'
//compile group: 'org.geotools', name: 'gt-data', version: '19.1'
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