using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities.Geometry;

static class Rvt_OrientatedBoundingBoxExtensions
{
    public static Line GetLine(this Rvt_OrientatedBoundingBox orientatedBoundingBox)
    {
        return Line.CreateBound(orientatedBoundingBox.Max, orientatedBoundingBox.Min);
    }
}

