package com.programmerare.crsTransformations

data class CrsIdentifier private constructor(
    val crsCode: String,
    val isEpsgCode: Boolean,
    val epsgNumber: Int
) {

    companion object {
        private const val EPSG_PREFIX_UPPERCASED = "EPSG:"
        private const val LENGTH_OF_EPSG_PREFIX = EPSG_PREFIX_UPPERCASED.length

        // The crsCode string will become trimmed, and if it is
        // "epsg" (or e.g. something like "ePsG") then it will be uppercased i.e. "EPSG"
        @JvmStatic
        fun createFromCrsCode(crsCode: String): CrsIdentifier {
            // these two default values will be used, unless the crsCode parameter is an EPSG string
            var epsgNumber = 0
            var isEpsgCode = false

            if(crsCode.isNullOrBlank()) {
                throw java.lang.IllegalArgumentException("CRS code must be non-empty")
            }
            var crsIdentifierCode = crsCode.trim() // but does not uppercase here (unless EPSG below)

            if(crsIdentifierCode.toUpperCase().startsWith(EPSG_PREFIX_UPPERCASED)) {
                val nonEpsgPartOfString = crsIdentifierCode.substring(LENGTH_OF_EPSG_PREFIX);
                val epsgNumberOrNull = nonEpsgPartOfString.toIntOrNull()
                if(epsgNumberOrNull != null) {
                    epsgNumber = epsgNumberOrNull
                    isEpsgCode = true
                    crsIdentifierCode = crsIdentifierCode.toUpperCase()
                }
            }
            return CrsIdentifier(crsIdentifierCode, isEpsgCode, epsgNumber)
        }

        @JvmStatic
        fun createFromEpsgNumber(epsgNumber: Int): CrsIdentifier {
            return CrsIdentifier(
                crsCode =  EPSG_PREFIX_UPPERCASED + epsgNumber,
                isEpsgCode = true,
                epsgNumber = epsgNumber
            )
        }
    }
}