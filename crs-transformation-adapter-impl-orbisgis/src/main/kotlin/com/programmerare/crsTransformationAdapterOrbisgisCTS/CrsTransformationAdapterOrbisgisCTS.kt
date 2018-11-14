package com.programmerare.crsTransformationAdapterOrbisgisCTS

import com.programmerare.crsTransformations.CrsTransformationAdapteeType
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterBaseLeaf
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.coordinate.createFromYNorthingLatitudeAndXEastingLongitude
import org.cts.CRSFactory;
import org.cts.crs.GeodeticCRS;
import org.cts.op.CoordinateOperationFactory;
import org.cts.registry.EPSGRegistry;

// " orbisgis/cts "
// https://github.com/orbisgis/cts

/**
 * Implementation of the interface CrsTransformationAdapter.
 * See the documentation of the interface.
 * @see com.programmerare.crsTransformations.CrsTransformationAdapter
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-code-generation" project is licensed with MIT.
 * The code in the "crs-transformation-adapter-impl-orbisgis" project
 * is licensed with LGPLV3+ i.e. the same license as the adaptee library orbisgis/cts.
 */
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
        val inputCoordinateArray = doubleArrayOf(inputCoordinate.xEastingLongitude, inputCoordinate.yNorthingLatitude)
        val outputCoordinateArray = coordinateOperation.transform(inputCoordinateArray)
        return createFromYNorthingLatitudeAndXEastingLongitude(yNorthingLatitude = outputCoordinateArray[1], xEastingLongitude = outputCoordinateArray[0], crsIdentifier = crsIdentifierForOutputCoordinateSystem)
    }

    // ----------------------------------------------------------
    override fun getAdapteeType() : CrsTransformationAdapteeType {
        return CrsTransformationAdapteeType.LEAF_ORBISGIS_1_5_1
    }
    // The purpose of the method below is to use it in test code
    // for detecting upgrades to a new version (and then update the above method returned enum value)
    // Future failure will be a reminder to update the above enum value
    protected override fun getNameOfJarFileOrEmptyString(): String {
        return super.getNameOfJarFileFromProtectionDomain(GeodeticCRS::class.java.protectionDomain)
    }
    // ----------------------------------------------------------
}