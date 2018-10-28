package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsTransformationFacade

internal abstract class CompositeStrategyBase
    (private val crsTransformationFacades: List<CrsTransformationFacade>)
    : CompositeStrategy {

    override final fun getAllTransformationFacadesInTheOrderTheyShouldBeInvoked(): List<CrsTransformationFacade> {
        return crsTransformationFacades
    }
}