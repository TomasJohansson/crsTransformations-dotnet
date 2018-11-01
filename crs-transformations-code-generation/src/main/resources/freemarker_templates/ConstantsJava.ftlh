package ${nameOfPackageOrNamespace};

/**
<#list rowsForClassLevelComment as rowForClassLevelComment>
* ${rowForClassLevelComment}
</#list>
*/
public class ${nameOfClass} {
    <#t>
    <#list constants as constant>

    /**
    * ${constant.documentationCommentForConstant}
    * https://epsg.io/${constant.epsgNumber?c}
    */
    public final static ${constant.dataTypeForConstant.dataTypeName} ${constant.nameForConstant} = ${constant.valueForConstant?no_esc};
    </#list>
}