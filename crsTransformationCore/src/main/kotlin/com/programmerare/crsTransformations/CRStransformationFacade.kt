package com.programmerare.crsTransformations

interface CRStransformationFacade {

    fun transform(
        inputCoordinate: Coordinate,
        epsgNumberForOutputCoordinateSystem: Int
    ): Coordinate

}