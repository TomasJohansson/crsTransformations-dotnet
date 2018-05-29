package com.programmerare.crsTransformations

abstract class CrsTransformationFacadeBase : CrsTransformationFacade {

    override final fun transform(
        inputCoordinate: Coordinate,
        crsCodeForOutputCoordinateSystem: String
    ): Coordinate {
        // this Template Method is invoking the below overloaded hook method in subclasses
        return transformHook(
            inputCoordinate,
            CrsIdentifier.createFromCrsCode(crsCodeForOutputCoordinateSystem)
        )
    }

    override final fun transform(
        inputCoordinate: Coordinate,
        epsgNumberForOutputCoordinateSystem: Int
    ): Coordinate {
        return transformHook(
            inputCoordinate,
            CrsIdentifier.createFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
        )
    }

    override final fun transform(
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): Coordinate {
        return transformHook(
            inputCoordinate,
            crsIdentifierForOutputCoordinateSystem
        )
    }

    abstract protected fun transformHook(
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): Coordinate


    override final fun transformToResultObject(
        inputCoordinate: Coordinate,
        epsgNumberForOutputCoordinateSystem: Int
    ): TransformResult {
        return transformToResultObject(
            inputCoordinate,
            CrsIdentifier.createFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
        )
    }

    override final fun transformToResultObject(
        inputCoordinate: Coordinate,
        crsCodeForOutputCoordinateSystem: String
    ): TransformResult {
        return transformToResultObject(
            inputCoordinate,
            CrsIdentifier.createFromCrsCode(crsCodeForOutputCoordinateSystem)
        )
    }

//    override final fun transformToResultObject(
//        inputCoordinate: Coordinate,
//        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
//    ): TransformResult {
//        return transformHook(inputCoordinate, CrsIdentifier.createFromCrsCode(crsCodeForOutputCoordinateSystem))
//    }


}