package com.programmerare.crsTransformationAdapterOrbisgisCTS

import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterBaseLeaf
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.coordinate.createFromYLatitudeXLongitude
import org.cts.CRSFactory;
import org.cts.crs.GeodeticCRS;
import org.cts.op.CoordinateOperationFactory;
import org.cts.registry.EPSGRegistry;

// " orbisgis/cts "
// https://github.com/orbisgis/cts
class CrsTransformationAdapterOrbisgisCTS : CrsTransformationAdapterBaseLeaf(), CrsTransformationAdapter {

    override protected fun transformHook(
            inputCoordinate: CrsCoordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): CrsCoordinate {
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
        return createFromYLatitudeXLongitude(yLatitude = outputCoordinateArray[1], xLongitude = outputCoordinateArray[0], crsIdentifier = crsIdentifierForOutputCoordinateSystem)
    }
}