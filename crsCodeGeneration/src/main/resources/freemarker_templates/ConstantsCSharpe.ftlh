namespace ${nameOfPackageOrNamespace} {

    /// <summary>
    <#list rowsForClassLevelComment as rowForClassLevelComment>
    /// ${rowForClassLevelComment}
    </#list>
    /// </summary>
    public sealed class ${nameOfClass} {
        <#t>
        <#list constants as constant>

        /// <summary>
        /// ${constant.documentationCommentForConstant}
        /// https://epsg.io/${constant.epsgNumber?c}
        /// </summary>
        public const ${constant.dataTypeForConstant.dataTypeName} ${constant.nameForConstant} = ${constant.valueForConstant?no_esc};
        </#list>
    }

}