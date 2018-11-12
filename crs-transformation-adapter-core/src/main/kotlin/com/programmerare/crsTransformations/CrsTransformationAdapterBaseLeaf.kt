package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier

/**
 * Base class for the 'leaf' adapters.
 * @see CrsTransformationAdapterBase
 * @see CrsTransformationAdapter
 */
abstract class CrsTransformationAdapterBaseLeaf : CrsTransformationAdapterBase(), CrsTransformationAdapter {

    override final fun transform(inputCoordinate: CrsCoordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): CrsTransformationResult {
        try {
            val outputCoordinate = transformHook(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            if(
                java.lang.Double.isNaN(outputCoordinate.yNorthingLatitude)
                ||
                java.lang.Double.isNaN(outputCoordinate.xEastingLongitude)
            ) {
                return CrsTransformationResult._createCrsTransformationResult(
                    inputCoordinate,
                    null,
                    exception = null, 
                    isSuccess = false,
                    crsTransformationAdapterResultSource = this
                )
            }
            else {
                return CrsTransformationResult._createCrsTransformationResult(
                    inputCoordinate,
                    outputCoordinate,
                    exception = null,
                    isSuccess = outputCoordinate != null,
                    crsTransformationAdapterResultSource = this
                )
            }
        }
        catch (e: Throwable) {
            return CrsTransformationResult._createCrsTransformationResult(
                inputCoordinate,
                null,
                exception = e,
                isSuccess = false,
                crsTransformationAdapterResultSource = this
            )
        }
    }

    override final fun getTransformationAdapterChildren(): List<CrsTransformationAdapter> {
        return listOf<CrsTransformationAdapter>()
    }

    override final fun isComposite(): Boolean {
        return false
    }

    override fun getAdapteeType() : CrsTransformationAdapteeType {
        return CrsTransformationAdapteeType.UNSPECIFIED_LEAF
    }
}