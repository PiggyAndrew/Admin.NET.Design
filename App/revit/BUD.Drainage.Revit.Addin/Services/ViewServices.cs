using Autodesk.Revit.DB;
using BUD.Drainage.Revit.Addin.Entities.Geometry;
using BUD.Drainage.Revit.Addin.Utilities;
using BUD.Revit.Framework;
using BUD.Tuna.Revit.Extensions.Collection;
using BUD.Tuna.Revit.Extensions.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Services;

internal class ViewServices
{
    public ViewServices(CommandContext commandContext)
    {
        Templates = commandContext.Document.GetElements<View>(v => v.IsTemplate).ToArray();
        ViewFamilyTypes = commandContext.Document.GetElementTypes<ViewFamilyType>().ToArray();


        #region

        //Transform originTransform = orientatedBoundingBox.Transform.Duplicate();
        //var xVector = (orientatedBoundingBox.Max - originTransform.Origin).DotProduct(originTransform.BasisX) * originTransform.BasisX;
        //originTransform.Origin = originTransform.Origin.Add(xVector);


        //var max = originTransform.Inverse.OfPoint(orientatedBoundingBox.Max);
        //var min = originTransform.Inverse.OfPoint(orientatedBoundingBox.Min);
        ////var min = originTransform.Inverse.OfPoint(orientatedBoundingBox.Min.Add(xVector));


        //var rotationTransform = Transform.Identity;
        //rotationTransform = rotationTransform.Multiply(Transform.CreateRotation(XYZ.BasisZ, -Math.PI / 2));
        //rotationTransform = rotationTransform.Multiply(Transform.CreateRotation(XYZ.BasisY, -Math.PI / 2));


        //XYZ transformedMax = rotationTransform.OfPoint(max);
        //XYZ transformedMin = rotationTransform.OfPoint(min);


        //originTransform = originTransform.Multiply(Transform.CreateRotation(XYZ.BasisY, Math.PI / 2));
        //originTransform = originTransform.Multiply(Transform.CreateRotation(XYZ.BasisZ, Math.PI / 2));

        //this.CommandContext.Document.TransientDisplay(Line.CreateBound(transformedMin, transformedMax));

        //ViewSection.CreateSection(this.CommandContext.Document, viewType.Id, new BoundingBoxXYZ()
        //{
        //    Max = transformedMax,
        //    Min = transformedMin,
        //    Transform = originTransform,
        //});

        #endregion
    }

    public View[] Templates { get; }

    public ViewFamilyType[] ViewFamilyTypes { get; }

    public View GetViewTemplate(string name)
    {
        return Templates.FirstOrDefault(t => t.Name == name);
    }

    /// <summary>
    /// Create view section
    /// </summary>
    /// <param name="document"></param>
    /// <param name="originTransform"></param>
    /// <param name="viewFamilyType"></param>
    /// <param name="firstPoint"></param>
    /// <param name="thirdPoint"></param>
    /// <returns></returns>
    private ViewSection CreateViewSection(Document document, Transform originTransform, ViewFamilyType viewFamilyType, XYZ firstPoint, XYZ thirdPoint)
    {
        var translationTransform = Transform.CreateTranslation(XYZ.Zero - originTransform.Origin);
        var originalTransform = originTransform.Multiply(translationTransform);

        var min = originalTransform.Inverse.OfPoint(firstPoint);
        var max = originalTransform.Inverse.OfPoint(thirdPoint);

        min = translationTransform.OfPoint(min);
        max = translationTransform.OfPoint(max);

        //create veiw section
        ViewSection viewSection = ViewSection.CreateSection(document, viewFamilyType.Id, new BoundingBoxXYZ()
        {
            Max = max,
            Min = min,
            Transform = originTransform,
        });

        return viewSection;
    }

    public ViewSection CreateViewSection(ViewFamilyType viewFamilyType, XYZ min, XYZ max, XYZ viewDirection, XYZ rightDirection)
    {
        Transform originTransform = TransformUtilities.CreateTransform((max + min) / 2, viewDirection, rightDirection);

        return CreateViewSection(viewFamilyType.Document, originTransform, viewFamilyType, min, max);
    }

    public ViewSection CreateViewSection(ViewFamilyType viewFamilyType, XYZ firstPoint, XYZ secondPoint, XYZ thirdPoint)
    {
        XYZ viewDirection = thirdPoint - secondPoint;
        XYZ rightDirection = secondPoint - new XYZ(firstPoint.X, firstPoint.Y, thirdPoint.Z);


        Transform originTransform = TransformUtilities.CreateTransform((thirdPoint + firstPoint) / 2, viewDirection, rightDirection);

        return CreateViewSection(viewFamilyType.Document, originTransform, viewFamilyType, firstPoint, thirdPoint);
    }

    public ViewSection CreateViewSection(ViewFamilyType viewFamilyType, Transform originTransform, Rvt_OrientatedBoundingBox orientatedBoundingBox, params XYZ[] rotationVectors)
    {
        var max = originTransform.Inverse.OfPoint(orientatedBoundingBox.Max);
        var min = originTransform.Inverse.OfPoint(orientatedBoundingBox.Min);

        var rotationTransform = Transform.Identity;
        for (int i = rotationVectors.Length - 1; i >= 0; i--)
        {
            rotationTransform = rotationTransform.Multiply(Transform.CreateRotation(rotationVectors[i], -Math.PI / 2));
        }


        XYZ transformedMax = rotationTransform.OfPoint(max);
        XYZ transformedMin = rotationTransform.OfPoint(min);

        var points = GetPoints(transformedMax, transformedMin);

        for (int i = 0; i < rotationVectors.Length; i++)
        {
            originTransform = originTransform.Multiply(Transform.CreateRotation(rotationVectors[i], Math.PI / 2));
        }

        viewFamilyType.Document.TransientDisplay(Line.CreateBound(transformedMin, transformedMax));
        return ViewSection.CreateSection(viewFamilyType.Document, viewFamilyType.Id, new BoundingBoxXYZ()
        {
            Max = points.Max,
            Min = points.Min,
            Transform = originTransform,
        });

        (XYZ Max, XYZ Min) GetPoints(XYZ max, XYZ min)
        {
            return new()
            {
                Max = new XYZ(max.X, max.Y, Math.Max(max.Z, min.Z)),
                Min = new XYZ(min.X, min.Y, Math.Min(min.Z, max.Z)),
            };
        }
    }
}
