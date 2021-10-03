namespace Programmerare.CrsTransformations.NuGetClientExampleProjectFSharpe

open NUnit.Framework
open Programmerare.CrsConstants.ConstantsByAreaNameNumber.v10_036
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.CompositeTransformations
open System.Collections.Generic
open Programmerare.CrsTransformations

//This F# class is used as a code example for 
// the libraries that should be retrieved with NuGet
// i.e. Project Dependencies are not used from this project.
// There are three projects with similar code as below but 
// for the different languages C# , F# and VB.NET :
//  NuGetClientExampleProjectCSharpe ( Programmerare.CrsTransformations.NuGetClientExampleProjectCSharpe )
//  NuGetClientExampleProjectFSharpe ( Programmerare.CrsTransformations.NuGetClientExampleProjectFSharpe )
//  NuGetClientExampleProjectVBnet ( Programmerare.CrsTransformations.NuGetClientExampleProjectVBnet )
// The main purpose for these projects is to verify that NuGet retrieval works including to show that the assembly deployed to NuGet works fine.
// For the F# and VB.NET projects, another purpose is to verify usage of the code from those languages.
// Since the library is implemented with F# it should not be a surprise that the code works for F#
// but for "completeness" I created a similar library for all these three languages, although 
// it is most interesting with the VB.NET code since the above VB.NET library 
// is the only code with VB.NET while there are lots of tests for C# 
// in the project "Programmerare.CrsTransformations.Test"

[<TestFixture>]
type FSharpeExampleUsingNuGetDependencies () =

    [<Literal>] 
    let SmallDeltaValue = 0.0000001;

    [<SetUp>]
    member this.Setup () =
        ()

    [<Test>]
    member this.FSharpeExampleCode () =
        let epsgWgs84 = EpsgNumber.WORLD__WGS_84__4326
        let epsgSweRef = EpsgNumber.SWEDEN__SWEREF99_TM__3006
        Assert.AreEqual(4326, epsgWgs84)
        Assert.AreEqual(3006, epsgSweRef)

        // F# type declerations are not needed by the compiler 
        // but still used as below, just to illustrate what type is actually used
        let centralStockholmWgs84: CrsCoordinate  = CrsCoordinateFactory.LatLon(59.330231, 18.059196, epsgWgs84)

        let crsTransformationAdapter = CrsTransformationAdapterCompositeFactory.Create().CreateCrsTransformationMedian()
        // the below row is an alternative to the above row if explicit typing would be desired
        //let crsTransformationAdapter: ICrsTransformationAdapter = CrsTransformationAdapterCompositeFactory.Create().CreateCrsTransformationMedian() :> ICrsTransformationAdapter

        let centralStockholmResultSweRef = crsTransformationAdapter.Transform(centralStockholmWgs84, epsgSweRef)
        // the below row is an alternative to the above row if explicit typing would be desired
        //let centralStockholmResultSweRef: CrsTransformationResult = crsTransformationAdapter.Transform(centralStockholmWgs84, epsgSweRef)
        
        Assert.IsNotNull(centralStockholmResultSweRef)
        Assert.IsTrue(centralStockholmResultSweRef.IsSuccess)
        
        let transformationResultChildren = centralStockholmResultSweRef.TransformationResultChildren
        // the below row is an alternative to the above row if explicit typing would be desired
        //let transformationResultChildren: IList<CrsTransformationResult> = centralStockholmResultSweRef.TransformationResultChildren

        // Reason for the below assertion with value 3 :
        // If the NuGet configuration includes all (currently three) adapter implementations, then the 
        // above created 'Composite' implementation will below use all three 'leaf' implementations 
        // and return a coordinate with a median longitude and a median latitude
        Assert.AreEqual(3, transformationResultChildren.Count)

        // Console.WriteLine(centralStockholmResultSweRef.OutputCoordinate)
        // Console output from the above code row: 
        // CrsCoordinate(xEastingLongitude=674032.357177155, yNorthingLatitude=6580821.99121561, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))
        let outputCoordinate: CrsCoordinate = centralStockholmResultSweRef.OutputCoordinate
        Assert.IsNotNull(outputCoordinate)
        Assert.AreEqual(674032.357177155, outputCoordinate.XEastingLongitude, SmallDeltaValue)
        Assert.AreEqual(6580821.99121561, outputCoordinate.YNorthingLatitude, SmallDeltaValue)

        let crsTransformationResultStatistic: CrsTransformationResultStatistic = centralStockholmResultSweRef.CrsTransformationResultStatistic
        //let medianCoordinate = crsTransformationResultStatistic.CoordinateMedian
        // the median values have already been tested above since we used 'CreateCrsTransformationMedian'
        // for creating the main result.
        let averageCoordinate = crsTransformationResultStatistic.CoordinateAverage
        Assert.AreEqual(674032.35716645606, averageCoordinate.XEastingLongitude, SmallDeltaValue)
        Assert.AreEqual(6580821.9921956062, averageCoordinate.YNorthingLatitude, SmallDeltaValue)

        Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable)
        Assert.AreEqual(3, crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults)
        Assert.That(crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude, Is.LessThan(0.01))
        Assert.That(crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude, Is.LessThan(0.01))

        // "Reliable True" below since there should be three sucesful results 
        // and the absolute value for the difference between longitudes and longitudes 
        // should be less than 0.01
        Assert.IsTrue(
            centralStockholmResultSweRef.IsReliable(
                3,  // minimumNumberOfSuccesfulResults
                0.01// maxDeltaValueForXLongitudeAndYLatitude
            )                                                
        )

        // "Reliable False" below because too extreme requirements of equal values for all the results 
        // i.e. very small tolerance for differences 
        Assert.IsFalse(
            centralStockholmResultSweRef.IsReliable(
                3,  // minimumNumberOfSuccesfulResults
                0.000000000000000000001// maxDeltaValueForXLongitudeAndYLatitude
            )                                                
        )

        // "Reliable False" below because can not require 4 succesful values 
        // when there are only 3 implementations
        Assert.IsFalse(
            centralStockholmResultSweRef.IsReliable(
                4,  // minimumNumberOfSuccesfulResults
                0.01// maxDeltaValueForXLongitudeAndYLatitude
            )                                                
        )

        let crsTransformationAdapterResultSource: ICrsTransformationAdapter = centralStockholmResultSweRef.CrsTransformationAdapterResultSource
        let adapteeType: CrsTransformationAdapteeType = crsTransformationAdapterResultSource.AdapteeType
        Assert.AreEqual(CrsTransformationAdapteeType.COMPOSITE_MEDIAN, adapteeType)
        let dict = new Dictionary<CrsTransformationAdapteeType, bool>()

        for crsTransformationResultLeaf in transformationResultChildren do
        // the below row is an alternative to the above row if explicit typing would be desired
        //for crsTransformationResultLeaf: CrsTransformationResult in transformationResultChildren do

            Assert.IsTrue(crsTransformationResultLeaf.IsSuccess)
            dict.Add(crsTransformationResultLeaf.CrsTransformationAdapterResultSource.AdapteeType, true)

            // Leafs always only have one result and thus there are zero difference between max and min result.
            // Therefore the below assertion should succeed
            Assert.IsTrue(
                crsTransformationResultLeaf.IsReliable(
                    1,  // minimumNumberOfSuccesfulResults
                    0.0000000000000000000000000000001// maxDeltaValueForXLongitudeAndYLatitude
                )                                                
            )

            // Leafs always only have one result and thus the below tested method should return False
            Assert.IsFalse(
                crsTransformationResultLeaf.IsReliable(
                    2,  // minimumNumberOfSuccesfulResults
                    0.1// maxDeltaValueForXLongitudeAndYLatitude
                )                                                
            )

            let leafResultStatistic = crsTransformationResultLeaf.CrsTransformationResultStatistic
            // the below row is an alternative to the above row if explicit typing would be desired
            //let leafResultStatistic: CrsTransformationResultStatistic = crsTransformationResultLeaf.CrsTransformationResultStatistic
            Assert.IsTrue(leafResultStatistic.IsStatisticsAvailable)
            Assert.AreEqual(1, leafResultStatistic.NumberOfPotentiallySuccesfulResults)
            Assert.That(leafResultStatistic.MaxDifferenceForXEastingLongitude, Is.LessThan(0.01))
            Assert.That(leafResultStatistic.MaxDifferenceForYNorthingLatitude, Is.LessThan(0.01))

        Assert.AreEqual(3, dict.Count)
        Assert.IsTrue(dict.ContainsKey(CrsTransformationAdapteeType.LEAF_MIGHTY_LITTLE_GEODESY_1_0_2))
        Assert.IsTrue(dict.ContainsKey(CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_2_0_0_RC1))
        Assert.IsTrue(dict.ContainsKey(CrsTransformationAdapteeType.LEAF_PROJ_NET_2_0_0))
 
