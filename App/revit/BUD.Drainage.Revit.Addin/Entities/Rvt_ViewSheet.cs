using Autodesk.Revit.DB;
using BUD.Drainage.Revit.Addin.Entities;
using BUD.Drainage.Revit.Addin.Entities.Geometry;
using BUD.Tuna.Revit.Extensions.Collection;
using BUD.Tuna.Revit.Extensions.Constants;
using BUD.Tuna.Revit.Extensions.Extensions;
using BUD.Tuna.Revit.Extensions.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

internal class Rvt_ViewSheet : Rvt_Element<ViewSheet>
{
    public Rvt_ViewSheet(ViewSheet element) : base(element)
    {
        IList<Element> elements = element.GetElements(BuiltInCategories.TitleBlocks).ToElements();
        if (elements.Count != 0)
        {
            FamilyInstance familyInstance = elements.First() as FamilyInstance;
            TitleBlock = new Rvt_TitleBlock(familyInstance);
        }
    }

    public Rvt_TitleBlock TitleBlock { get; set; }

    public List<Rvt_Viewport> Viewports { get; } = new List<Rvt_Viewport>();

    public XYZ[] Split(int count)
    {
        XYZ[] split = [];

        Rvt_OrientatedBoundingBox box = TitleBlock.GetOrientedBoundingBox();

        //将包围盒缩小到空白布局区域，数值不确定，要根据用户的图框决定
        box.Max = box.Max.Translate(-box.YVector, 10.ConvertToFeet()).Translate(-box.XVector, 130.ConvertToFeet());
        box.Min = box.Min.Translate(box.XVector, 25.ConvertToFeet()).Translate(box.YVector, 10.ConvertToFeet());
        box.Initialize();

        XYZ centerPoint = (box.Max + box.Min) / 2;

        switch (count)
        {
            case 0:
                break;
            case 1:
                split = new XYZ[1];
                split[0] = centerPoint;
                break;
            case 2:
                split = new XYZ[2];
                split[0] = (box.Max.Subtract(box.XVector / 2) + box.Min) / 2;
                split[1] = (box.Min.Add(box.XVector / 2) + box.Max) / 2; ;
                break;
            case 3:
            case 4:
                split = new XYZ[4];
                XYZ cellXVector = box.XVector / 2;
                XYZ cellYVector = box.YVector / 2;
                split[0] = (box.Min.Add(cellYVector) + box.Min.Add(cellXVector).Add(cellYVector * 2)) / 2;
                split[1] = (box.Min.Add(cellYVector).Add(cellXVector) + box.Min.Add(cellXVector * 2).Add(cellYVector * 2)) / 2;
                split[2] = (box.Min + box.Min.Add(cellXVector).Add(cellYVector)) / 2;
                split[3] = (box.Min.Add(cellXVector) + box.Min.Add(cellXVector * 2).Add(cellYVector)) / 2;
                break;
            default:
                break;
        }

        return split;
    }

    public void AddView(View view, XYZ position)
    {
        Viewport viewport = Viewport.Create(Document, ElementId, view.Id, position);
        Viewports.Add(new Rvt_Viewport(viewport));
    }
}
