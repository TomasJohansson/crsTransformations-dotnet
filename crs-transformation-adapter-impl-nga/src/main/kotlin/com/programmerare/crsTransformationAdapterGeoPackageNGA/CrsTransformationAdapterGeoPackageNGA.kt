package com.programmerare.crsTransformationAdapterGeoPackageNGA

import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterBaseLeaf
import com.programmerare.crsTransformations.coordinate.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import com.programmerare.crsTransformations.coordinate.createFromXLongitudeYLatitude
import mil.nga.sf.Point
import mil.nga.sf.proj.ProjectionFactory

// " ngageoint/geopackage-java "
// https://github.com/ngageoint/geopackage-java
// https://github.com/ngageoint/geopackage-core-java
// http://ngageoint.github.io/geopackage-java/
class CrsTransformationAdapterGeoPackageNGA : CrsTransformationAdapterBaseLeaf(), CrsTransformationAdapter {

    override protected fun transformHook(
            inputCoordinate: Coordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): Coordinate {
        val inputEPSGnumber = inputCoordinate.crsIdentifier.epsgNumber.toLong()
        val outputEPSGnumber = crsIdentifierForOutputCoordinateSystem.epsgNumber.toLong()
        val projection = ProjectionFactory.getProjection(inputEPSGnumber) // "EPSG:4326" wgs84
        val projectionTransform = projection.getTransformation(outputEPSGnumber)  // sweref 99
        val inputPoint = Point(inputCoordinate.xLongitude, inputCoordinate.yLatitude)
        val outputPoint = projectionTransform.transform(inputPoint)
        val outputCoordinate = createFromXLongitudeYLatitude(outputPoint.getX(), outputPoint.getY(), crsIdentifierForOutputCoordinateSystem)
        return outputCoordinate
    }
}