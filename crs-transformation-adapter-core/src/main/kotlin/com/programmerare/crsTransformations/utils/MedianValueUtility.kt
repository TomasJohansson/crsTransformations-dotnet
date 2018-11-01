package com.programmerare.crsTransformations.utils

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