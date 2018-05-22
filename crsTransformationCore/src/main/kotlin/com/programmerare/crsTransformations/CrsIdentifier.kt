package com.programmerare.crsTransformations

class CrsIdentifier private constructor(val crsCode: String) {
    companion object {
        @JvmStatic
        fun createFromCrsCode(crsCode: String): CrsIdentifier {
            return CrsIdentifier(crsCode)
        }

        private const val EPSG_PREFIX = "EPSG:"

        @JvmStatic
        fun createFromEpsgNumber(epsgNumber: Int): CrsIdentifier {
            return CrsIdentifier(EPSG_PREFIX + epsgNumber)
        }
    }
}