package com.programmerare.crsTransformations

interface TransformResult {
    val inputCoordinate: Coordinate
    val outputCoordinate: Coordinate
    val exception: Throwable?
    val isSuccess: Boolean
    val crsTransformationFacadeThatCreatedTheResult: CrsTransformationFacade

    /**
     * Empty list if the transform implementation is a concrete "Leaf"
     * implementation, but if it is a composite/aggregating implementation
     * then all the individual "leaf" results are returned in this list.
     */
    val subResults: List<TransformResult>

    val resultsStatistic: ResultsStatistic
}