package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import java.util.List

class CRStransformationFacadeChainOfResponsibility(crsTransformationFacades: List<CRStransformationFacade>) : CRStransformationFacadeBaseComposite(crsTransformationFacades) {

    override fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        for (facade: CRStransformationFacade in crsTransformationFacades) {
            val res = facade.transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            if(res.isSuccess) {
                return res
            }
        }
        return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false)
    }
}