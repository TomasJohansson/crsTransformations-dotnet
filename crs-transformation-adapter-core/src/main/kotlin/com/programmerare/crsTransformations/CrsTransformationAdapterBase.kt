package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.Coordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.crsIdentifier.createFromCrsCode
import com.programmerare.crsTransformations.crsIdentifier.createFromEpsgNumber

abstract class CrsTransformationAdapterBase : CrsTransformationAdapter {

    override final fun transformToCoordinate(
            inputCoordinate: Coordinate,
            crsCodeForOutputCoordinateSystem: String
    ): Coordinate {
        // this Template Method is invoking the below overloaded hook method in subclasses
        return transformHook(
            inputCoordinate,
                createFromCrsCode(crsCodeForOutputCoordinateSystem)
        )
    }

    override final fun transformToCoordinate(
            inputCoordinate: Coordinate,
            epsgNumberForOutputCoordinateSystem: Int
    ): Coordinate {
        return transformHook(
            inputCoordinate,
                createFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
        )
    }

    override final fun transformToCoordinate(
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


    override final fun transform(
            inputCoordinate: Coordinate,
            epsgNumberForOutputCoordinateSystem: Int
    ): TransformResult {
        return transform(
            inputCoordinate,
                createFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
        )
    }

    override final fun transform(
            inputCoordinate: Coordinate,
            crsCodeForOutputCoordinateSystem: String
    ): TransformResult {
        return transform(
            inputCoordinate,
                createFromCrsCode(crsCodeForOutputCoordinateSystem)
        )
    }

//    override final fun transform(
//        inputCoordinate: Coordinate,
//        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
//    ): TransformResult {
//        return transformHook(inputCoordinate, CrsIdentifierFactory.createFromCrsCode(crsCodeForOutputCoordinateSystem))
//    }


    override final fun getLongNameOfImplementation(): String {
        return this.javaClass.name
    }

}