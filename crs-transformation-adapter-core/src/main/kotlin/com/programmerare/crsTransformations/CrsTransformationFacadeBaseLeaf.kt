package com.programmerare.crsTransformations

abstract class CrsTransformationFacadeBaseLeaf : CrsTransformationFacadeBase(), CrsTransformationFacade {

    override final fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        try {
            val outputCoordinate = transformHook(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            if(
                java.lang.Double.isNaN(outputCoordinate.yLatitude)
                ||
                java.lang.Double.isNaN(outputCoordinate.xLongitude)
            ) {
                return TransformResultImplementation(inputCoordinate, null, exception = null, isSuccess = false, crsTransformationFacadeThatCreatedTheResult = this)
            }
            else {
                return TransformResultImplementation(inputCoordinate, outputCoordinate, exception = null, isSuccess = outputCoordinate != null, crsTransformationFacadeThatCreatedTheResult = this)
            }
        }
        catch (e: Throwable) {
            return TransformResultImplementation(inputCoordinate, null, exception = e, isSuccess = false, crsTransformationFacadeThatCreatedTheResult = this)
        }
    }

}