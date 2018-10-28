package com.programmerare.crsTransformationFacadeGeoPackageNGA

import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsTransformations.CrsTransformationFacadeBaseLeaf
import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import mil.nga.sf.Point
import mil.nga.sf.proj.ProjectionFactory

// " ngageoint/geopackage-java "
// https://github.com/ngageoint/geopackage-java
// https://github.com/ngageoint/geopackage-core-java
// http://ngageoint.github.io/geopackage-java/
class CrsTransformationFacadeGeoPackageNGA : CrsTransformationFacadeBaseLeaf(), CrsTransformationFacade {

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
        val outputCoordinate = Coordinate.createFromXLongitudeYLatitude(outputPoint.getX(), outputPoint.getY(), crsIdentifierForOutputCoordinateSystem)
        return outputCoordinate
    }
}