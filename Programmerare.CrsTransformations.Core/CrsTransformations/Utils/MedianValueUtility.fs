namespace Programmerare.CrsTransformations.Utils
open System.Collections.Generic
module MedianValueUtility =
    let GetMedianValue(values: List<double>): double =
        values.Sort()
        let middle = values.Count / 2
        if (values.Count % 2 = 1) then
            values.[middle]
        else
            (values.[middle-1] + values.[middle]) / 2.0

// TODO: improve this median currently O(n log n) instead of O(n)
// https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
// https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Statistics/ArrayStatistics.cs
// the current below implementation is based on the Kotlin 
// implementation Programmerare.CrsTransformations.Core\crsTransformations\utils\MedianValueUtility.kt