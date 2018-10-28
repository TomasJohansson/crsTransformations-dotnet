package com.programmerare.crsTransformations

interface CrsTransformationFacade {

    fun transformToCoordinate(
        inputCoordinate: Coordinate,
        epsgNumberForOutputCoordinateSystem: Int
    ): Coordinate

    fun transformToCoordinate(
        inputCoordinate: Coordinate,
        crsCodeForOutputCoordinateSystem: String
    ): Coordinate

    fun transformToCoordinate(
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): Coordinate


    fun transformToResultObject(
        inputCoordinate: Coordinate,
        epsgNumberForOutputCoordinateSystem: Int
    ): TransformResult

    fun transformToResultObject(
        inputCoordinate: Coordinate,
        crsCodeForOutputCoordinateSystem: String
    ): TransformResult

    fun transformToResultObject(
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): TransformResult

    /**
     * Should normally simply return the class name,
     * but when implementing test doubles (e.g. Mockito stub)
     * it should be implemented by defining different names
     * to simulate that different classes (implementations)
     * should have different weights.
     */
    fun getNameOfImplementation(): String
}