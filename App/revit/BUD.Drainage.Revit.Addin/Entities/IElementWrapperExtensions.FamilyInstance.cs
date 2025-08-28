using Autodesk.Revit.DB;
using BUD.Drainage.Revit.Addin.Entities;
using BUD.Tuna.Revit.Extensions.Geometry;
using System;
using System.Linq;

/// <summary>
/// Extension methods for IElementWrapper specifically for FamilyInstance elements.
/// </summary>
internal static partial class IElementWrapperExtensions
{
    /// <summary>
    /// Gets references for the specified FamilyInstance reference type.
    /// </summary>
    /// <param name="element">The FamilyInstance element to get references from.</param>
    /// <param name="referenceType">The type of reference to get.</param>
    /// <returns>An array of references.</returns>
    public static Reference[] GetReferences(this IElementWrapper<FamilyInstance> element, FamilyInstanceReferenceType referenceType)
    {
        return [.. element.RvtObject.GetReferences(referenceType)];
    }

    /// <summary>
    /// Resolves faces based on the base view for the FamilyInstance.
    /// </summary>
    /// <param name="element">The FamilyInstance element to resolve faces for.</param>
    /// <param name="view">The view to use for resolving faces.</param>
    /// <returns>An array of faces.</returns>
    public static Face[] ResolveFacesBaseView(this IElementWrapper<FamilyInstance> element, View view)
    {
        return [.. element.RvtObject.ResolveFaces((o) =>
        {
            o.ComputeReferences = true;
            o.IncludeNonVisibleObjects = false;
            o.View = view;
        })];
    }
}
