using Autodesk.Revit.DB;
using DSD.Toolkit.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

/// <summary>
/// Base class for Revit <see cref="Autodesk.Revit.DB.Element"/>
/// </summary>
/// <typeparam name="T"></typeparam>
internal class Rvt_Element<T> : ObservableObject, IElementWrapper<T> where T : Element
{
    public Rvt_Element(T element)
    {
        if (element == null)
        {
            throw new ArgumentNullException();
        }

        RvtObject = element;
        Document = element.Document;
        ElementId = element.Id;

        Name = element.Name;
    }

    /// <summary>
    /// Name of element
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// ID of element
    /// </summary>
    public ElementId ElementId { get; } = ElementId.InvalidElementId;

    /// <summary>
    /// Document containing the element
    /// </summary>
    public Document Document { get; }

    /// <summary>
    /// Native Revit object
    /// </summary>
    public T RvtObject { get; }

    /// <inheritdoc/>
    public bool Equals(IElementWrapper other) => ElementId == other?.ElementId;

    /// <summary>
    /// Implicitly convert proxy object to element
    /// </summary>
    /// <param name="elementProxy"></param>
    public static implicit operator T(Rvt_Element<T> elementProxy)
    {
        return elementProxy.RvtObject;
    }

    /// <summary>
    /// Implicitly convert element to proxy object
    /// </summary>
    /// <param name="element"></param>
    public static implicit operator Rvt_Element<T>(T element)
    {
        return new Rvt_Element<T>(element);
    }
}
