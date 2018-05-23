package com.programmerare.crsTransformations

interface TransformResult {
    val inputCoordinate: Coordinate
    val outputCoordinate: Coordinate
    val exception: Exception?
    val isSuccess: Boolean
}