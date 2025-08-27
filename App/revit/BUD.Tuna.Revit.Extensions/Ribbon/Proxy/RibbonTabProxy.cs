using Autodesk.Revit.UI;
using BUD.Tuna.Revit.Extensions.Ribbon.Abstraction;
using System;
using System.Collections.Generic;

namespace BUD.Tuna.Revit.Extensions.Ribbon.Proxy;

internal class RibbonTabProxy : IRibbonTab
{
    private readonly List<RibbonPanelProxy> _items = new();

    public string Title { get; internal set; }

    public string AppPath { get; set; }

    public UIApplication Application { get; internal set; }

    public IRibbonPanel AddRibbonPanel(string name, Action<IRibbonPanel> handle)
    {
        RibbonPanelProxy ribbonPanelProxy = new()
        {
            Parent = this,
            Title = name,
            OriginalObject = Application.CreateRibbonPanel(Title, name)
        };

        _items.Add(ribbonPanelProxy);

        handle.Invoke(ribbonPanelProxy);

        return ribbonPanelProxy;
    }

    public IEnumerable<IRibbonItem> GetItems() => _items;

    public List<RibbonPanel> GetRibbonPanels()
    {
        return Application.GetRibbonPanels(Title);
    }
}
