package com.programmerare.crsConstants

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.*

class ConstantsGeneratedTest {

    @Test
    fun constantNumberTest() {
        val list = listOf(
            com.programmerare.crsConstants.constantsByNumberNameArea.v9_3.EpsgNumber._3006__SWEREF99_TM__SWEDEN,
            com.programmerare.crsConstants.constantsByNumberAreaName.v9_3.EpsgNumber._3006__SWEDEN__SWEREF99_TM,

            com.programmerare.crsConstants.constantsByAreaNameNumber.v9_3.EpsgNumber.SWEDEN__SWEREF99_TM__3006,
            com.programmerare.crsConstants.constantsByAreaNumberName.v9_3.EpsgNumber.SWEDEN__3006__SWEREF99_TM,

            com.programmerare.crsConstants.constantsByNameAreaNumber.v9_3.EpsgNumber.SWEREF99_TM__SWEDEN__3006,
            com.programmerare.crsConstants.constantsByNameNumberArea.v9_3.EpsgNumber.SWEREF99_TM__3006__SWEDEN
        )

        for (i in list.indices) {
            var j = i+1
            for (j in i+1 .. list.size) {
                val v1 = list[i]
                val v2 = list[j]
                assertEquals(
                    v1,
                    v2
                )
            }
        }
    }

    @Test
    fun constantStringTest() {
        val list = listOf(
            com.programmerare.crsConstants.constantsByNumberNameArea.v9_3.EpsgCode._3006__SWEREF99_TM__SWEDEN,
            com.programmerare.crsConstants.constantsByNumberAreaName.v9_3.EpsgCode._3006__SWEDEN__SWEREF99_TM,

            com.programmerare.crsConstants.constantsByAreaNameNumber.v9_3.EpsgCode.SWEDEN__SWEREF99_TM__3006,
            com.programmerare.crsConstants.constantsByAreaNumberName.v9_3.EpsgCode.SWEDEN__3006__SWEREF99_TM,

            com.programmerare.crsConstants.constantsByNameAreaNumber.v9_3.EpsgCode.SWEREF99_TM__SWEDEN__3006,
            com.programmerare.crsConstants.constantsByNameNumberArea.v9_3.EpsgCode.SWEREF99_TM__3006__SWEDEN
        )

        for (i in list.indices) {
            var j = i+1
            while(j < list.size) {
                val v1 = list[i]
                val v2 = list[j]
                println("i "+ i)
                println("j "+ j)
                assertEquals(
                    v1, v2
                )
                j++
            }
        }
    }

}