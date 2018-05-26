package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import java.lang.RuntimeException

final class CrsTransformationFacadeComposite private constructor(protected val compositeStrategy: CompositeStrategy) : CrsTransformationFacadeBase(), CrsTransformationFacade {

    override final fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): Coordinate {
        val transformResult = transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
        if(transformResult.isSuccess) {
            return transformResult.outputCoordinate
        }
        else {
            throw RuntimeException("Transformation failed")
        }
    }

    override final fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        val crsTransformationFacades = compositeStrategy.getAllTransformationFacadesInTheOrderTheyShouldBeInvoked()
        val list = mutableListOf<TransformResult>()
        var lastResultOrNullIfNoPrevious: TransformResult? = null
        for (facade: CrsTransformationFacade in crsTransformationFacades) {
            if(compositeStrategy.shouldInvokeNextFacade(list, lastResultOrNullIfNoPrevious, facade)) {
                val res = facade.transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
                list.add(res)
                lastResultOrNullIfNoPrevious = res
            }
            if(!compositeStrategy.shouldContinueIterationOfFacadesToInvoke(list, lastResultOrNullIfNoPrevious)) {
                break
            }
        }
        return compositeStrategy.calculateAggregatedResult(list, inputCoordinate, crsIdentifierForOutputCoordinateSystem, this)
    }

    companion object {
        @JvmStatic
        fun createCrsTransformationMedian(list: List<CrsTransformationFacade>): CrsTransformationFacadeComposite {
            return CrsTransformationFacadeComposite(
                CompositeStrategyForMedianValue(
                    list
                )
            )
        }

        @JvmStatic
        fun createCrsTransformationAverage(list: List<CrsTransformationFacade>): CrsTransformationFacadeComposite {
            return CrsTransformationFacadeComposite(
                CompositeStrategyForAverageValue(
                    list
                )
            )
        }

        @JvmStatic
        fun createCrsTransformationWeightedAverage(weightedFacades: List<Pair<CrsTransformationFacade, Double>>): CrsTransformationFacadeComposite {
            return CrsTransformationFacadeComposite(
                CompositeStrategyForWeightedAverageValue.createCompositeStrategyForWeightedAverageValue(weightedFacades)
            )
        }

        @JvmStatic
        fun createCrsTransformationWeightedAverageByReflection(weightedFacades: List<Pair<String, Double>>): CrsTransformationFacadeComposite {
            return CrsTransformationFacadeComposite(
                CompositeStrategyForWeightedAverageValue.createCompositeStrategyForWeightedAverageValueByReflection(weightedFacades)
            )
        }

        @JvmStatic
        fun createCrsTransformationChainOfResponsibility(list: List<CrsTransformationFacade>): CrsTransformationFacadeComposite {
            return CrsTransformationFacadeComposite(
                CompositeStrategyForChainOfResponsibility(
                    list
                )
            )
        }
    }
}