using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using BUD.Drainage.Revit.Addin.Entities;
using BUD.Tuna.Revit.Extensions.Extensions;

namespace BUD.Drainage.Revit.Addin.Entities;

internal static partial class IElementWrapperExtensions
{
    /// <summary>
    /// Translates the Revit element by a given vector.
    /// </summary>
    /// <param name="element">The element to be translated.</param>
    /// <param name="vector">The translation vector.</param>
    public static void Translate(this IElementWrapper<Element> element, XYZ vector)
    {
        ElementTransformUtils.MoveElement(element.Document, element.RvtObject.Id, vector);
    }

    /// <summary>
    /// Gets the parameter of the Revit element based on the <see cref="BuiltInParameter"/>.
    /// </summary>
    /// <param name="element">The element from which to get the parameter.</param>
    /// <param name="builtInParameter">The built-in parameter to retrieve.</param>
    /// <returns>The parameter of the element.</returns>
    public static Parameter GetParameter(this IElementWrapper<Element> element, BuiltInParameter builtInParameter)
    {
        return element.RvtObject.get_Parameter(builtInParameter);
    }

    /// <summary>
    /// Gets the parameter of the Revit element based on the <see cref="Autodesk.Revit.DB.ElementId"/>.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="parameterId"></param>
    /// <returns></returns>
    public static Parameter GetParameter(this IElementWrapper<Element> element, ElementId parameterId)
    {
        return element.RvtObject.GetParameter(parameterId);
    }

    /// <summary>
    /// Gets the parameter of the Revit element based on the parameter name.
    /// </summary>
    /// <param name="element">The element from which to get the parameter.</param>
    /// <param name="parameterName">The name of the parameter to retrieve.</param>
    /// <returns>The parameter of the element.</returns>
    public static Parameter GetParameter(this IElementWrapper<Element> element, string parameterName)
    {
        return element.RvtObject.LookupParameter(parameterName);
    }

    /// <summary>
    /// Sets the value of a built-in parameter for the Revit element.
    /// </summary>
    /// <param name="element">The element to set the parameter value for.</param>
    /// <param name="builtInParameter">The built-in parameter to set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value was set successfully; otherwise, false.</returns>
    public static bool SetParameterValue(this IElementWrapper<Element> element, BuiltInParameter builtInParameter, string value)
    {
        return element.GetParameter(builtInParameter).Set(value);
    }

    /// <summary>
    /// Sets the value of a built-in parameter for the Revit element.
    /// </summary>
    /// <param name="element">The element to set the parameter value for.</param>
    /// <param name="builtInParameter">The built-in parameter to set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value was set successfully; otherwise, false.</returns>
    public static bool SetParameterValue(this IElementWrapper<Element> element, BuiltInParameter builtInParameter, int value)
    {
        return element.GetParameter(builtInParameter).Set(value);
    }

    /// <summary>
    /// Sets the value of a built-in parameter for the Revit element.
    /// </summary>
    /// <param name="element">The element to set the parameter value for.</param>
    /// <param name="builtInParameter">The built-in parameter to set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value was set successfully; otherwise, false.</returns>
    public static bool SetParameterValue(this IElementWrapper<Element> element, BuiltInParameter builtInParameter, double value)
    {
        return element.GetParameter(builtInParameter).Set(value);
    }

    /// <summary>
    /// Sets the value of a built-in parameter for the Revit element.
    /// </summary>
    /// <param name="element">The element to set the parameter value for.</param>
    /// <param name="builtInParameter">The built-in parameter to set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value was set successfully; otherwise, false.</returns>
    public static bool SetParameterValue(this IElementWrapper<Element> element, BuiltInParameter builtInParameter, ElementId value)
    {
        return element.GetParameter(builtInParameter).Set(value);
    }


}
