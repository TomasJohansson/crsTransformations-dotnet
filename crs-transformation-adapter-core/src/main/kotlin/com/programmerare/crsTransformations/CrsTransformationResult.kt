package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.CrsCoordinate

// TODO: remove this interface

/**
 * Interface used as result type from the transform method of the adapter interface.  
 * @see CrsTransformationAdapter
 */
interface CrsTransformationResult {
    /**
     * The input coordinate used in the transform that return the result object. 
     */
    val inputCoordinate: CrsCoordinate

    /**
     * The coordinate which is the result from the transform.  
     *
     * Depending on the adapter implementation, the output coordinate 
     * can either be a direct result from one specific 'leaf' adaptee implementation  
     * or it can be an aggregated result (i.e. median or average) from 
     * a 'composite' implementation.
     */    
    val outputCoordinate: CrsCoordinate

    /**
     * Either null or an exception depending on whether or not 
     * the transform resulted in an exception being thrown.
     */
    val exception: Throwable?

    /**
     * True if the transform was successful or false if it failed.
     * Note that "successful" does not necessarily mean that the 
     * result is correct but an exception was not thrown 
     * and the result was not "NaN" (Not a Number).
     */
    val isSuccess: Boolean

    /**
     * @return CrsTransformationAdapter the adapter which created the result.
     * 
     *  It may be useful when a composite adapter is returning a result aggregating many results
     *  and you want to figure out which result originated from which leaf adapter implementation.
     */
    val crsTransformationAdapterResultSource: CrsTransformationAdapter

    /**
     * Empty list if the transform implementation is a concrete "Leaf"
     * implementation, but if it is a composite/aggregating implementation
     * then all the individual "leaf" results are returned in this list.
     */
    val transformationResultChildren: List<CrsTransformationResult>

    /**
     * An object with conveniently available aggregating information about the 
     * results for the different implementations, which is useful for composite implementations.
     * For a leaf implementation this method is not meaningful.
     * It is a convenience method in the sense that the information provided 
     * can be calculated from client code by iterating the leafs/children of a composite.
     * @CrsTransformationResultStatistic
     */
    val crsTransformationResultStatistic: CrsTransformationResultStatistic

    /**
     * Convenience method intended for "Composite" implementations
     * to easy check that more than one implementation (the specified min number)
     * resulted in the same coordinate (within the specified delta value).  
     * 
     * If false is returned then you can retrieve the CrsTransformationResultStatistic object
     * to find the details regarding the differences.
     * 
     * The method is actually relevant to use only for aggregated transformations i.e. the "Composite" implementations.
     * 
     * However, there is also a reasonable behaviour for the "Leaf" implementations 
     * regarding the number of results (always 1) and the "differences" in lat/long for the "different" 
     * implementations i.e. the "difference" should always be zero since there is only one implementation.
     * 
     * In other words, the method is meaningful only for the "Composite" implementations 
     * but the "Leaf" implementations should not cause exception to be thrown when using 
     * the method but instead logically expected behaviour.
     * @param minimumNumberOfSuccesfulResults specifies the minimum number of results for a results to be considered as reliable.
     *      Currently there are five implementations (though one of them can only handle coordinate system used in Sweden)
     *      so you will probably not want to use a value smaller than 4.
     * @param maxDeltaValueForXLongitudeAndYLatitude specifies the maximum difference in either x/Long or y/Lat to be considered as reliable.
     *      IMPORTANT note: the unit for the delta value is the unit of the output/result coordinate.
     *      For example if you are using a projected coordinate system with x/Y values in meters then the value 1 (i.e. one meter)
     *      is fairly small, but the value 1 would be very big for "GPS" (WGS84) latitude/longitude values.
     */
    fun isReliable(
        minimumNumberOfSuccesfulResults: Int,
        maxDeltaValueForXLongitudeAndYLatitude: Double
    ): Boolean
}