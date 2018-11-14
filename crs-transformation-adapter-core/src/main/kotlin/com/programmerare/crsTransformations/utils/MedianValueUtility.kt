package com.programmerare.crsTransformations.utils

/**
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 */
internal object MedianValueUtility {
    @JvmStatic
    fun getMedianValue(values: kotlin.collections.List<Double>): Double {
        val sortedDescending = values.sortedDescending()
        val middle = sortedDescending.size / 2
        return if (sortedDescending.size % 2 == 1) {
            sortedDescending.get(middle)
        } else {
            return (sortedDescending.get(middle-1) + sortedDescending.get(middle)) / 2;
        }
    }
}