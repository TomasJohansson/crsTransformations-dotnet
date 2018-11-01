package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.coordinate.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.TransformResult

internal interface CompositeStrategy {
    fun getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked(): List<CrsTransformationAdapter>

    fun shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: TransformResult?) : Boolean

    fun calculateAggregatedResult(
            allResults: List<TransformResult>,
            inputCoordinate: Coordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
            crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter
    ): TransformResult
}