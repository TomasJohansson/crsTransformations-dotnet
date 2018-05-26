package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsTransformations.TransformResult

interface CompositeStrategy {
    fun getAllTransformationFacadesInTheOrderTheyShouldBeInvoked(): List<CrsTransformationFacade>

    fun shouldInvokeNextFacade(allResultsSoFar: List<TransformResult>, lastResultOrNullIfNoPrevious: TransformResult?, nextFacadeToInvoke: CrsTransformationFacade): Boolean

    fun shouldContinueIterationOfFacadesToInvoke(allResultsSoFar: List<TransformResult>, lastResultOrNullIfNoPrevious: TransformResult?) : Boolean

    fun calculateAggregatedResult(
        allResults: List<TransformResult>,
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
        crsTransformationFacadeThatCreatedTheResult: CrsTransformationFacade
    ): TransformResult
}