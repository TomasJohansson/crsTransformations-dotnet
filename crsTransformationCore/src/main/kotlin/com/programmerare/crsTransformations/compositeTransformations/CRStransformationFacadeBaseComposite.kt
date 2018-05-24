package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import java.lang.RuntimeException

// subclasses:
// CRStransformationFacadeChainOfResponsibility
// CRStransformationFacadeAverage
abstract class CRStransformationFacadeBaseComposite(protected val crsTransformationFacades: java.util.List<CRStransformationFacade>) : CRStransformationFacadeBase(), CRStransformationFacade {

    override final fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): Coordinate {
        val transformResult = transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
        if(transformResult.isSuccess) {
            return transformResult.outputCoordinate
        }
        else {
            throw RuntimeException("Transformation failed")
        }
    }
}
