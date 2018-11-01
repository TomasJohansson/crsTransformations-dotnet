package com.programmerare.crsConstants

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.*

class ConstantsGeneratedTest {

    @Test
    fun constantNumberTest() {
        // Note that the package names below are different (the part beginning with "constantsBy")
        val list = listOf(
            com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber._3006__SWEREF99_TM__SWEDEN,
            com.programmerare.crsConstants.constantsByNumberAreaName.v9_5_4.EpsgNumber._3006__SWEDEN__SWEREF99_TM,

            com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber.SWEDEN__SWEREF99_TM__3006,
            com.programmerare.crsConstants.constantsByAreaNumberName.v9_5_4.EpsgNumber.SWEDEN__3006__SWEREF99_TM,

            com.programmerare.crsConstants.constantsByNameAreaNumber.v9_5_4.EpsgNumber.SWEREF99_TM__SWEDEN__3006,
            com.programmerare.crsConstants.constantsByNameNumberArea.v9_5_4.EpsgNumber.SWEREF99_TM__3006__SWEDEN
        )
        assertAllElementsAreEqual(list)
    }

    @Test
    fun constantStringTest() {
        val list = listOf(
            com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgCode._3006__SWEREF99_TM__SWEDEN,
            com.programmerare.crsConstants.constantsByNumberAreaName.v9_5_4.EpsgCode._3006__SWEDEN__SWEREF99_TM,

            com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgCode.SWEDEN__SWEREF99_TM__3006,
            com.programmerare.crsConstants.constantsByAreaNumberName.v9_5_4.EpsgCode.SWEDEN__3006__SWEREF99_TM,

            com.programmerare.crsConstants.constantsByNameAreaNumber.v9_5_4.EpsgCode.SWEREF99_TM__SWEDEN__3006,
            com.programmerare.crsConstants.constantsByNameNumberArea.v9_5_4.EpsgCode.SWEREF99_TM__3006__SWEDEN
        )
        assertAllElementsAreEqual(list)
    }

    private fun <T> assertAllElementsAreEqual(list: List<T>) {
        val firstElement = list.get(0)
        for (i in 1 until list.size) {
            assertEquals(
                firstElement,
                list.get(i)
            )
        }
    }
}