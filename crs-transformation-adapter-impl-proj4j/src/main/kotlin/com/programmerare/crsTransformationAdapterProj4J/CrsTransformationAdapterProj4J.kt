package com.programmerare.crsTransformationAdapterProj4J

import org.osgeo.proj4j.CRSFactory;
import org.osgeo.proj4j.CoordinateTransformFactory;
import org.osgeo.proj4j.ProjCoordinate;
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterBaseLeaf
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.coordinate.createFromXLongitudeYLatitude

// " Proj4J/proj4j "
// https://github.com/Proj4J/proj4j
class CrsTransformationAdapterProj4J : CrsTransformationAdapterBaseLeaf(), CrsTransformationAdapter {

    private var coordinateTransformFactory: CoordinateTransformFactory = CoordinateTransformFactory()
    private var crsFactory: CRSFactory = CRSFactory()

    override protected fun transformHook(
            inputCoordinate: CrsCoordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): CrsCoordinate {
        val sourceCrs = crsFactory.createFromName(inputCoordinate.crsIdentifier.crsCode)
        val targetCrs = crsFactory.createFromName(crsIdentifierForOutputCoordinateSystem.crsCode)
        val coordinateTransform = coordinateTransformFactory.createTransform(sourceCrs, targetCrs)
        val projCoordinateInput = ProjCoordinate(inputCoordinate.xEastingLongitude, inputCoordinate.yNorthingLatitude)
        val projCoordinateOutput = ProjCoordinate()
        coordinateTransform.transform(projCoordinateInput, projCoordinateOutput)
        return createFromXLongitudeYLatitude(projCoordinateOutput.x, projCoordinateOutput.y, crsIdentifierForOutputCoordinateSystem)
    }
}