package com.programmerare.crsCodeGeneration.constantsGenerator

class ConstantNameRenderer(var renderStrategy: RenderStrategy) : RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return renderStrategy.getValueForConstant(epsgNumber)
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return renderStrategy.getNameForConstant(crsName, areaName, epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return renderStrategy.getDataTypeForConstant()
    }

    private val fieldSeparator = "__"
}

interface RenderStrategy {
    fun getValueForConstant(epsgNumber: Int): String
    fun getDataTypeForConstant(): DataType
    fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String
}
abstract class RenderStrategyBase: RenderStrategy {
    protected val fieldSeparator = "__"

    protected fun getDataTypeForConstantInteger(): DataType {
        return DataType.INTEGER
    }
    protected fun getDataTypeForConstantString(): DataType {
        return DataType.STRING
    }
    protected fun getValueForConstantAsInteger(epsgNumber: Int): String {
        return epsgNumber.toString()
    }
    protected fun getValueForConstantAsString(epsgNumber: Int): String {
        return "\"EPSG:" + epsgNumber + "\""
    }
    protected fun getNameForConstantAdjusted(part1: String, part2: String, part3: String): String {
        val nameForConstant = part1 + fieldSeparator + part2 + fieldSeparator + part3
        return nameForConstant.adjusted()
    }
}
///////////////////////
class RenderStrategyNameAreaNumberInteger: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsInteger(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantInteger()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted(crsName, areaName, epsgNumber.toString())
    }
}

class RenderStrategyNameNumberAreaInteger: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsInteger(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantInteger()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted(crsName, epsgNumber.toString(), areaName)
    }
}

class RenderStrategyNumberNameAreaInteger: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsInteger(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantInteger()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted("_" + epsgNumber.toString(), crsName, areaName)
    }
}

class RenderStrategyNumberAreaNameInteger: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsInteger(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantInteger()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted("_" + epsgNumber.toString(), areaName, crsName)
    }
}


class RenderStrategyAreaNumberNameInteger: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsInteger(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantInteger()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted(areaName, epsgNumber.toString(), crsName)
    }
}

class RenderStrategyAreaNameNumberInteger: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsInteger(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantInteger()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted(areaName, crsName, epsgNumber.toString())
    }
}
////////////////////////
class RenderStrategyNameAreaNumberString: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsString(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantString()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted(crsName, areaName, epsgNumber.toString())
    }
}

class RenderStrategyNameNumberAreaString: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsString(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantString()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted(crsName, epsgNumber.toString(), areaName)
    }
}

class RenderStrategyNumberNameAreaString: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsString(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantString()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted("_" + epsgNumber.toString(), crsName, areaName)
    }
}

class RenderStrategyNumberAreaNameString: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsString(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantString()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted("_" + epsgNumber.toString(), areaName, crsName)
    }
}


class RenderStrategyAreaNumberNameString: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsString(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantString()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted(areaName, epsgNumber.toString(), crsName)
    }
}

class RenderStrategyAreaNameNumberString: RenderStrategyBase() , RenderStrategy {
    override fun getValueForConstant(epsgNumber: Int): String {
        return super.getValueForConstantAsString(epsgNumber)
    }

    override fun getDataTypeForConstant(): DataType {
        return super.getDataTypeForConstantString()
    }

    override fun getNameForConstant(crsName: String, areaName: String, epsgNumber: Int): String {
        return super.getNameForConstantAdjusted(areaName, crsName, epsgNumber.toString())
    }
}

fun String.adjusted(): String {
    return this.getUppercasedWithOnylValidCharacters()
}

fun String.getUppercasedWithOnylValidCharacters(): String {
    // the last regexp below just makes sure there are will not be more than two "_" in a row
    return this.toUpperCase().replace("[^a-zA-Z0-9_]".toRegex(), "_").replace("_{2,}".toRegex(), "__")
}