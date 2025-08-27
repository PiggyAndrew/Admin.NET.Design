using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using BUD.Revit.Framework;
using BUD.Tuna.Revit.Extensions.Ribbon;
using BUD.Tuna.Revit.Extensions.Ribbon.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace BUD.Drainage.Revit.Addin;

/// <summary>
/// Main application class for the Revit Drainage System plugin
/// Responsible for initializing plugin interface and functionality
/// </summary>
public class App : ApplicationBase
{
    /// <summary>
    /// Initialize the application
    /// Override base class method to set up plugin interface elements
    /// </summary>
    /// <param name="applicationContext">Revit application context</param>
    public override void Initialize(HostApplicationContext applicationContext)
    {

        if (!Commands.ShowLatestChatCommand.IsPaneRegistered(applicationContext.UIControlledApplication))
        {
            Commands.ShowLatestChatCommand.RegisterDockablePane(applicationContext.UIControlledApplication);
        }

        RibbonInitialize(applicationContext.UIControlledApplication);
    }

    /// <summary>
    /// Create bitmap image from image name
    /// </summary>
    /// <param name="image">Image filename</param>
    /// <returns>Bitmap image object for Ribbon interface</returns>
    private BitmapImage CreateBitmapImageByURI(string image)
    {
        return new BitmapImage(new Uri($"pack://application:,,,/BUD.Drainage.Revit.Addin;component/Assets/icon/{image}"));
    }

    /// <summary>
    /// Initialize Ribbon interface
    /// Create plugin tabs, panels and buttons
    /// </summary>
    /// <param name="application">Revit UI application object</param>
    private void RibbonInitialize(UIControlledApplication application)
    {
        IRibbonTab ribbonTab = application.AddRibbonTab("AI");

        ribbonTab.AddRibbonPanel("AI", panel =>
        {
          
            panel.AddPushButton<Commands.MainAppViewCommand>(data =>
                data.LargeImage = CreateBitmapImageByURI("mass.png"));
        });
    }

    private void Application_DialogBoxShowing(object sender, Autodesk.Revit.UI.Events.DialogBoxShowingEventArgs e)
    {
        if (e.DialogId == "TaskDialog_Model_From_Future")
        {
            e.OverrideResult(1);
        }
    }
}
