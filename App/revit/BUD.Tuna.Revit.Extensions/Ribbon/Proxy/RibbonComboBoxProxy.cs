using Autodesk.Revit.UI;
using BUD.Tuna.Revit.Extensions.Ribbon;
using BUD.Tuna.Revit.Extensions.Ribbon.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Ribbon.Proxy;

internal class RibbonComboBoxProxy : RibbonElementProxy<ComboBox>, IRibbonComboBox
{
    Action<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs> _handle;
    public RibbonItemType Type => RibbonItemType.ComboBox;

    public string Name { get; set; }

    public RibbonComboBoxProxy(ComboBox comboBox)
    {
        OriginalObject = comboBox;
        OriginalObject.CurrentChanged += OriginalObject_CurrentChanged;
    }

    public IRibbonComboBox AddItem(string title)
    {
        ComboBoxMemberData comboBoxMemberData = new ComboBoxMemberData(title, title);
        OriginalObject.AddItem(comboBoxMemberData);

        return this;
    }

    public IRibbonComboBox AddItems(params string[] titles)
    {
        foreach (var item in titles)
        {
            AddItem(item);
        }
        return this;
    }

    public IRibbonComboBox AddSeparator()
    {
        OriginalObject.AddSeparator();
        return this;
    }

    public void OnSelectedChanged(Action<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs> handle)
    {
        _handle = handle;
    }

    private void OriginalObject_CurrentChanged(object sender, Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs e)
    {
        _handle?.Invoke(e);
    }
}

