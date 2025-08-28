using Autodesk.Revit.DB;
using BUD.Drainage.Revit.Addin.Entities.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

internal class Rvt_Viewport : Rvt_Element<Viewport>
{
    public Rvt_Viewport(Viewport element) : base(element) { }

    public Rvt_OrientatedBoundingBox GetOrientedBoundingBox()
    {

        ViewSheet viewSheet = Document.GetElement(RvtObject.SheetId) as ViewSheet;
        var bbx = RvtObject.get_BoundingBox(viewSheet);

        Outline boxOutline = RvtObject.GetBoxOutline();

        return new Rvt_OrientatedBoundingBox(
            Transform.Identity,
            boxOutline.MaximumPoint,
            boxOutline.MinimumPoint);
    }
}
