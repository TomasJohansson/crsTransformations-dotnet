package com.programmerare.crsTransformations

interface TransformResult {
    val inputCoordinate: Coordinate
    val outputCoordinate: Coordinate
    val exception: Throwable?
    val isSuccess: Boolean
    val crsTransformationFacadeThatCreatedTheResult: CrsTransformationFacade
    val subResults: List<TransformResult>
}