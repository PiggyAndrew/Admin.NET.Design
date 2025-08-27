using Autodesk.Revit.DB;
using BUD.Drainage.Revit.Addin.Entities;
using BUD.Drainage.Revit.Addin.Entities.Geometry;
using BUD.Tuna.Revit.Extensions.Constants;
using BUD.Tuna.Revit.Extensions.Extensions;
using BUD.Tuna.Revit.Extensions.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

internal class Rvt_TitleBlock : Rvt_Element<FamilyInstance>
{
    public Rvt_TitleBlock(FamilyInstance element) : base(element)
    {
        if (element.Category.Id != BuiltInCategories.TitleBlocks)
        {
            throw new Exception();
        }

        LocationPoint locationPoint = (LocationPoint)element.Location;
        Origin = locationPoint.Point;

        Width = element.GetParameter(BuiltInParameters.Sheet.Width).AsDouble();
        Height = element.GetParameter(BuiltInParameters.Sheet.Height).AsDouble();
    }

    public double Width { get; set; }

    public double Height { get; set; }

    public XYZ Origin { get; set; }

    public Rvt_OrientatedBoundingBox GetOrientedBoundingBox()
    {
        return new Rvt_OrientatedBoundingBox(
            RvtObject.GetTransform(),
            Origin,
            Origin.Translate(-XYZ.BasisX, Width).Translate(-XYZ.BasisY, Height));
    }
}
