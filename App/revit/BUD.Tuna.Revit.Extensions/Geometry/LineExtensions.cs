using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Geometry;

public static class LineExtensions
{
    public static Line Offset(this Line line, double distance, XYZ vector)
    {
        XYZ startPoint = line.GetEndPoint(0);
        XYZ endPoint = line.GetEndPoint(1);

        startPoint = startPoint.Translate(vector, distance);
        endPoint = endPoint.Translate(vector, distance);

        return Line.CreateBound(startPoint, endPoint);
    }
}

