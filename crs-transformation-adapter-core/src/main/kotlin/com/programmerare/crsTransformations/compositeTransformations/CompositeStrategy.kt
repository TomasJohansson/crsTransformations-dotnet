package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.coordinate.Coordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationResult

internal interface CompositeStrategy {
    fun getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked(): List<CrsTransformationAdapter>

    fun shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: CrsTransformationResult?) : Boolean

    fun calculateAggregatedResult(
            allResults: List<CrsTransformationResult>,
            inputCoordinate: Coordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
            crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter
    ): CrsTransformationResult
}