using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

/// <summary>
/// Interface representing a wrapper for Revit elements.
/// This interface provides access to the element's name, ID, and the document it belongs to.
/// </summary>
internal interface IElementWrapper
{
    /// <summary>
    /// Name of the element.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// ID of the element.
    /// </summary>
    ElementId ElementId { get; }

    /// <summary>
    /// Document where the element is located.
    /// </summary>
    Document Document { get; }
}
