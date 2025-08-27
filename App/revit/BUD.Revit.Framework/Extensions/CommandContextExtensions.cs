using Autodesk.Revit.DB;
using System;
using System.IO;

/// <summary>
/// Extension methods for the CommandContext class.
/// </summary>
namespace BUD.Revit.Framework.Extensions;

public static class CommandContextExtensions
{
    /// <summary>
    /// Creates a new document from a template and opens it.
    /// </summary>
    /// <param name="commandContext">The command context to operate on.</param>
    /// <param name="templateFile">The path to the template file.</param>
    /// <param name="savePath">The path where the new document will be saved.</param>
    public static void CreateAndOpenDocument(this CommandContext commandContext, string templateFile, string savePath)
    {
        using Document document = commandContext.UIApplication.Application.NewProjectTemplateDocument(templateFile);
        document.SaveAs(savePath);
        document.Close();
        commandContext.UIApplication.OpenAndActivateDocument(savePath);
    }
}
