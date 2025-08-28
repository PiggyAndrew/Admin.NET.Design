using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

/// <summary>
/// Defines an interface for wrapping elements.
/// </summary>
/// <typeparam name="T">The type of the Revit element.</typeparam>
internal interface IElementWrapper<out T> : IElementWrapper, IEquatable<IElementWrapper> where T : Element
{
    /// <summary>
    /// The native Revit object.
    /// </summary>
    T RvtObject { get; }
}
