using Autodesk.Revit.UI;
using DSD.Toolkit.Logger;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DSD.Toolkit.MVVM;

namespace BUD.Revit.Framework;

public abstract class ApplicationBase : DSDApplication, IExternalApplication
{
    public Result OnShutdown(UIControlledApplication application)
    {
        HostApplication.Current.Dispose();
        return Result.Succeeded;
    }

    public Result OnStartup(UIControlledApplication application)
    {
        ILogger logger = Logger.InitializeLogger(options =>
        {
            options.LogDirectory = Directory.GetParent(GetType().Assembly.Location).FullName;
            options.FileNamePattern = $"Revit_{{date}}_{{time}}.log";
        });

        logger.Information("Initialize Application");

        try
        {
            HostApplication hostApplication = HostApplication.CreatHostAppllication(application);
            Initialize(hostApplication.ApplicationContext);

            hostApplication.Logger = logger;
        }
        catch (Exception e)
        {
            logger.Exception(e);
        }

        return Result.Succeeded;
    }

    public abstract void Initialize(HostApplicationContext applicationContext);
}
