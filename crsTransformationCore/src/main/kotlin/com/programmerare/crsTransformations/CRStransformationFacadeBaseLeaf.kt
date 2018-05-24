package com.programmerare.crsTransformations

abstract class CRStransformationFacadeBaseLeaf : CRStransformationFacadeBase(), CRStransformationFacade {

    override final fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        try {
            val outputCoordinate = transform(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            return TransformResultImplementation(inputCoordinate, outputCoordinate, exception = null, isSuccess = true)
        }
        catch (e: Exception) {
            return TransformResultImplementation(inputCoordinate, null, exception = e, isSuccess = false)
        }
    }

}