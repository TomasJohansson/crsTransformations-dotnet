package com.programmerare.crsTransformationFacadeGeoTools

// build.gradle: implementation("org.geotools:gt-main:19.1")
import com.vividsolutions.jts.geom.GeometryFactory // jts-core-...jar
import org.geotools.geometry.jts.JTSFactoryFinder // gt-main-...jar
import org.geotools.referencing.CRS
import org.opengis.referencing.crs.CoordinateReferenceSystem
import org.opengis.referencing.operation.MathTransform
import com.programmerare.crsTransformations.CRStransformationFacade
import com.programmerare.crsTransformations.Coordinate
import org.geotools.geometry.jts.JTS

// http://docs.geotools.org/
// https://github.com/geotools/geotools/blob/master/pom.xml
class CRStransformationFacadeGeoTools : CRStransformationFacade {

    private val epsgPrefix = "EPSG:" // TODO: define this string ONCE in some appropriate place ...

    private val geometryFactory: GeometryFactory

    init {
        geometryFactory = JTSFactoryFinder.getGeometryFactory()
    }

    override fun transform(
        inputCoordinate: Coordinate,
        epsgNumberForOutputCoordinateSystem: Int
    ): Coordinate {
        val sourceCRS: CoordinateReferenceSystem = CRS.decode(epsgPrefix + inputCoordinate.epsgNumber, true)
        val targetCRS: CoordinateReferenceSystem = CRS.decode(epsgPrefix + epsgNumberForOutputCoordinateSystem, true)
        val mathTransform: MathTransform = CRS.findMathTransform(sourceCRS, targetCRS)

        /*
        val sourceArray = doubleArrayOf(inputCoordinate.xLongitude, inputCoordinate.yLatitude)
        val destinationArray = doubleArrayOf(0.0, 0.0)
        JTS.xform(transform, sourceArray, destinationArray)
        val lon = destinationArray[0]
        val lat = destinationArray[1]
        */
        // the above implementation is an alternative to the below implementation
        val inputPoint = geometryFactory.createPoint(com.vividsolutions.jts.geom.Coordinate(inputCoordinate.xLongitude, inputCoordinate.yLatitude))
        val sourceGeometry = inputPoint
        val outputGeometry = JTS.transform(sourceGeometry, mathTransform)
        val outputCoordinate = outputGeometry.coordinate
        val lon = outputCoordinate.x
        val lat = outputCoordinate.y

        return Coordinate(yLatitude = lat, xLongitude = lon, epsgNumber = epsgNumberForOutputCoordinateSystem)
    }
}