package com.programmerare.crsTransformations

class CrsIdentifier private constructor(val crsCode: String, val isEpsgCode: Boolean, val epsgNumber: Int) {

    // The created crsCode strings will be trimmed, and if they are "epsg" (or e.g. "ePsG") then they will be uppercased i.e. "EPSG"

    companion object {
        private const val EPSG_PREFIX_UPPERCASED = "EPSG:"

        private val regexWithEpsgDigitsWithinTheOnlyParenthesis: Regex

        init {
            regexWithEpsgDigitsWithinTheOnlyParenthesis = Regex("""\s*epsg:(\d+)\s*""", RegexOption.IGNORE_CASE)
        }

        @JvmStatic
        fun createFromCrsCode(crsCode: String): CrsIdentifier {
            // the three default values to uses, unless the input string will match
            // a regular expression with EPSG string
            var epsgNumber = -999999
            var isEpsgCode = false
            var crsIdentifierCode = crsCode.trim()
            if(crsIdentifierCode.isNullOrEmpty()) {
                throw java.lang.IllegalArgumentException("CRS code must be non-empty")
            }

            val matchGroupCollection: MatchGroupCollection? = regexWithEpsgDigitsWithinTheOnlyParenthesis.matchEntire(crsIdentifierCode)?.groups
            if(matchGroupCollection != null) {
                // as the name of the regexp indicates, digits should be matched within parenthesis
                // which means that the below could should always return an integer
                // and therefore "!!" should not cause any NullPointerException
                epsgNumber = matchGroupCollection.get(1)?.value!!.toInt()
                isEpsgCode = true
                crsIdentifierCode =  EPSG_PREFIX_UPPERCASED + epsgNumber
            }
            return CrsIdentifier(crsIdentifierCode, isEpsgCode, epsgNumber)
        }

        @JvmStatic
        fun createFromEpsgNumber(epsgNumber: Int): CrsIdentifier {
            return CrsIdentifier(crsCode =  EPSG_PREFIX_UPPERCASED + epsgNumber, isEpsgCode = true, epsgNumber = epsgNumber)
        }
    }
}