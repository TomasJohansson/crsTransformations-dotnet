using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_7;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.CompositeTransformations;

namespace Programmerare.CrsTransformations {

[TestFixture]
public abstract class CrsTransformationTestBase {

    protected readonly int epsgNumberForWgs84         = EpsgNumber.WORLD__WGS_84__4326;
    protected readonly int epsgNumberForSweref99TM    = EpsgNumber.SWEDEN__SWEREF99_TM__3006;
    protected readonly int epsgNumberForRT90          = EpsgNumber.SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021;

    protected List<ICrsTransformationAdapter> crsTransformationAdapterLeafImplementations;
    protected List<ICrsTransformationAdapter> crsTransformationAdapterCompositeImplementations;
    protected List<ICrsTransformationAdapter> crsTransformationAdapterImplementations;

    protected CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;

    private CrsTransformationAdapterWeightFactory weightFactory;

    [SetUp]
    public void SetUp() {
        weightFactory = CrsTransformationAdapterWeightFactory.Create();

        crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();

        crsTransformationAdapterLeafImplementations = new List<ICrsTransformationAdapter>{
            new CrsTransformationAdapterDotSpatial(),
            new CrsTransformationAdapterProjNet(),
            new CrsTransformationAdapterMightyLittleGeodesy()
        };

        crsTransformationAdapterCompositeImplementations = new List<ICrsTransformationAdapter>{
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage(),
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian(),
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(),
            crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(new List<CrsTransformationAdapterWeight>{
                weightFactory.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), 51.0),
                weightFactory.CreateFromInstance(new CrsTransformationAdapterProjNet(), 52.0),
                weightFactory.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), 53.0)
            })
        };
        crsTransformationAdapterImplementations = new List<ICrsTransformationAdapter>();
        crsTransformationAdapterImplementations.AddRange(crsTransformationAdapterLeafImplementations);
        crsTransformationAdapterImplementations.AddRange(crsTransformationAdapterCompositeImplementations);
    }
} // the test class ends here
} // namespace ends here