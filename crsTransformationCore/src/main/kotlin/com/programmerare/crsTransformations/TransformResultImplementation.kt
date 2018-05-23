package com.programmerare.crsTransformations

class TransformResultImplementation(
    override val inputCoordinate: Coordinate,
    override val outputCoordinate: Coordinate,
    override val exception: Exception?,
    override val isSuccess: Boolean
): TransformResult {
}