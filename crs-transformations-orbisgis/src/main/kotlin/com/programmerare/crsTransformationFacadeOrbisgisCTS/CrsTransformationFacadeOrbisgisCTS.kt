package com.programmerare.crsTransformationFacadeOrbisgisCTS

import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsTransformations.CrsTransformationFacadeBaseLeaf
import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import org.cts.CRSFactory;
import org.cts.crs.GeodeticCRS;
import org.cts.op.CoordinateOperationFactory;
import org.cts.registry.EPSGRegistry;

// " orbisgis/cts "
// https://github.com/orbisgis/cts
class CrsTransformationFacadeOrbisgisCTS : CrsTransformationFacadeBaseLeaf(), CrsTransformationFacade {

    override protected fun transformHook(
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): Coordinate {
        val crsFactory: CRSFactory = CRSFactory()
        val registryManager = crsFactory.registryManager
        registryManager.addRegistry(EPSGRegistry())
        val inputCRS = crsFactory.getCRS(inputCoordinate.crsIdentifier.crsCode) // e.g. "EPSG:4326" = WGS84
        val outputCRS = crsFactory.getCRS(crsIdentifierForOutputCoordinateSystem.crsCode) // e.g. "EPSG:3006" = SWEREF99 TM
        val inputCRSgeodetic = inputCRS as GeodeticCRS
        val outputCRSgeodetic = outputCRS as GeodeticCRS
        val coordinateOperations = CoordinateOperationFactory.createCoordinateOperations(inputCRSgeodetic, outputCRSgeodetic)
        val coordinateOperation = CoordinateOperationFactory.getMostPrecise(coordinateOperations);
        val inputCoordinateArray = doubleArrayOf(inputCoordinate.xLongitude, inputCoordinate.yLatitude)
        val outputCoordinateArray = coordinateOperation.transform(inputCoordinateArray)
        return Coordinate.createFromYLatitudeXLongitude(yLatitude = outputCoordinateArray[1], xLongitude = outputCoordinateArray[0], crsIdentifier = crsIdentifierForOutputCoordinateSystem)
    }
}