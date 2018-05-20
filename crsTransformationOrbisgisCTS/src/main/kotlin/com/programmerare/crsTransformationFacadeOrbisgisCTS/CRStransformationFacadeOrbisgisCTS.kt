package com.programmerare.crsTransformationFacadeOrbisgisCTS

import java.util.*
import org.cts.CRSFactory;
import org.cts.crs.GeodeticCRS;
import org.cts.op.CoordinateOperationFactory;
import org.cts.registry.EPSGRegistry;

// " orbisgis/cts "
// https://github.com/orbisgis/cts
object CRStransformationFacadeOrbisgisCTS {

    // This ugly method signature (using a List<Double> as both input and output, will be refactored later
    // and will use a Coordinate object instead)
    // This ugly method signature (using a List<Double> as both input and output, will be refactored later
    // and will use a Coordinate object instead)
    @JvmStatic
    fun transformWgs84CoordinateToSweref99TM(
        inputCoordinate: List<Double>
    ): List<Double> {
        val crsFactory: CRSFactory = CRSFactory()
        val registryManager = crsFactory.registryManager
        registryManager.addRegistry(EPSGRegistry())
        val inputCRS = crsFactory.getCRS("EPSG:4326") // WGS84
        val outputCRS = crsFactory.getCRS("EPSG:3006") // SWEREF99 TM
        val inputCRSgeodetic = inputCRS as GeodeticCRS
        val outputCRSgeodetic = outputCRS as GeodeticCRS
        val coordinateOperations = CoordinateOperationFactory.createCoordinateOperations(inputCRSgeodetic, outputCRSgeodetic)
        val coordinateOperation = CoordinateOperationFactory.getMostPrecise(coordinateOperations);
        val inputCoordinateArray = doubleArrayOf(inputCoordinate.get(1), inputCoordinate.get(0))
        val outputCoordinateArray = coordinateOperation.transform(inputCoordinateArray)
        return Arrays.asList(outputCoordinateArray[1], outputCoordinateArray[0])
    }
}