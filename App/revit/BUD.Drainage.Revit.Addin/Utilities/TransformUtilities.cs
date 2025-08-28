using Autodesk.Revit.DB;
using BUD.Tuna.Revit.Extensions.Collection;
using BUD.Tuna.Revit.Extensions.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Utilities;

internal class TransformUtilities
{
    public static Transform CreateTransform(XYZ origin, XYZ zVector, XYZ yVector)
    {
        Transform originTransform = Transform.Identity;

        originTransform.Origin = origin;
        originTransform.BasisZ = zVector.Normalize();
        originTransform.BasisY = yVector.Normalize();
        originTransform.BasisX = originTransform.BasisY.CrossProduct(originTransform.BasisZ).Normalize();

        return originTransform;
    }

    public static Transform CreateTransform(XYZ origin, XYZ zVector, XYZ yVector, XYZ xVector)
    {
        Transform originTransform = Transform.Identity;

        originTransform.Origin = origin;
        originTransform.BasisZ = zVector.Normalize();
        originTransform.BasisY = yVector.Normalize();
        originTransform.BasisX = xVector.Normalize();

        return originTransform;
    }

    public static void Display(Document document, Transform transform)
    {
        //紫色
        var gs1 = document.GetElements<GraphicsStyle>().FirstOrDefault();
        //蓝色
        var gs2 = document.GetElements<GraphicsStyle>().FirstOrDefault();
        //绿色
        var gs3 = document.GetElements<GraphicsStyle>().FirstOrDefault();

        document.TransientDisplay(Point.Create(transform.Origin));
        document.TransientDisplay(LineUtilities.CreateLine(transform.Origin, transform.BasisX), gs1.Id);
        document.TransientDisplay(LineUtilities.CreateLine(transform.Origin, transform.BasisY * 2), gs2.Id);
        document.TransientDisplay(LineUtilities.CreateLine(transform.Origin, transform.BasisZ * 3), gs3.Id);
    }
}
