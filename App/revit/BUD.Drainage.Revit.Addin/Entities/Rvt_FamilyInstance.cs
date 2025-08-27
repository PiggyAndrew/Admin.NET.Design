using Autodesk.Revit.DB;
using BUD.Drainage.Revit.Addin.Entities.Geometry;
using BUD.Tuna.Revit.Extensions.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

internal class Rvt_FamilyInstance : Rvt_Element<FamilyInstance>
{
    public Rvt_FamilyInstance(FamilyInstance element) : base(element)
    {
        Transform = element.GetTransform();
        Location = element.Location;

        Number = this.GetParameter(BuiltInParameters.Element.Mark).AsString();
    }

    /// <summary>
    /// <see cref="BuiltInParameter.ALL_MODEL_MARK"/>
    /// </summary>
    public string Number { get; set; }

    public Transform Transform { get; set; }

    public Location Location { get; set; }

    public Rvt_OrientatedBoundingBox GetOrientedBoundingBox()
    {
        Transform transform = RvtObject.GetTransform();

        GeometryElement geometryElement = RvtObject.get_Geometry(new Options());
        GeometryInstance geometryInstance = geometryElement.First() as GeometryInstance;

        BoundingBoxXYZ bbx = geometryInstance.SymbolGeometry.GetBoundingBox();
        XYZ max = bbx.Max;
        XYZ min = bbx.Min;

        return new Rvt_OrientatedBoundingBox(
            transform,
            transform.OfPoint(max),
            transform.OfPoint(min));
    }
}
