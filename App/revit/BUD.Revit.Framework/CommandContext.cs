using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Revit.Framework;

/// <summary>
/// Represents the context for commands in Revit, managing the active document and UI document.
/// </summary>
public class CommandContext : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the CommandContext class with the specified UIApplication.
    /// </summary>
    /// <param name="uIApplication">The Revit application UI instance.</param>
    public CommandContext(UIApplication uIApplication)
    {
        UIApplication = uIApplication;
        ChangeDocumentContext(UIApplication.ActiveUIDocument);
    }

    /// <summary>
    /// Changes the document context based on the provided UIDocument.
    /// </summary>
    /// <param name="uiDocument">The UIDocument to set as the current document.</param>
    private void ChangeDocumentContext(UIDocument uiDocument)
    {
        UIDocument = uiDocument;
        if (UIDocument == null)
        {
            IsVaildDocument = false;
            Document = null;
            return;
        }

        Document = UIDocument.Document;
        IsVaildDocument = true;
    }

    /// <summary>
    /// Attaches event handlers for document events.
    /// </summary>
    internal void Attach()
    {
        UIApplication.Application.DocumentOpened += Application_DocumentOpened;
        UIApplication.Application.DocumentCreated += Application_DocumentCreated;
        UIApplication.Application.DocumentClosed += Application_DocumentClosed;
    }

    /// <summary>
    /// Handles the DocumentClosed event to update the document context.
    /// </summary>
    private void Application_DocumentClosed(object sender, Autodesk.Revit.DB.Events.DocumentClosedEventArgs e)
    {
        Document = null;
        UIDocument = null;
        IsVaildDocument = false;
        ChangeDocumentContext(UIApplication.ActiveUIDocument);
    }

    /// <summary>
    /// Handles the DocumentOpened event to update the document context.
    /// </summary>
    private void Application_DocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
    {
        if (e.Status != Autodesk.Revit.DB.Events.RevitAPIEventStatus.Succeeded)
        {
            return;
        }

        ChangeDocumentContext(new UIDocument(e.Document));
    }

    /// <summary>
    /// Handles the DocumentCreated event to update the document context.
    /// </summary>
    private void Application_DocumentCreated(object sender, Autodesk.Revit.DB.Events.DocumentCreatedEventArgs e)
    {
        if (e.Status != Autodesk.Revit.DB.Events.RevitAPIEventStatus.Succeeded)
        {
            return;
        }

        ChangeDocumentContext(new UIDocument(e.Document));
    }

    /// <summary>
    /// Gets or sets the current document.
    /// </summary>
    public Document? Document { get; set; }

    /// <summary>
    /// Gets or sets the current UI document.
    /// </summary>
    public UIDocument? UIDocument { get; set; }

    /// <summary>
    /// Gets or sets the Revit application UI instance.
    /// </summary>
    public UIApplication UIApplication { get; set; }

    /// <summary>
    /// Indicates whether the current document is valid.
    /// </summary>
    public bool IsVaildDocument { get; set; } = true;

    /// <summary>
    /// Disposes the CommandContext, detaching event handlers.
    /// </summary>
    public void Dispose()
    {
        UIApplication.Application.DocumentOpened -= Application_DocumentOpened;
        UIApplication.Application.DocumentCreated -= Application_DocumentCreated;
        UIApplication.Application.DocumentClosed -= Application_DocumentClosed;
    }
}
