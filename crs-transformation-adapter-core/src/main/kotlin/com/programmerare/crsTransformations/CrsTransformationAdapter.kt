package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier

/**
 * The methods 'transformToCoordinate' can throw exception when transformation fails.
 * The methods 'transform' should always return an object,
 * but then you should check failures with 'TransformationResult.isSuccess'.
 *
 */
interface CrsTransformationAdapter {

    fun transformToCoordinate(
            inputCoordinate: CrsCoordinate,
            epsgNumberForOutputCoordinateSystem: Int
    ): CrsCoordinate

    fun transformToCoordinate(
            inputCoordinate: CrsCoordinate,
            crsCodeForOutputCoordinateSystem: String
    ): CrsCoordinate

    fun transformToCoordinate(
            inputCoordinate: CrsCoordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): CrsCoordinate


    fun transform(
            inputCoordinate: CrsCoordinate,
            epsgNumberForOutputCoordinateSystem: Int
    ): CrsTransformationResult

    fun transform(
            inputCoordinate: CrsCoordinate,
            crsCodeForOutputCoordinateSystem: String
    ): CrsTransformationResult

    fun transform(
            inputCoordinate: CrsCoordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): CrsTransformationResult

    /**
     * Should normally simply return the full class name (including the package name),
     * but when implementing test doubles (e.g. Mockito stub)
     * it should be implemented by defining different names
     * to simulate that different classes (implementations)
     * should have different weights.
     */
    fun getLongNameOfImplementation(): String

    /**
     * Should return the unique suffix part of the class name
     * i.e. the class name without the prefix which is common
     * for all implementations.
     */
    fun getShortNameOfImplementation(): String
}