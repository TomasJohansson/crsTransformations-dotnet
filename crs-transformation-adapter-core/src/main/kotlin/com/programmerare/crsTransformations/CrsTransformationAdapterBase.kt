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
    ): CrsTransformationResult {
        return transform(
            inputCoordinate,
                createFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
        )
    }

    override final fun transform(
            inputCoordinate: Coordinate,
            crsCodeForOutputCoordinateSystem: String
    ): CrsTransformationResult {
        return transform(
            inputCoordinate,
                createFromCrsCode(crsCodeForOutputCoordinateSystem)
        )
    }

    override final fun getLongNameOfImplementation(): String {
        return this.javaClass.name
    }

    private val classNamePrefix = "CrsTransformationAdapter"
    // if the above string would change because of class renamings
    // then it will be detected by a failing test

    override final fun getShortNameOfImplementation(): String {
        val className = this.javaClass.simpleName
        if(className.startsWith(classNamePrefix) && !className.equals(classNamePrefix)) {
            return className.substring(classNamePrefix.length)
        }
        else {
            return className
        }
    }
}