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
// This median implementation above: O(n log n) instead of O(n)
// However, since the number of values in the list is currently three
// (i.e. no more than three implementations to use the value for, then it is not a problem)
// It is also very unlikely that there will ever be even ten implementations 
// and the performance difference is therefore unlikely to ever be noticeable.
// Therefore I consider an improvement of the above implementation as a YAGNI
// https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
// https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Statistics/ArrayStatistics.cs
// The current below implementation is based on the Kotlin 
// implementation Programmerare.CrsTransformations.Core\crsTransformations\utils\MedianValueUtility.kt