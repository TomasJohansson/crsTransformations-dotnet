Imports Programmerare.CrsTransformations.Coordinate
Imports Programmerare.CrsTransformations.CompositeTransformations
Imports NUnit.Framework
Imports Programmerare.CrsConstants.ConstantsByAreaNameNumber.v10_036


Namespace Programmerare.CrsTransformations.NuGetClientExampleProjectVBnet

    'This VB.NET Class Is used As a code example For 
    'the libraries that should be retrieved With NuGet
    'i.e. Project Dependencies are Not used from this project.
    'There are three projects With similar code As below but 
    'For the different languages C# , F# And VB.NET :
    ' NuGetClientExampleProjectCSharpe ( Programmerare.CrsTransformations.NuGetClientExampleProjectCSharpe )
    ' NuGetClientExampleProjectFSharpe ( Programmerare.CrsTransformations.NuGetClientExampleProjectFSharpe )
    ' NuGetClientExampleProjectVBnet ( Programmerare.CrsTransformations.NuGetClientExampleProjectVBnet )
    'The main purpose For these projects Is To verify that NuGet retrieval works including To show that the assembly deployed To NuGet works fine.
    'For the F# And VB.NET projects, another purpose Is To verify usage Of the code from those languages.
    'Since the library Is implemented With F# it should Not be a surprise that the code works For F#
    'but for "completeness" I created a similar library for all these three languages, although 
    'it Is most interesting with the VB.NET code since the above VB.NET library 
    'Is the only code with VB.NET while there are lots of tests for C# 
    'in the project "Programmerare.CrsTransformations.Test"

    <TestFixture>
    Public Class VBnetExampleUsingNuGetDependencies

        Private Const SmallDeltaValue As Double = 0.0000001

        <SetUp>
        Public Sub Setup()
        End Sub

        <Test>
        Public Sub VBnetExampleCode()
            ' VB.NET type declarations are not needed by the compiler 
            ' but still used as below, just to illustrate what type is actually used
            Dim epsgWgs84 As Integer = EpsgNumber.WORLD__WGS_84__4326
            Dim epsgSweRef As Integer = EpsgNumber.SWEDEN__SWEREF99_TM__3006
            Assert.AreEqual(4326, epsgWgs84)
            Assert.AreEqual(3006, epsgSweRef)

            Dim centralStockholmWgs84 As CrsCoordinate = CrsCoordinateFactory.LatLon(59.330231, 18.059196, epsgWgs84)

            Dim crsTransformationAdapter As ICrsTransformationAdapter = CrsTransformationAdapterCompositeFactory.Create().CreateCrsTransformationMedian()
            Dim centralStockholmResultSweRef As CrsTransformationResult = crsTransformationAdapter.Transform(centralStockholmWgs84, epsgSweRef)

            Assert.IsNotNull(centralStockholmResultSweRef)
            Assert.IsTrue(centralStockholmResultSweRef.IsSuccess)
            Dim transformationResultChildren As IList(Of CrsTransformationResult) = centralStockholmResultSweRef.TransformationResultChildren

            ' Reason for the below assertion with value 3 :
            ' If the NuGet configuration includes all (currently three) adapter implementations, then the 
            ' above created 'Composite' implementation will below use all three 'leaf' implementations 
            ' and return a coordinate with a median longitude and a median latitude
            Assert.AreEqual(3, transformationResultChildren.Count)

            'Console.WriteLine(centralStockholmResultSweRef.OutputCoordinate)
            'Console output from the above code row: 
            'CrsCoordinate(xEastingLongitude=674032.357177155, yNorthingLatitude=6580821.99121561, crsIdentifier=CrsIdentifier(crsCode='EPSG:3006', isEpsgCode=True, epsgNumber=3006))
            Dim outputCoordinate As CrsCoordinate = centralStockholmResultSweRef.OutputCoordinate
            Assert.IsNotNull(outputCoordinate)
            Assert.AreEqual(674032.357177155, outputCoordinate.XEastingLongitude, SmallDeltaValue)
            Assert.AreEqual(6580821.99121561, outputCoordinate.YNorthingLatitude, SmallDeltaValue)

            Dim crsTransformationResultStatistic As CrsTransformationResultStatistic = centralStockholmResultSweRef.CrsTransformationResultStatistic
            'Dim medianCoordinate As CrsCoordinate = crsTransformationResultStatistic.CoordinateMedian
            ' The median values have already been tested above since we used 'CreateCrsTransformationMedian'
            ' for creating the main result.
            Dim averageCoordinate As CrsCoordinate = crsTransformationResultStatistic.CoordinateAverage
            Assert.AreEqual(674032.35716645606, averageCoordinate.XEastingLongitude, SmallDeltaValue)
            Assert.AreEqual(6580821.9921956062, averageCoordinate.YNorthingLatitude, SmallDeltaValue)

            Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable)
            Assert.AreEqual(3, crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults)
            ' Brackets needed below since the method name "Is" Is also a keyword in VB.NET
            Assert.That(crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude, [Is].LessThan(0.01))
            Assert.That(crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude, [Is].LessThan(0.01))

            ' "Reliable True" below since there should be three sucesful results 
            ' and the absolute value for the difference between longitudes and longitudes 
            ' should be less than 0.01
            Assert.IsTrue(
                centralStockholmResultSweRef.IsReliable(
                    3,   ' minimumNumberOfSuccesfulResults
                    0.01 ' maxDeltaValueForXLongitudeAndYLatitude
                )
            )

            ' "Reliable False" below because too extreme requirements of equal values for all the results 
            ' i.e. very small tolerance for differences 
            Assert.IsFalse(
                centralStockholmResultSweRef.IsReliable(
                    3,  ' minimumNumberOfSuccesfulResults
                    1.0E-21 ' maxDeltaValueForXLongitudeAndYLatitude
                )
            )

            ' "Reliable False" below because can not require 4 succesful values 
            ' when there are only 3 implementations
            Assert.IsFalse(
                centralStockholmResultSweRef.IsReliable(
                    4,   ' minimumNumberOfSuccesfulResults
                    0.01 ' maxDeltaValueForXLongitudeAndYLatitude
                )
            )

            Dim crsTransformationAdapterResultSource As ICrsTransformationAdapter = centralStockholmResultSweRef.CrsTransformationAdapterResultSource
            Dim adapteeType As CrsTransformationAdapteeType = crsTransformationAdapterResultSource.AdapteeType
            Assert.AreEqual(CrsTransformationAdapteeType.COMPOSITE_MEDIAN, adapteeType)
            Dim dict As New Dictionary(Of CrsTransformationAdapteeType, Boolean)
            For Each crsTransformationResultLeaf As CrsTransformationResult In transformationResultChildren
                Assert.IsTrue(crsTransformationResultLeaf.IsSuccess)
                dict.Add(crsTransformationResultLeaf.CrsTransformationAdapterResultSource.AdapteeType, True)

                ' Leafs always only have one result and thus there are zero difference between max and min result.
                ' Therefore the below assertion should succeed
                Assert.IsTrue(
                    crsTransformationResultLeaf.IsReliable(
                        1,  ' minimumNumberOfSuccesfulResults
                        1.0E-31 ' maxDeltaValueForXLongitudeAndYLatitude
                    )
                )

                ' Leafs always only have one result and thus the below tested method should return False
                Assert.IsFalse(
                    crsTransformationResultLeaf.IsReliable(
                        2,  ' minimumNumberOfSuccesfulResults
                        0.1 ' maxDeltaValueForXLongitudeAndYLatitude
                    )
                )

                Dim leafResultStatistic As CrsTransformationResultStatistic = crsTransformationResultLeaf.CrsTransformationResultStatistic
                Assert.IsTrue(leafResultStatistic.IsStatisticsAvailable)
                Assert.AreEqual(1, leafResultStatistic.NumberOfPotentiallySuccesfulResults)
                Assert.That(leafResultStatistic.MaxDifferenceForXEastingLongitude, [Is].LessThan(0.01))
                Assert.That(leafResultStatistic.MaxDifferenceForYNorthingLatitude, [Is].LessThan(0.01))
            Next
            Assert.AreEqual(3, dict.Count)
            Assert.IsTrue(dict.ContainsKey(CrsTransformationAdapteeType.LEAF_MIGHTY_LITTLE_GEODESY_1_0_2))
            Assert.IsTrue(dict.ContainsKey(CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_2_0_0_RC1))
            Assert.IsTrue(dict.ContainsKey(CrsTransformationAdapteeType.LEAF_PROJ_NET_2_0_0))

        End Sub

    End Class

End Namespace