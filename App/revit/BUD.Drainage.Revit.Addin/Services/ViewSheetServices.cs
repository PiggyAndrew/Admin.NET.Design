using Autodesk.Revit.DB;
using BUD.Drainage.Revit.Addin.Entities;
using BUD.Revit.Framework;
using BUD.Tuna.Revit.Extensions.Collection;
using BUD.Tuna.Revit.Extensions.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Services;

internal class ViewSheetServices
{
    private CommandContext _commandContext;

    public ViewSheetServices(CommandContext commandContext)
    {
        _commandContext = commandContext;
        TitleBlocks = commandContext.Document.GetElementTypes(BuiltInCategories.TitleBlocks).ToElementIds().ToArray();
    }

    public ElementId[] TitleBlocks { get; }

    public ViewSheet CreateViewSheet(params View[] views)
    {
        ViewSheet viewSheet = ViewSheet.Create(_commandContext.Document, TitleBlocks.First());

        Rvt_ViewSheet viewSheetProxy = new Rvt_ViewSheet(viewSheet);

        XYZ[] positions = viewSheetProxy.Split(views.Length);

        for (int i = 0; i < views.Length; i++)
        {
            View view = views[i];
            viewSheetProxy.AddView(view, positions[i]);
        }

        return viewSheet;
    }
}
