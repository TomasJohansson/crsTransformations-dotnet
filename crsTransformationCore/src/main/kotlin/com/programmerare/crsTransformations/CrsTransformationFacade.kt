package com.programmerare.crsTransformations

interface CrsTransformationFacade {

    fun transform(
        inputCoordinate: Coordinate,
        epsgNumberForOutputCoordinateSystem: Int
    ): Coordinate

    fun transform(
        inputCoordinate: Coordinate,
        crsCodeForOutputCoordinateSystem: String
    ): Coordinate

    fun transform(
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
}