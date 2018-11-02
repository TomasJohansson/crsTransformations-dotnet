package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.crsIdentifier.createFromCrsCode
import com.programmerare.crsTransformations.crsIdentifier.createFromEpsgNumber

abstract class CrsTransformationAdapterBase : CrsTransformationAdapter {

    override final fun transformToCoordinate(
            inputCoordinate: CrsCoordinate,
            crsCodeForOutputCoordinateSystem: String
    ): CrsCoordinate {
        // this Template Method is invoking the below overloaded hook method in subclasses
        return transformHook(
            inputCoordinate,
                createFromCrsCode(crsCodeForOutputCoordinateSystem)
        )
    }

    override final fun transformToCoordinate(
            inputCoordinate: CrsCoordinate,
            epsgNumberForOutputCoordinateSystem: Int
    ): CrsCoordinate {
        return transformHook(
            inputCoordinate,
                createFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
        )
    }

    override final fun transformToCoordinate(
            inputCoordinate: CrsCoordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): CrsCoordinate {
        return transformHook(
            inputCoordinate,
            crsIdentifierForOutputCoordinateSystem
        )
    }

    abstract protected fun transformHook(
            inputCoordinate: CrsCoordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): CrsCoordinate


    override final fun transform(
            inputCoordinate: CrsCoordinate,
            epsgNumberForOutputCoordinateSystem: Int
    ): CrsTransformationResult {
        return transform(
            inputCoordinate,
                createFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
        )
    }

    override final fun transform(
            inputCoordinate: CrsCoordinate,
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