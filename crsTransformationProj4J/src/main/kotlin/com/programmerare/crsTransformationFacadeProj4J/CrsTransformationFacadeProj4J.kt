package com.programmerare.crsTransformationFacadeProj4J

import org.osgeo.proj4j.CRSFactory;
import org.osgeo.proj4j.CoordinateTransformFactory;
import org.osgeo.proj4j.ProjCoordinate;
import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsTransformations.CrsTransformationFacadeBaseLeaf
import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier

// " Proj4J/proj4j "
// https://github.com/Proj4J/proj4j
class CrsTransformationFacadeProj4J : CrsTransformationFacadeBaseLeaf(), CrsTransformationFacade {

    private var coordinateTransformFactory: CoordinateTransformFactory = CoordinateTransformFactory()
    private var crsFactory: CRSFactory = CRSFactory()

    override protected fun transformHook(
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): Coordinate {
        val sourceCrs = crsFactory.createFromName(inputCoordinate.crsIdentifier.crsCode)
        val targetCrs = crsFactory.createFromName(crsIdentifierForOutputCoordinateSystem.crsCode)
        val coordinateTransform = coordinateTransformFactory.createTransform(sourceCrs, targetCrs)
        val projCoordinateInput = ProjCoordinate(inputCoordinate.xLongitude, inputCoordinate.yLatitude)
        val projCoordinateOutput = ProjCoordinate()
        coordinateTransform.transform(projCoordinateInput, projCoordinateOutput)
        return Coordinate.createFromXLongitudeYLatitude(projCoordinateOutput.x, projCoordinateOutput.y, crsIdentifierForOutputCoordinateSystem)
    }
}