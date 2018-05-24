package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CRStransformationFacade
import java.util.List

class CRStransformationFacadeChainOfResponsibility(crsTransformationFacades: List<CRStransformationFacade>) : CRStransformationFacadeBaseComposite(crsTransformationFacades) {
}