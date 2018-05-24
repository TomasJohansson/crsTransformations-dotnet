package com.programmerare.crsTransformations

abstract class CRStransformationFacadeBaseLeaf : CRStransformationFacade {
    override final fun transform(inputCoordinate: Coordinate, crsCodeForOutputCoordinateSystem: String): Coordinate {
        // the below invoked overloaded method is implemented in subclasses
        // i.e. it is a hook method invoked by this Template Method
        return transform(inputCoordinate, CrsIdentifier.createFromCrsCode(crsCodeForOutputCoordinateSystem))
    }

    override final fun transform(inputCoordinate: Coordinate, epsgNumberForOutputCoordinateSystem: Int): Coordinate {
        // this Template Method is invokign the below overloaded hook method in subclasses
        return transform(inputCoordinate, CrsIdentifier.createFromEpsgNumber(epsgNumberForOutputCoordinateSystem))
    }

    override final fun transformToResultObject(inputCoordinate: Coordinate, epsgNumberForOutputCoordinateSystem: Int): TransformResult {
        return transformToResultObject(inputCoordinate, CrsIdentifier.createFromEpsgNumber(epsgNumberForOutputCoordinateSystem))
    }

    override final fun transformToResultObject(inputCoordinate: Coordinate, crsCodeForOutputCoordinateSystem: String): TransformResult {
        return transformToResultObject(inputCoordinate, CrsIdentifier.createFromCrsCode(crsCodeForOutputCoordinateSystem))
    }

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