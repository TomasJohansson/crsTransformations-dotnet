package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.coordinate.Coordinate

interface TransformResult {
    val inputCoordinate: Coordinate
    val outputCoordinate: Coordinate
    val exception: Throwable?
    val isSuccess: Boolean

    /**
     * @return CrsTransformationAdapter the adapter which created the result.
     *  It may be useful when a composite adapter is returning a result aggregating many results
     *  and you want to figure out which result originated from which leaf adapter implementation.
     */
    val crsTransformationAdapterResultSource: CrsTransformationAdapter

    /**
     * Empty list if the transform implementation is a concrete "Leaf"
     * implementation, but if it is a composite/aggregating implementation
     * then all the individual "leaf" results are returned in this list.
     */
    val transformResultChildren: List<TransformResult>

    val resultsStatistic: ResultsStatistic

    /**
     * Convenience method intended for "Composite" implementations
     * to easy check that more than one implementation (the specified min number)
     * resulted in the same coordinate (within the specified delta value).
     * If false is returned then you can retrieve the ResultsStatistic object
     * to find the details regarding the differences.
     * The method is actually relevant to use only for aggregated transformations i.e. the "Composite" implementations.
     * However, there is also a reasonable behaviour for the "Leaf" implementations regarding the number of results (always 1)
     * and the "differences" in lat/long for the "different" implementations i.e. the "difference" should always be zero since there is only one implementation.
     * In other words, the method is meaningful only for the "Composite" implementations but the "Leaf" implementations should not cause exception to be thrown when using the method but instead logically expected behaviour.
     * @param minimumNumberOfSuccesfulResults specifies the minimum number of results for a results to be considered as reliable.
     *      Currently there are five implementations (though one of them can only handle coordinate system used in Sweden)
     *      so you will probably not want to use a value smaller than 4.
     * @param maxDeltaValueForXLongitudeAndYLatitude specifies the maximum difference in either x/Long or y/Lat to be considered as reliable.
     *      IMPORTANT: the unit for the delta value is the unit of the output/result coordinate.
     *      For example if you are using a projected coordinate system with x/Y values in meters then the value 1 (i.e. one meter)
     *      is fairly small, but the value 1 would be very big for "GPS" (WGS84) latitudes/longitudes.
     */
    fun isReliable(
        minimumNumberOfSuccesfulResults: Int,
        maxDeltaValueForXLongitudeAndYLatitude: Double
    ): Boolean
}