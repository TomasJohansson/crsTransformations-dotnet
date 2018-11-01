package com.programmerare.crsCodeGeneration.utils

object JavaPackageToCSharpeNamespaceConverter {

    private val regexMatchingVersionPart = Regex("""v[_\d]+""")

    /**
     * @param nameOfJavaPackage e.g. "com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4"
     * @return something like "Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4"
     */
    fun getAsNameOfCSharpeNameSpace(nameOfJavaPackage: String): String {
        val parts = nameOfJavaPackage.split('.')
        val sb = StringBuilder()
        for(i in parts.indices) {
            val part = parts[i]
            if(i == 0 && part == "com") continue;
            if(!sb.isEmpty()) {
                sb.append(".")
            }
            if(regexMatchingVersionPart.matches(part)) {
                sb.append(part)
            }
            else {
                sb.append(part.capitalize())
            }
        }
        return sb.toString()
    }
}
