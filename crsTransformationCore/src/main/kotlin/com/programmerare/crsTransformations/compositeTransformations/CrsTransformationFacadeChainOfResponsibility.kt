package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import java.util.List

class CrsTransformationFacadeChainOfResponsibility(crsTransformationFacades: List<CrsTransformationFacade>) : CrsTransformationFacadeBaseComposite(crsTransformationFacades) {

    override fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        for (facade: CrsTransformationFacade in crsTransformationFacades) {
            val res = facade.transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            if(res.isSuccess) {
                return res
            }
        }
        return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false)
    }
}