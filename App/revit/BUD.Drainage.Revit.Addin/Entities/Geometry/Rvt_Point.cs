using Autodesk.Revit.DB;
using DSD.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BUD.Drainage.Revit.Addin.Entities.Geometry;

internal class Rvt_Point : DSD_Point
{
    public Rvt_Point() { }

    public static implicit operator XYZ(Rvt_Point point) => new XYZ(point.X, point.Y, point.Z);

    public static implicit operator Rvt_Point(XYZ point) => new Rvt_Point();
}
